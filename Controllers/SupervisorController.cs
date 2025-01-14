using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using ALFINapp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class SupervisorController : Controller
    {
        private readonly MDbContext _context;
        public SupervisorController(MDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> VistaMainSupervisor(int page = 1, int pageSize = 20)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            var supervisorData = from ca in _context.clientes_asignados
                                 join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                 join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                                 join db in _context.detalle_base on bc.IdBase equals db.IdBase
                                 join u in _context.usuarios on ca.IdUsuarioV equals u.IdUsuario into usuarioJoin
                                 from u in usuarioJoin.DefaultIfEmpty()
                                 where ca.IdUsuarioS == usuarioId
                                        && ca.ClienteDesembolso != true
                                        && ca.ClienteRetirado != true
                                        && ca.FechaAsignacionSup.HasValue
                                        && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                        && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                        && db.TipoBase == ca.FuenteBase
                                 select new SupervisorDTO
                                 {
                                     IdAsignacion = ca.IdAsignacion,
                                     IdCliente = ca.IdCliente,
                                     idUsuarioV = ca.IdUsuarioV.HasValue ? ca.IdUsuarioV.Value : 0,
                                     FechaAsignacionV = ca.FechaAsignacionVendedor,

                                     Dni = bc.Dni,
                                     XAppaterno = bc.XAppaterno,
                                     XApmaterno = bc.XApmaterno,
                                     XNombre = bc.XNombre,

                                     NombresCompletos = u != null ? u.NombresCompletos : "Asesor no Asignado",
                                     DniVendedor = u != null ? u.Dni : " ",
                                 };
            var basesFiltradas = (from ca in _context.clientes_asignados 
                                    where ca.IdentificadorBase != null where ca.IdUsuarioS == usuarioId 
                                        && ca.FechaAsignacionSup.HasValue
                                        && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                        && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                    select new 
                                    {
                                        IdentificadorBase = ca.IdentificadorBase
                                    }).ToList().Distinct();

            if (supervisorData == null)
            {
                return NotFound("El presente Usuario Supervisor no tiene clientes Asignados");
            }
            // Contar los clientes pendientes (idUsuarioV es null)
            int clientesPendientesSupervisor = supervisorData.Count(cliente => cliente.idUsuarioV == 0);
            // Contar todos los clientes
            int totalClientes = supervisorData.Count();
            // Contar los clientes asignados (idUsuarioV no es null o 0)
            int clientesAsignadosSupervisor = supervisorData.Count(cliente => cliente.idUsuarioV != 0);

            var paginatedData = supervisorData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);
            // Filtrado de las bases
            ViewData["basesFiltradas"] = basesFiltradas;
            ViewData["UsuarioNombre"] = usuario != null ? usuario.NombresCompletos : "Usuario No Encontrado";
            ViewData["ClientesPendientesSupervisor"] = clientesPendientesSupervisor;
            ViewData["clientesAsignadosSupervisor"] = clientesAsignadosSupervisor;
            ViewData["totalClientes"] = totalClientes;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalClientes / (double)pageSize);

            return View("MainSupervisor", paginatedData);
        }

        [HttpGet]
        public IActionResult AsignarVendedorView()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var vendedoresConClientes = (from u in _context.usuarios
                                         where u.Rol == "VENDEDOR" && u.IDUSUARIOSUP == usuarioId
                                         join ca in _context.clientes_asignados
                                         on u.IdUsuario equals ca.IdUsuarioV into clientes
                                         from ca in clientes.DefaultIfEmpty() // Esta parte asegura que incluso si no hay clientes, el vendedor se incluya
                                         group ca by new
                                         {
                                             u.NombresCompletos,
                                             u.IdUsuario,
                                         } into grouped
                                         select new VendedorConClientesDTO
                                         {
                                             NombresCompletos = grouped.Key.NombresCompletos,
                                             IdUsuario = grouped.Key.IdUsuario,
                                             NumeroClientes = grouped.Count(c => c != null
                                                    && c.FechaAsignacionVendedor.HasValue
                                                    && c.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                                    && c.FechaAsignacionSup.Value.Month == DateTime.Now.Month) // Contamos solo los clientes asignados, ignorando los null
                                         }).ToList();

            var BasesAsignadas = (from ca in _context.clientes_asignados
                                  where ca.IdUsuarioS == usuarioId
                                          && ca.FechaAsignacionSup.HasValue
                                          && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                          && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                  select new { ca.FuenteBase })
                                    .Distinct()
                                    .ToList();
            if (vendedoresConClientes == null)
            {
                TempData["MessageError"] = "La consulta para dar asesores ha fallado";
                return RedirectToAction("VistaMainSupervisor", "Supervisor");
            }
            ViewData["BasesAsignadas"] = BasesAsignadas;
            return PartialView("_Asignarvendedores", vendedoresConClientes);
        }

        [HttpPost]
        public IActionResult AsignarVendedoresPorNumero(int nclientes, int id_vendedor)
        {
            if (!int.TryParse(nclientes.ToString(), out int n_clientes) || nclientes <= 0)
            {
                TempData["MessageError"] = "La entrada debe de ser un numero valido y positivo.";
                return RedirectToAction("VistaMainSupervisor");
            }

            var idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId").Value;
            var clientesDisponibles = _context.clientes_asignados
                                            .Where(ca => ca.IdUsuarioS == idSupervisorActual && ca.IdUsuarioV == null && ca.FechaAsignacionSup.HasValue && ca.FechaAsignacionSup.Value.Year == 2025
                                            && ca.FechaAsignacionSup.Value.Month == 1)
                                            .Take(nclientes)
                                            .ToList();
            Console.WriteLine($"Solo hay {clientesDisponibles.Count} clientes disponibles para asignar.");

            if (clientesDisponibles.Count < nclientes)
            {
                TempData["MessageError"] = $"Solo hay {clientesDisponibles.Count} clientes disponibles para asignar.";
                return RedirectToAction("VistaMainSupervisor");
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
            return RedirectToAction("VistaMainSupervisor");
        }

        [HttpGet]
        public IActionResult ModificarAsignacionVendedorView(int id_asignacion)
        {
            var idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId").Value;

            var vendedoresAsignados = (from u in _context.usuarios
                                       where u.IDUSUARIOSUP == idSupervisorActual && u.Rol == "VENDEDOR"
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
                return RedirectToAction("VistaMainSupervisor", "Supervisor");
            }

            TempData["idAsignacion"] = id_asignacion;
            return PartialView("_ModificarAsignacion", vendedoresAsignados);
        }
        [HttpGet]
        public IActionResult ModificarAsignacionVendedor(int idasignado, int idVendedor)
        {
            var asignacion = _context.clientes_asignados.FirstOrDefault(c => c.IdAsignacion == idasignado);

            if (asignacion.IdUsuarioV == idVendedor)
            {
                TempData["MessageError"] = "Debe seleccionar un Asesor diferente, ya que el Asesor actual es el mismo.";
                return RedirectToAction("VistaMainSupervisor");
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

            return RedirectToAction("VistaMainSupervisor");
        }

        /*[HttpPost]
        public IActionResult GuardarAsesoresAsignados(List<AsignarAsesorDTO> asignacionasesor, string selectAsesorBase)
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");

            string mensajesError = " ";
            if (asignacionasesor == null)
            {
                TempData["MessageError"] = "No se han enviado datos para asignar asesores.";
                return RedirectToAction("VistaMainSupervisor");
            }

            if (string.IsNullOrEmpty(selectAsesorBase))
            {
                TempData["MessageError"] = "Debe seleccionar una Fuente Base.";
                return RedirectToAction("VistaMainSupervisor");
            }

            // Comprobación adicional para verificar si todas las entradas tienen NumClientes igual a 0
            if (asignacionasesor.All(a => a.NumClientes == 0))
            {
                TempData["MessageError"] = "No se ha llenado ninguna entrada. Los campos no pueden estar vacíos.";
                return RedirectToAction("VistaMainSupervisor");
            }
            foreach (var asignacion in asignacionasesor)
            {
                Console.WriteLine($"IdVendedor: {asignacion.IdVendedor}, NumClientes: {asignacion.NumClientes}");
                if (asignacion.NumClientes == 0)
                {
                    continue;
                }

                int nClientes = asignacion.NumClientes;
                var clientesDisponibles = _context.clientes_asignados
                                        .Where(ca => ca.IdUsuarioS == idSupervisorActual && ca.IdUsuarioV == null
                                                && ca.FechaAsignacionSup.HasValue && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                                && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                                && ca.FuenteBase == selectAsesorBase)
                                        .Take(nClientes)
                                        .ToList();

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
                TempData["MessageError"] = mensajesError;
            }
            else
            {
                TempData["Message"] = "Las Asignaciones se culminaron con exito";
            }
            return RedirectToAction("VistaMainSupervisor");
        }*/

        [HttpGet]
        public IActionResult ModificarAsesoresView()
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                TempData["MessageError"] = "Error en la autenticacion. Intente iniciar sesion nuevamente.";
                return RedirectToAction("Index", "Home");
            }

            var asesoresAsignadosaSupervisor = (from u in _context.usuarios
                                                where u.Rol == "VENDEDOR" && u.IDUSUARIOSUP == idSupervisorActual
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
                                                                                        && g.ca.IdUsuarioS == idSupervisorActual), // Clientes asignados
                                                    ClientesTrabajando = grouped.Count(g => g.ca != null
                                                                                        && g.ca.TipificacionMayorPeso != null
                                                                                        && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                        && g.ca.IdUsuarioS == idSupervisorActual), // Clientes trabajados
                                                    ClientesSinTrabajar = grouped.Count(g => g.ca != null
                                                                                        && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                        && g.ca.IdUsuarioS == idSupervisorActual)
                                                                                         - grouped.Count(g => g.ca != null
                                                                                        && g.ca.TipificacionMayorPeso != null
                                                                                        && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                        && g.ca.IdUsuarioS == idSupervisorActual) // Diferencia entre asignados y trabajados
                                                }).ToList();
            return PartialView("_ModificarAsesores", asesoresAsignadosaSupervisor);
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
                                      ClientesTrabajando = _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario && ca.TipificacionMayorPeso != null),
                                      ClientesSinTrabajar = _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario) - _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario && ca.TipificacionMayorPeso != null)
                                  }).FirstOrDefault();
            Console.WriteLine($"El Asesor {asesorBusqueda.NombresCompletos} ha sido encontrado");
            if (asesorBusqueda == null)
            {
                Console.WriteLine("El Asesor no ha sido encontrado");
                return NotFound("El Asesor no ha sido encontrado");
            }
            Console.WriteLine("Retornando la vista parcial");
            return PartialView("_InterfazAsesor", asesorBusqueda); // Retorna una vista parcial
        }
        [HttpGet]
        public IActionResult AgregarNuevoAsesorView()
        {
            return PartialView("_AgregarNuevoAsesor");
        }

        [HttpGet]
        public IActionResult ModificarAsignacionPorTipificacionView()
        {
            try
            {
                int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");

                var tipificacionesGenerales = (from t in _context.tipificaciones
                                               select t).ToList();

                Console.WriteLine("Retornando la vista parcial");
                ViewData["tipificacionesGenerales"] = tipificacionesGenerales;
                return PartialView("_ModificarPorTipificacion");
            }
            catch (System.Exception)
            {
                return Json(new { error = true, message = "Ha ocurrido un Error al mandar los Datos" });
                throw;
            }
        }
        [HttpGet]
        public IActionResult ObtenerClientesPorTipificacion(string tipificacion)
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");

            // Consulta para las tipificaciones generales
            var clientesNumDB = (from ca in _context.clientes_asignados
                                 join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                 where ca.IdUsuarioS == idSupervisorActual
                                    && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                    && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                 select new
                                 {
                                     IdCliente = ce.IdCliente,
                                     TipificacionMayorPeso = ca.TipificacionMayorPeso
                                 }).ToList();

            // Filtrar por la tipificación más relevante
            var clientesFiltrados = clientesNumDB
                                    .Where(cndb => cndb.TipificacionMayorPeso == tipificacion)
                                    .ToList();


            var viewModel = new ResultadoTipificacionViewModelDTO
            {
                DetalleTipificacion = tipificacion,
                NumeroClientes = clientesFiltrados.Count
            };

            var AsesoresDelSupervisor = (from u in _context.usuarios
                                         where u.Rol == "VENDEDOR" && u.IDUSUARIOSUP == idSupervisorActual
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
                                                                                     && g.ca.IdUsuarioS == idSupervisorActual)
                                         }).ToList();

            ViewData["AsesoresDelSupervisor"] = AsesoresDelSupervisor;
            return PartialView("_ResultadoTipificacion", viewModel);
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
                return Json(new {error = true, message = ex.Message});  // Devuelve el error
                throw;
            }
        }
    }
}