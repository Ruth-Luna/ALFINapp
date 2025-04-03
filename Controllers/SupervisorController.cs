using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ALFINapp.Infrastructure.Services;
using ALFINapp.Application.Interfaces.Supervisor;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class SupervisorController : Controller
    {
        private readonly MDbContext _context;
        private readonly DBServicesConsultasSupervisores _dbServicesConsultasSupervisores;
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly IUseCaseGetInicio _useCaseGetInicioSup;
        public SupervisorController(
            MDbContext context, 
            DBServicesConsultasSupervisores dbServicesConsultasSupervisores, 
            DBServicesGeneral dbServicesGeneral,
            IUseCaseGetInicio useCaseGetInicioSup)
        {
            _context = context;
            _dbServicesConsultasSupervisores = dbServicesConsultasSupervisores;
            _dbServicesGeneral = dbServicesGeneral;
            _useCaseGetInicioSup = useCaseGetInicioSup;
        }
        /// <summary>
        /// Obtiene y muestra la página de inicio del supervisor con información sobre los leads y clientes asignados.
        /// </summary>
        /// <param name="page">Número de página actual para la paginación. Valor predeterminado: 1</param>
        /// <param name="pageSize">Cantidad de elementos por página. Valor predeterminado: 20</param>
        /// <returns>
        /// Vista "Inicio" con la lista de supervisores y datos estadísticos en ViewData:
        /// - UsuarioNombre: Nombre completo del supervisor actual
        /// - ClientesPendientesSupervisor: Número de clientes sin asesor asignado
        /// - DestinoBases: Lista de bases de datos disponibles
        /// - clientesAsignadosSupervisor: Número de clientes con asesor asignado
        /// - totalClientes: Número total de clientes
        /// </returns>
        /// <remarks>
        /// El método realiza las siguientes operaciones:
        /// - Verifica la autenticación del usuario
        /// - Consulta los leads asignados al supervisor
        /// - Calcula estadísticas de asignación de clientes
        /// - Obtiene las bases de datos disponibles para el supervisor
        /// - Limita la lista de supervisores a 200 registros
        /// 
        /// Los datos se almacenan en ViewData para su uso en la vista.
        /// </remarks>
        /// <example>
        /// Uso típico desde una vista:
        /// <code>
        /// @{
        ///     var nombreSupervisor = ViewData["UsuarioNombre"] as string;
        ///     var clientesPendientes = (int)ViewData["ClientesPendientesSupervisor"];
        ///     var totalClientes = (int)ViewData["totalClientes"];
        /// }
        /// </code>
        /// </example>
        /// <exception cref="Exception">
        /// Pueden ocurrir excepciones durante:
        /// - La consulta a la base de datos
        /// - La obtención de datos de sesión
        /// - El procesamiento de los datos del supervisor
        /// </exception>
        [HttpGet]
        [PermissionAuthorization("Supervisor", "Inicio")]
        public async Task<IActionResult> Inicio(int page = 1, int pageSize = 20)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? IdRol = HttpContext.Session.GetInt32("RolUser");
            if (IdRol == null || usuarioId == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión";
                return RedirectToAction("Index", "Home");
            }

            var supervisorData = await _useCaseGetInicioSup.Execute(usuarioId.Value);
            if (supervisorData.IsSuccess == false)
            {
                TempData["MessageError"] = supervisorData.Message;
                return RedirectToAction("Index", "Home");
            }

            return View("Inicio", supervisorData.Data);
        }

        [HttpPost]
        public IActionResult AsignarVendedoresPorNumero(int nclientes, int id_vendedor)
        {
            if (!int.TryParse(nclientes.ToString(), out int n_clientes) || nclientes <= 0)
            {
                TempData["MessageError"] = "La entrada debe de ser un numero valido y positivo.";
                return RedirectToAction("Redireccionar", "Error");
            }

            var idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesion.";
                return RedirectToAction("Redireccionar", "Error");
            }
            var clientesDisponibles = _context.clientes_asignados
                                            .Where(ca => ca.IdUsuarioS == idSupervisorActual.Value && ca.IdUsuarioV == null && ca.FechaAsignacionSup.HasValue && ca.FechaAsignacionSup.Value.Year == 2025
                                            && ca.FechaAsignacionSup.Value.Month == 1)
                                            .Take(nclientes)
                                            .ToList();
            Console.WriteLine($"Solo hay {clientesDisponibles.Count} clientes disponibles para asignar.");

            if (clientesDisponibles.Count < nclientes)
            {
                TempData["MessageError"] = $"Solo hay {clientesDisponibles.Count} clientes disponibles para asignar.";
                return RedirectToAction("Redireccionar", "Error");
            }

            foreach (var cliente in clientesDisponibles)
            {
                Console.WriteLine($"Estamos asignando el cliente {cliente.IdCliente}");
                cliente.IdUsuarioV = id_vendedor;
                cliente.FechaAsignacionVendedor = DateTime.Now;
            }

            // Guardar los cambios en la base de datos
            _context.SaveChanges();
            TempData["Message"] = $"{nclientes} clientes han sido asignados correctamente al Asesor.";
            return RedirectToAction("Redireccionar", "Error");
        }

        [HttpGet]
        public IActionResult ModificarAsignacionVendedorView(int id_asignacion)
        {
            var idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión";
                return RedirectToAction("Index", "Home");
            }

            var vendedoresAsignados = (from u in _context.usuarios
                                       where u.IDUSUARIOSUP == idSupervisorActual.Value && u.Rol == "VENDEDOR"
                                       join ca in _context.clientes_asignados
                                       on u.IdUsuario equals ca.IdUsuarioV into clientes
                                       from ca in clientes.DefaultIfEmpty()
                                       group ca by new
                                       {
                                           u.NombresCompletos,
                                           u.IdUsuario,
                                       } into grouped
                                       select new VendedorConClientesDTO
                                       {
                                           NombresCompletos = grouped.Key.NombresCompletos,
                                           IdUsuario = grouped.Key.IdUsuario,
                                           NumeroClientes = grouped.Count(c => c != null) // Contamos solo los clientes asignados, ignorando los null
                                       }).ToList();
            if (vendedoresAsignados == null)
            {
                TempData["MessageError"] = "La consulta para dar asesores ha fallado";
                return RedirectToAction("Redireccionar", "Error");
            }

            TempData["idAsignacion"] = id_asignacion;
            return PartialView("_ModificarAsignacion", vendedoresAsignados);
        }
        [HttpGet]
        public IActionResult ModificarAsignacionVendedor(int idasignado, int idVendedor)
        {
            var asignacion = _context.clientes_asignados.FirstOrDefault(c => c.IdAsignacion == idasignado);
            if (asignacion == null)
            {
                TempData["MessageError"] = "El id de Asignacion mandado es incorrecto";
                return RedirectToAction("Redireccionar", "Error");
            }
            
            if (asignacion.IdUsuarioV == idVendedor)
            {
                TempData["MessageError"] = "Debe seleccionar un Asesor diferente, ya que el Asesor actual es el mismo.";
                return RedirectToAction("Redireccionar", "Error");
            }
            if (asignacion != null)
            {
                asignacion.IdUsuarioV = idVendedor;
                asignacion.FechaAsignacionVendedor = DateTime.Now;
                _context.SaveChanges();

                TempData["Message"] = $"{asignacion.IdCliente} ha sido asignado al Asesor {idVendedor}.";
            }

            else
            {
                TempData["Message"] = "El id de Asignacion mandado es incorrecto";
            }

            return RedirectToAction("Redireccionar", "Error");
        }

        [HttpGet]
        public IActionResult ObtenerVistaModificarAsignaciones(string IdUsuario, string dni)
        {
            try
            {
                int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
                if (!int.TryParse(IdUsuario, out int idUsuario))
                {
                    return Json(new { success = false, message = "IdUsuario inválido" });
                }
                var clientesAsignadosAsesorPrincipal = (from u in _context.usuarios
                                                        where u.IdUsuario == idUsuario
                                                        join ca in _context.clientes_asignados on u.IdUsuario equals ca.IdUsuarioV into caGroup
                                                        from ca in caGroup.DefaultIfEmpty()  // Realizamos un left join
                                                        group new { u, ca }
                                                        by new
                                                        {
                                                            u.IdUsuario,
                                                            u.NombresCompletos,
                                                            u.Dni,
                                                            u.Telefono,
                                                            u.Departamento,
                                                            u.Provincia,
                                                            u.Distrito,
                                                            u.Estado,
                                                            u.Rol
                                                        } into grouped
                                                        select new UsuarioAsesorDTO
                                                        {
                                                            IdUsuario = grouped.Key.IdUsuario,
                                                            Dni = grouped.Key.Dni,
                                                            NombresCompletos = grouped.Key.NombresCompletos,
                                                            Telefono = grouped.Key.Telefono,
                                                            Departamento = grouped.Key.Departamento,
                                                            Provincia = grouped.Key.Provincia,
                                                            Distrito = grouped.Key.Distrito,
                                                            Estado = grouped.Key.Estado,
                                                            Rol = grouped.Key.Rol,
                                                            TotalClientesAsignados = grouped.Count(g => g.ca != null
                                                                                                && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                                && g.ca.IdUsuarioS == idSupervisorActual
                                                                                                && g.ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                                                && g.ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month), // Clientes asignados
                                                            ClientesTrabajando = grouped.Count(g => g.ca != null
                                                                                                && g.ca.TipificacionMayorPeso != null
                                                                                                && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                                && g.ca.IdUsuarioS == idSupervisorActual
                                                                                                && g.ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                                                && g.ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month), // Clientes trabajados
                                                            ClientesSinTrabajar = grouped.Count(g => g.ca != null
                                                                                                && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                                && g.ca.IdUsuarioS == idSupervisorActual
                                                                                                && g.ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                                                && g.ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month)
                                                                                                 - grouped.Count(g => g.ca != null
                                                                                                && g.ca.TipificacionMayorPeso != null
                                                                                                && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                                && g.ca.IdUsuarioS == idSupervisorActual
                                                                                                && g.ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                                                && g.ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month) // Diferencia entre asignados y trabajados
                                                        }).FirstOrDefault();
                var asesoresAsignadosaSupervisor = (from u in _context.usuarios
                                                    where u.Rol == "VENDEDOR" && u.IDUSUARIOSUP == idSupervisorActual && u.IdUsuario != idUsuario
                                                    join ca in _context.clientes_asignados on u.IdUsuario equals ca.IdUsuarioV into caGroup
                                                    from ca in caGroup.DefaultIfEmpty()  // Realizamos un left join
                                                    group new { u, ca }
                                                    by new
                                                    {
                                                        u.IdUsuario,
                                                        u.NombresCompletos,
                                                        u.Dni,
                                                        u.Telefono,
                                                        u.Departamento,
                                                        u.Provincia,
                                                        u.Distrito,
                                                        u.Estado,
                                                        u.Rol
                                                    } into grouped
                                                    select new UsuarioAsesorDTO
                                                    {
                                                        IdUsuario = grouped.Key.IdUsuario,
                                                        Dni = grouped.Key.Dni,
                                                        NombresCompletos = grouped.Key.NombresCompletos,
                                                        Telefono = grouped.Key.Telefono,
                                                        Departamento = grouped.Key.Departamento,
                                                        Provincia = grouped.Key.Provincia,
                                                        Distrito = grouped.Key.Distrito,
                                                        Estado = grouped.Key.Estado,
                                                        Rol = grouped.Key.Rol,
                                                        TotalClientesAsignados = grouped.Count(g => g.ca != null
                                                                                            && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                            && g.ca.IdUsuarioS == idSupervisorActual
                                                                                            && g.ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                                            && g.ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month), // Clientes asignados
                                                        ClientesTrabajando = grouped.Count(g => g.ca != null
                                                                                            && g.ca.TipificacionMayorPeso != null
                                                                                            && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                            && g.ca.IdUsuarioS == idSupervisorActual
                                                                                            && g.ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                                            && g.ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month), // Clientes trabajados
                                                        ClientesSinTrabajar = grouped.Count(g => g.ca != null
                                                                                            && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                            && g.ca.IdUsuarioS == idSupervisorActual)
                                                                                             - grouped.Count(g => g.ca != null
                                                                                            && g.ca.TipificacionMayorPeso != null
                                                                                            && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                            && g.ca.IdUsuarioS == idSupervisorActual
                                                                                            && g.ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                                            && g.ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month) // Diferencia entre asignados y trabajados
                                                    }).ToList();
                ViewData["AsesorAModificar"] = clientesAsignadosAsesorPrincipal;
                // Retorna la vista parcial con los datos necesarios
                return PartialView("_VistaModificarAsignaciones", asesoresAsignadosaSupervisor);
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Ha ocurrido un Error al mandar los Datos" });
                throw;
            }
        }
        [HttpGet]
        public IActionResult ObtenerInterfazAsesor(int idUsuario)
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            var asesorBusqueda = (from u in _context.usuarios
                                  where u.IdUsuario == idUsuario
                                  select new UsuarioAsesorDTO
                                  {
                                      IdUsuario = u.IdUsuario,
                                      Dni = u.Dni,
                                      NombresCompletos = u.NombresCompletos,
                                      Telefono = u.Telefono,
                                      Departamento = u.Departamento,
                                      Provincia = u.Provincia,
                                      Distrito = u.Distrito,
                                      Region = u.REGION,
                                      Estado = u.Estado,
                                      Rol = u.Rol,
                                      TotalClientesAsignados = _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario),
                                      ClientesTrabajando = _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario
                                                                                            && ca.TipificacionMayorPeso != null),
                                      ClientesSinTrabajar = _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario) -
                                                            _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario &&
                                                                                            ca.TipificacionMayorPeso != null)
                                  }).FirstOrDefault();
            Console.WriteLine($"El Asesor {asesorBusqueda.NombresCompletos} ha sido encontrado");
            if (asesorBusqueda == null)
            {
                Console.WriteLine("El Asesor no ha sido encontrado");
                return Json(new { success = false, message = "La entrada no ha sido ocurrido ha ocurrido un error" });
            }
            Console.WriteLine("Retornando la vista parcial");
            return PartialView("_InterfazActivarAsesor", asesorBusqueda); // Retorna una vista parcial
        }

        [HttpGet]
        public IActionResult InformesTipificacionesView()
        {
            try
            {
                return PartialView("_InformesTipificacionesAsesores");
            }
            catch (System.Exception ex)
            {
                return Json(new { error = true, message = ex.Message });  // Devuelve el error
                throw;
            }
        }
    }
}