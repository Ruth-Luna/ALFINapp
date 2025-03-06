using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ALFINapp.Filters;
using ALFINapp.Models;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.Controllers
{
    [RequireSession]

    public class AsesorController : Controller
    {
        private readonly DBServicesAsignacionesAsesores _dbServicesAsignacionesAsesores;
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesEstadoAsesores _dbServicesEstadoAsesores;
        private readonly MDbContext _context;
        private readonly DBServicesUsuarios _dBServicesUsuarios;
        private readonly DBServicesConsultasSupervisores _dBServicesConsultasSupervisores;

        public AsesorController(
            DBServicesAsignacionesAsesores dbServicesAsignacionesAsesores,
            DBServicesGeneral dbServicesGeneral,
            DBServicesEstadoAsesores dbServicesEstadoAsesores,
            MDbContext context,
            DBServicesUsuarios dBServicesUsuarios,
            DBServicesConsultasSupervisores dBServicesConsultasSupervisores)
        {
            _dbServicesAsignacionesAsesores = dbServicesAsignacionesAsesores;
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesEstadoAsesores = dbServicesEstadoAsesores;
            _context = context;
            _dBServicesUsuarios = dBServicesUsuarios;
            _dBServicesConsultasSupervisores = dBServicesConsultasSupervisores;
        }

        [HttpPost]
        public async Task<IActionResult> ActivarAsesor(string DNI, int idUsuario)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? rol = HttpContext.Session.GetInt32("RolUser");
            if (usuarioId == null || rol == null)
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
        public async Task<IActionResult> DesactivarAsesor(string DNI, int idUsuario)
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
                                && ca.FechaAsignacionVendedor != null
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

        /// <summary>
        /// Agrega un nuevo usuario al sistema con rol de asesor u otros roles permitidos.
        /// </summary>
        /// <param name="nuevoUsuario">Objeto Usuario que contiene la información del nuevo usuario a crear.
        /// Debe incluir DNI, nombres completos, rol y datos de ubicación.</param>
        /// <returns>
        /// IActionResult que contiene un objeto JSON con:
        /// - success: true si la operación fue exitosa, false si hubo errores
        /// - message: Mensaje descriptivo del resultado de la operación
        /// </returns>
        /// <remarks>
        /// El método realiza las siguientes validaciones:
        /// - Verifica que el usuario no sea nulo
        /// - Comprueba que el DNI no esté ya registrado
        /// - Valida que el DNI tenga al menos 8 dígitos
        /// - Verifica que haya un supervisor activo en la sesión
        /// - Asigna automáticamente el supervisor actual como responsable
        /// - Convierte los datos de texto a mayúsculas
        /// - Asigna el rol correspondiente según el IdRol proporcionado
        /// 
        /// La contraseña se genera automáticamente usando el formato: DNI$clave123
        /// </remarks>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// var nuevoUsuario = new Usuario
        /// {
        ///     Dni = "12345678",
        ///     NombresCompletos = "Juan Pérez",
        ///     IdRol = 3,
        ///     Departamento = "Lima",
        ///     Provincia = "Lima",
        ///     Distrito = "San Isidro",
        ///     REGION = "Costa"
        /// };
        /// var resultado = await AgregarNuevoAsesor(nuevoUsuario);
        /// </code>
        /// </example>
        /// <exception cref="System.Exception">
        /// Captura y maneja excepciones generales durante el proceso de creación del usuario.
        /// Los mensajes de error se devuelven en la respuesta JSON.
        /// </exception>
        [HttpPost]
        public async Task<IActionResult> AgregarNuevoAsesor([FromBody] Usuario nuevoUsuario)
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
                    return Json(new { success = false, message = "El Documento ya está registrado en la base de datos." });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Los datos enviados no son válidos" });
                }

                if (string.IsNullOrEmpty(nuevoUsuario.Dni) ||
                    (nuevoUsuario.Dni.Length != 8 && nuevoUsuario.Dni.Length != 9) ||
                    !nuevoUsuario.Dni.All(char.IsDigit))
                {
                    return Json(new { success = false, message = "El DNI o documento de Identidad debe contener 8 o 9 dígitos numéricos." });
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
                    nuevoUsuario.Rol = "ASESOR";
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
                if (idsupervisoractual == null)
                {
                    return Json(new { success = false, message = "El ID Supervisor a asignar automaticamente es invalido. Comunicarse con Soporte Tecnico." });
                }
                var EnviarNuevoUsuario = await _dBServicesUsuarios.CrearUsuario(nuevoUsuario, idsupervisoractual.Value);
                if (!EnviarNuevoUsuario.IsSuccess)
                {
                    return Json(new { success = false, message = EnviarNuevoUsuario.Message });
                }
                return Json(new { success = true, message = $"Se ha agregado al nuevo Usuario {nuevoUsuario.NombresCompletos} con el Rol {nuevoUsuario.Rol}. Usted ha sido asignado como Supervisor del Asesor de Creditos" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Activates an advisor based on the provided DNI and user ID.
        /// </summary>
        /// <param name="DNI">The DNI of the advisor to be activated.</param>
        /// <param name="idUsuario">The ID of the user performing the activation.</param>
        /// <returns>A JSON result indicating the success or failure of the activation process.</returns>
        /// <remarks>
        /// This method checks if the user is authenticated and verifies the provided DNI and user ID.
        /// If the advisor is already active, it returns a message indicating so.
        /// Otherwise, it attempts to activate the advisor and returns the result.
        /// </remarks>
        /// <exception cref="System.Exception">
        /// Se capturan todas las excepciones y se devuelven como un mensaje de error en el JSON de respuesta.
        /// </exception>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// var asignaciones = new List<AsignarAsesorDTO> {
        ///     new AsignarAsesorDTO { IdVendedor = 1, NumClientes = 5 },
        ///     new AsignarAsesorDTO { IdVendedor = 2, NumClientes = 3 }
        /// };
        /// var resultado = await AsignarClientesAAsesores(asignaciones, "BaseClientes2024");
        /// </code>
        /// </example>
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
                var totalClientes = await _dBServicesConsultasSupervisores.ConsultaLeadsDelSupervisorDestino(idSupervisorActual.Value, selectAsesorBase);
                if (!totalClientes.IsSuccess || totalClientes.Data == null)
                {
                    return Json(new { success = false, message = $"Ha ocurrido un error al obtener los clientes disponibles. {totalClientes.Message}" });
                }
                foreach (var asignacion in asignacionasesor)
                {
                    if (asignacion.NumClientes == 0)
                    {
                        continue;
                    }

                    int nClientes = asignacion.NumClientes;
                    contadorClientesAsignados = contadorClientesAsignados + nClientes;
                    var clientesDisponibles = totalClientes.Data;
                    if (clientesDisponibles.Count < nClientes)
                    {
                        mensajesError = mensajesError + $"En la base '{selectAsesorBase}', solo hay {clientesDisponibles.Count} clientes disponibles para la asignación. La entrada ha sido obviada.";
                        continue;
                    }
                    if (contadorClientesAsignados > clientesDisponibles.Count)
                    {
                        mensajesError = mensajesError + $"Ha ocurrido un error al asignar los clientes. Esta mandando mas entradas del total disponible";
                        continue;
                    }
                }
                if (mensajesError != " ")
                {
                    return Json(new { success = false, message = $"{mensajesError}" });
                }
                contadorClientesAsignados = 0;
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
                        return Json(new { success = false, message = $"{getClientesDisponibles.message}" });
                    }
                    var clientesDisponibles = getClientesDisponibles.data;
                    foreach (var cliente in clientesDisponibles)
                    {
                        cliente.IdUsuarioV = asignacion.IdVendedor;
                        cliente.FechaAsignacionVendedor = DateTime.Now;
                        var result = await _dbServicesAsignacionesAsesores.ActualizarClienteAsignado(cliente);
                        if (!result.IsSuccess)
                        {
                            return Json(new { success = false, message = $"{result.message}" });
                        }
                    }
                }
                if (mensajesError != " ")
                {
                    return Json(new { success = false, message = $"{mensajesError}" });
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