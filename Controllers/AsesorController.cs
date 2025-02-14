using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ALFINapp.Models;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.Controllers
{
    public class AsesorController : Controller
    {
        private readonly List<int> rolesPermitidos = new List<int> { 1, 2, 3 };
        private readonly DBServicesAsignacionesAsesores _dbServicesAsignacionesAsesores;
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesEstadoAsesores _dbServicesEstadoAsesores;
        private readonly MDbContext _context;

        public AsesorController(
            DBServicesAsignacionesAsesores dbServicesAsignacionesAsesores, 
            DBServicesGeneral dbServicesGeneral, 
            DBServicesEstadoAsesores dbServicesEstadoAsesores,
            MDbContext context)
        {
            _dbServicesAsignacionesAsesores = dbServicesAsignacionesAsesores;
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesEstadoAsesores = dbServicesEstadoAsesores;
            _context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> ActivarAsesor(string DNI, int idUsuario)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["Message"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                //Verificar Datos Enviados
                if (string.IsNullOrEmpty(DNI))
                {
                    return Json(new { success = false, message = "Debe ingresar el DNI del asesor" });
                }
                if (idUsuario == 0)
                {
                    return Json(new { success = false, message = "Debe ingresar el Id del asesor" });
                }

                var GetAsesorParaActivar = await _dbServicesGeneral.GetUserInformation(idUsuario);
                if (!GetAsesorParaActivar.IsSuccess)
                {
                    return Json(new { success = false, message = GetAsesorParaActivar.Message });
                }
                var asesorParaActivar = GetAsesorParaActivar.Data;
                if (asesorParaActivar == null)
                {
                    return Json(new { success = false, message = "No se encontró el asesor" });
                }
                if (asesorParaActivar.Estado == "ACTIVO")
                {
                    return Json(new { success = false, message = "El asesor ya se encuentra activo" });
                }
                var estadoActivacion = await _dbServicesEstadoAsesores.ActivarAsesor(asesorParaActivar, usuarioId.Value);
                if (!estadoActivacion.IsSuccess)
                {
                    return Json(new { success = false, message = estadoActivacion.message });
                }
                return Json(new { success = true, message = "Asesor activado correctamente" });
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Ha ocurrido un error al activar el asesor" });
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> DesactivarAsesorAsync(string DNI, int idUsuario)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["Message"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                //Verificar Datos Enviados
                if (string.IsNullOrEmpty(DNI))
                {
                    return Json(new { success = false, message = "Debe ingresar el DNI del asesor" });
                }
                if (idUsuario == 0)
                {
                    return Json(new { success = false, message = "Debe ingresar el Id del asesor" });
                }

                var getAsesorParaDesactivar = await _dbServicesGeneral.GetUserInformation(idUsuario);
                if (!getAsesorParaDesactivar.IsSuccess)
                {
                    return Json(new { success = false, message = getAsesorParaDesactivar.Message });
                }
                var asesorParaDesactivar = getAsesorParaDesactivar.Data;

                if (asesorParaDesactivar == null)
                {
                    return Json(new { success = false, message = "No se encontró el asesor" });
                }
                if (asesorParaDesactivar.Estado == "INACTIVO")
                {
                    return Json(new { success = false, message = "El asesor ya se encuentra inactivo" });
                }

                var estadoDeactivacion = await _dbServicesEstadoAsesores.DesactivarAsesor(asesorParaDesactivar, usuarioId.Value);
                if (!estadoDeactivacion.IsSuccess)
                {
                    return Json(new { success = false, message = estadoDeactivacion.message });
                }
                return Json(new { success = true, message = "Asesor desactivado correctamente" });
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Ha ocurrido un error al desactivar el asesor" });
                throw;
            }
        }

        [HttpPost]
        public IActionResult GuardarCambiosAsignaciones(List<AsignacionesDTO> AsignacionesEnviadas, int AsesorCambioID)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            try
            {
                if (AsignacionesEnviadas == null)
                {
                    return Json(new { success = false, message = "Llene al menos un campo de la seccion Modificar Clientes Asignados" });
                }

                if (AsesorCambioID == 0)
                {
                    return Json(new { success = false, message = "Debe ingresar el Id del asesor al que se asignarán los clientes" });
                }

                bool cambiosRealizados = false;
                foreach (var asignacion in AsignacionesEnviadas)
                {
                    if (asignacion.Modificaciones == 0)
                    {
                        continue;
                    }
                    var clientesAModificar = _context.clientes_asignados
                        .Where(ca => ca.IdUsuarioV == asignacion.IdUsuario
                                && ca.TipificacionMayorPeso == null
                                && ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                && ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month)
                        .Take(asignacion.Modificaciones)
                        .ToList();
                    if (clientesAModificar.Count < asignacion.Modificaciones)
                    {
                        return Json(new { success = false, message = $"No hay suficientes clientes disponibles para asignar. Solo hay {clientesAModificar.Count} clientes disponibles. Tal asignacion ha sido Obviada" });
                    }

                    foreach (var cliente in clientesAModificar)
                    {
                        cliente.IdUsuarioV = AsesorCambioID;
                        cliente.FechaAsignacionVendedor = DateTime.Now;
                    }
                    cambiosRealizados = true;

                    _context.SaveChanges();
                }
                if (!cambiosRealizados)
                {
                    return Json(new { success = false, message = "No se realizaron modificaciones. No hay clientes para asignar." });
                }

                return Json(new { success = true, message = "Se han modificado los clientes asignados al asesor" });
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Llene al menos un campo de la seccion Modificar Clientes Asignados" });
                throw;
            }
        }

        [HttpPost]
        public IActionResult AgregarNuevoAsesor([FromBody] Usuario nuevoUsuario)
        {
            if (nuevoUsuario == null)
            {
                return Json(new { success = false, message = "El usuario no puede ser nulo." });
            }
            try
            {
                var usuarioExistente = _context.usuarios.FirstOrDefault(u => u.Dni == nuevoUsuario.Dni);
                if (usuarioExistente != null)
                {
                    return Json(new { success = false, message = "El DNI ya está registrado en la base de datos." });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Los datos enviados no son válidos" });
                }

                if (string.IsNullOrEmpty(nuevoUsuario.Dni) || nuevoUsuario.Dni.Length < 8)
                {
                    return Json(new { success = false, message = "El DNI debe contener al menos 8 dígitos." });
                }

                int? idsupervisoractual = HttpContext.Session.GetInt32("UsuarioId");

                if (idsupervisoractual == null)
                {
                    return Json(new { success = false, message = "El ID Supervisor a asignar automaticamente es invalido. Comunicarse con Soporte Tecnico." });
                }
                else
                {
                    var supervisorData = _context.usuarios.AsNoTracking().FirstOrDefault(u => u.IdUsuario == idsupervisoractual);
                    if (supervisorData == null)
                    {
                        return Json(new { success = false, message = "No se encontró el supervisor actual." });
                    }
                    nuevoUsuario.IDUSUARIOSUP = idsupervisoractual ?? 0;
                    nuevoUsuario.RESPONSABLESUP = supervisorData.NombresCompletos;
                }

                nuevoUsuario.NombresCompletos = nuevoUsuario.NombresCompletos?.ToUpper();
                if (nuevoUsuario.IdRol == 0)
                {
                    return Json(new { success = false, message = "Debe seleccionar un Rol para el nuevo usuario" });
                }
                nuevoUsuario.IdRol = nuevoUsuario.IdRol;
                if (nuevoUsuario.IdRol == 1)
                {
                    nuevoUsuario.Rol = "ADMINISTRADOR";
                }
                else if (nuevoUsuario.IdRol == 2)
                {
                    nuevoUsuario.Rol = "SUPERVISOR";
                }
                else if (nuevoUsuario.IdRol == 3)
                {
                    nuevoUsuario.Rol = "VENDEDOR";
                }
                else
                {
                    nuevoUsuario.Rol = "DESCONOCIDO";
                }
                nuevoUsuario.Departamento = nuevoUsuario.Departamento?.ToUpper();
                nuevoUsuario.Provincia = nuevoUsuario.Provincia?.ToUpper();
                nuevoUsuario.Distrito = nuevoUsuario.Distrito?.ToUpper();
                nuevoUsuario.REGION = nuevoUsuario.REGION?.ToUpper();

                nuevoUsuario.FechaRegistro = DateTime.Now;
                nuevoUsuario.Estado = "ACTIVO";
                nuevoUsuario.contraseña = $"{nuevoUsuario.Dni}$clave123";
                _context.usuarios.Add(nuevoUsuario);
                _context.SaveChanges();
                return Json(new { success = true, message = $"Se ha agregado al nuevo Usuario {nuevoUsuario.NombresCompletos} con el Rol {nuevoUsuario.Rol}" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = ex.Message});
            }
        }

        /// <summary>
        /// Obtiene el número de clientes asignados a un asesor específico que coinciden con una tipificación detallada.
        /// </summary>
        /// <param name="tipificacionDetalle">La tipificación detallada para filtrar los clientes.</param>
        /// <param name="idAsesorBuscar">El ID del asesor para el cual se buscan los clientes asignados.</param>
        /// <returns>
        /// Un IActionResult que contiene un objeto JSON con:
        /// - success: true si la operación fue exitosa, false en caso contrario.
        /// - numClientes: El número de clientes que coinciden con la tipificación especificada.
        /// - message: Un mensaje de error en caso de que ocurra una excepción.
        /// </returns>
        [HttpGet]
        public IActionResult ObtenerNumDeClientesPorTipificacion(string tipificacionDetalle, int idAsesorBuscar)
        {
            try
            {
                int? idsupervisoractual = HttpContext.Session.GetInt32("UsuarioId");
                if (idsupervisoractual == null)
                {
                    return Json(new { success = false, message = "El ID Supervisor a asignar automaticamente es invalido. Comunicarse con Soporte Tecnico." });
                }

                var NumClientesAsignados = _context.clientes_asignados
                    .Where(ca => ca.IdUsuarioV == idAsesorBuscar
                        && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                        && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                        && ca.TipificacionMayorPeso == tipificacionDetalle
                        && ca.IdUsuarioS == idsupervisoractual
                        )
                    .Count();
                return Json(new { success = true, numClientes = NumClientesAsignados });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Ha ocurrido un error inesperado" });
            }
        }


        /// <summary>
        /// Modifica las asignaciones de clientes basándose en una tipificación específica y las reasigna a un asesor determinado.
        /// </summary>
        /// <param name="TipificacionModificar">La tipificación de los clientes que se desean reasignar.</param>
        /// <param name="idAsesorAsignar">El ID del asesor al que se asignarán los clientes.</param>
        /// <param name="numDeModificaciones">El número de asignaciones que se desean modificar.</param>
        /// <returns>
        /// Un IActionResult que contiene un objeto JSON con:
        /// - success: true si la operación fue exitosa, false en caso contrario.
        /// - message: Un mensaje describiendo el resultado de la operación.
        /// </returns>
        [HttpPost]
        public IActionResult ModificarAsignacionesPorTipificaciones(string TipificacionModificar, int idAsesorAsignar, int numDeModificaciones)
        {
            try
            {
                int? idsupervisoractual = HttpContext.Session.GetInt32("UsuarioId");
                if (idsupervisoractual == null)
                {
                    return Json(new { success = false, message = "El ID Supervisor es invalido. Comunicarse con Soporte Tecnico." });
                }
                // Verificar si los parámetros son nulos o inválidos
                if (string.IsNullOrEmpty(TipificacionModificar))
                {
                    return Json(new { success = false, message = "La tipificación a modificar no puede estar vacía." });
                }

                if (numDeModificaciones <= 0)
                {
                    return Json(new { success = false, message = "El número de modificaciones debe ser mayor que cero." });
                }

                if (idAsesorAsignar <= 0)
                {
                    return Json(new { success = false, message = "El ID del asesor a asignar es inválido." });
                }

                // Obtener el ID del supervisor actual
                int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
                if (idSupervisorActual == null)
                {
                    return Json(new { success = false, message = "No se pudo obtener el ID del supervisor actual." });
                }

                // Buscar y actualizar las asignaciones
                var asignacionesDisponibles = _context.clientes_asignados
                        .Where(ca => ca.IdUsuarioS == idSupervisorActual &&
                                    ca.TipificacionMayorPeso == TipificacionModificar &&
                                    ca.IdUsuarioV != idAsesorAsignar
                                     && ca.IdUsuarioS == idsupervisoractual
                                     && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                     && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month)
                        .ToList();

                if (asignacionesDisponibles.Count < numDeModificaciones)
                {
                    return Json(new { success = false, message = $"No hay suficientes asignaciones para modificar. Se solicitaron {numDeModificaciones}, pero solo hay {asignacionesDisponibles.Count} disponibles." });
                }

                var asignacionesAModificar = asignacionesDisponibles.Take(numDeModificaciones).ToList();

                foreach (var asignacion in asignacionesAModificar)
                {
                    asignacion.IdUsuarioV = idAsesorAsignar;
                    asignacion.FechaAsignacionVendedor = DateTime.Now;
                }

                _context.SaveChanges();
                return Json(new { success = true, message = $"Se han modificado {asignacionesAModificar.Count} asignaciones correctamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "Ha ocurrido un error inesperado al modificar las asignaciones." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AsignarClientesAAsesores(List<AsignarAsesorDTO> asignacionasesor, string selectAsesorBase)
        {
            try
            {
                int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
                if (idSupervisorActual == null)
                {
                    return Json(new { success = false, message = "No se pudo obtener el ID del supervisor actual recuerde Iniciar Sesion." });
                }

                string mensajesError = " ";
                if (asignacionasesor == null)
                {
                    return Json(new { success = false, message = "No se han enviado datos para asignar asesores." });
                }

                if (string.IsNullOrEmpty(selectAsesorBase))
                {
                    return Json(new { success = false, message = "Debe seleccionar un Destino de la Base." });
                }

                // Comprobación adicional para verificar si todas las entradas tienen NumClientes igual a 0
                if (asignacionasesor.All(a => a.NumClientes == 0))
                {
                    return Json(new { success = false, message = "No se ha llenado ninguna entrada. Los campos no pueden estar vacíos." });
                }
                int contadorClientesAsignados = 0;
                foreach (var asignacion in asignacionasesor)
                {
                    Console.WriteLine($"IdVendedor: {asignacion.IdVendedor}, NumClientes: {asignacion.NumClientes}");
                    if (asignacion.NumClientes == 0)
                    {
                        continue;
                    }

                    int nClientes = asignacion.NumClientes;
                    contadorClientesAsignados = contadorClientesAsignados + nClientes;
                    var getClientesDisponibles = await _dbServicesAsignacionesAsesores.ObtenerClientesDisponibles(idSupervisorActual.Value, selectAsesorBase, nClientes);
                    if (!getClientesDisponibles.IsSuccess || getClientesDisponibles.data == null)
                    {
                        mensajesError = mensajesError + $"Ha ocurrido un error al obtener los clientes disponibles. {getClientesDisponibles.message}";
                        continue;
                    }

                    var clientesDisponibles = getClientesDisponibles.data;
                    
                    if (clientesDisponibles.Count < nClientes)
                    {
                        mensajesError = mensajesError + $"En la base '{selectAsesorBase}', solo hay {clientesDisponibles.Count} clientes disponibles para la asignación. La entrada ha sido obviada.";
                        continue;
                    }

                    foreach (var cliente in clientesDisponibles)
                    {
                        cliente.IdUsuarioV = asignacion.IdVendedor;
                        cliente.FechaAsignacionVendedor = DateTime.Now;
                    }
                    _context.SaveChanges();
                }
                if (mensajesError != " ")
                {
                    return Json(new { success = true, message = $"{mensajesError}" });
                }
                else
                {
                    return Json(new { success = true, message = $"Se han modificado {contadorClientesAsignados} asignaciones correctamente." });
                }
            }

            catch (System.Exception ex)
            {
                return Json(new { success = false, message = $"Ha ocurrido un error inesperado al modificar las asignaciones. {ex.Message}" });
            }
        }

    }
}