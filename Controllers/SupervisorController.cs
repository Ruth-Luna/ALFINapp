using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly MDbContext _context;
        public SupervisorController(MDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> VistaMainSupervisor(int page = 1, int pageSize = 10)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["Message"] = "Ha ocurrido un error en la autenticacion";
                return RedirectToAction("Index", "Home");
            }

            var supervisorData = from ca in _context.clientes_asignados
                                 join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                 join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                                 join u in _context.usuarios on ca.IdUsuarioV equals u.IdUsuario into usuarioJoin
                                 from u in usuarioJoin.DefaultIfEmpty()
                                 where ca.IdUsuarioS == usuarioId
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

                                     NombresCompletos = u != null ? u.NombresCompletos : "Vendedor no Asignado",
                                     ApellidoPaterno = u != null ? u.ApellidoPaterno : "No disponible", // Manejo de null para ApellidoPaterno
                                     DniVendedor = u != null ? u.Dni : " ",
                                 };
            if (supervisorData == null)
            {
                return NotFound("El presente Usuario Supervisor no tiene clientes Asignados");
            }
            // Contar los clientes pendientes (idUsuarioV es null)
            int clientesPendientesSupervisor = supervisorData.Count(cliente => cliente.idUsuarioV == 0);

            var totalClientes = supervisorData.Count();
            var paginatedData = supervisorData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);
            ViewData["UsuarioNombre"] = usuario != null ? usuario.NombresCompletos : "Usuario No Encontrado";
            ViewData["ClientesPendientesSupervisor"] = clientesPendientesSupervisor;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalClientes / (double)pageSize);

            return View("MainSupervisor", paginatedData);
        }

        [HttpGet]
        public IActionResult AsignarVendedorView()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["Message"] = "No ha iniciado sesion";
                return RedirectToAction("VistaMainSupervisor", "Supervisor");
            }
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            Console.WriteLine("HEmos pasado la Comprobacion");
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
                                             NumeroClientes = grouped.Count(c => c != null) // Contamos solo los clientes asignados, ignorando los null
                                         }).ToList();
            if (vendedoresConClientes == null)
            {
                TempData["Message"] = "La consulta para dar vendedores ha fallado";
                return RedirectToAction("VistaMainSupervisor", "Supervisor");
            }
            return PartialView("_Asignarvendedores", vendedoresConClientes);
        }

        [HttpPost]
        public IActionResult AsignarVendedoresPorNumero(int nclientes, int id_vendedor)
        {
            Console.WriteLine($"IMPRIMIENTO LEER IMPORTANTE");
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["Message"] = "No ha iniciado sesión.";
                return RedirectToAction("Index", "Home");
            }

            if (!int.TryParse(nclientes.ToString(), out int n_clientes) || nclientes <= 0)
            {
                TempData["Message"] = "La entrada debe de ser un numero valido y positivo.";
                return RedirectToAction("VistaMainSupervisor");
            }

            var idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId").Value;
            var clientesDisponibles = _context.clientes_asignados
                                            .Where(ca => ca.IdUsuarioS == idSupervisorActual && ca.IdUsuarioV == null)
                                            .Take(nclientes)
                                            .ToList();
            Console.WriteLine($"Solo hay {clientesDisponibles.Count} clientes disponibles para asignar.");

            if (clientesDisponibles.Count < nclientes)
            {
                TempData["Message"] = $"Solo hay {clientesDisponibles.Count} clientes disponibles para asignar.";
                return RedirectToAction("VistaMainSupervisor");
            }

            foreach (var cliente in clientesDisponibles)
            {
                Console.WriteLine($"Estamo asignando el cliente {cliente.IdCliente}");
                cliente.IdUsuarioV = id_vendedor;
                cliente.FechaAsignacionVendedor = DateTime.Now;
            }

            // Guardar los cambios en la base de datos
            _context.SaveChanges();
            TempData["Message"] = $"{nclientes} clientes han sido asignados correctamente al vendedor.";
            return RedirectToAction("VistaMainSupervisor");
        }
        [HttpGet]
        public IActionResult ModificarAsignacionVendedorView(int id_asignacion)
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["Message"] = "No ha iniciado sesión.";
                return RedirectToAction("Index", "Home");
            }
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
                TempData["Message"] = "La consulta para dar vendedores ha fallado";
                return RedirectToAction("VistaMainSupervisor", "Supervisor");
            }
            
            TempData["idAsignacion"] = id_asignacion;
            return PartialView("_ModificarAsignacion", vendedoresAsignados);
        }
        [HttpGet]
        public IActionResult ModificarAsignacionVendedor (int idasignado, int idVendedor)
        {
            var asignacion = _context.clientes_asignados.FirstOrDefault(c => c.IdAsignacion == idasignado);

            if (asignacion.IdUsuarioV == idVendedor)
            {
                TempData["Message"] = "Debe seleccionar un Vendedor diferente";
                return RedirectToAction("VistaMainSupervisor");
            }
            if (asignacion != null)
            {
                asignacion.IdUsuarioV = idVendedor;
                asignacion.FechaAsignacionVendedor = DateTime.Now;
                _context.SaveChanges();

                TempData["Message"] = $"{asignacion.IdCliente} ha sido asignado al vendedor {idVendedor}.";
            }

            else
            {
                TempData["Message"] = "El id de Asignacion mandado es incorrecto";
            }
            
            return RedirectToAction("VistaMainSupervisor");
        }

        [HttpPost]
        public IActionResult GuardarAsesoresAsignados (List<AsignarAsesorDTO> asignacionasesor)
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                TempData["Message"] = "Error en la autenticación. Intente iniciar sesión nuevamente.";
                return RedirectToAction("Index", "Home");
            }

            
            var mensajes = new List<string>();

            foreach (var asignacion in asignacionasesor)
            {
                Console.WriteLine($"GuardarAsesoresAsignados*************************");
                Console.WriteLine($"IdVendedor: {asignacion.IdVendedor}, NumClientes: {asignacion.NumClientes}");
                if (asignacion.NumClientes == 0)
                {
                    mensajes.Add($"No se asignaron clientes al vendedor {asignacion.IdVendedor} porque el número de clientes es 0.");
                    continue;
                }

                int Contador = 0;
                int nClientes = asignacion.NumClientes;
                var clientesDisponibles = _context.clientes_asignados
                                        .Where(ca => ca.IdUsuarioS == idSupervisorActual && ca.IdUsuarioV == null)
                                        .Take(nClientes)
                                        .ToList();

                if (clientesDisponibles.Count < nClientes)
                {
                    mensajes.Add($"Solo hay {clientesDisponibles.Count} clientes disponibles para asignar al vendedor {asignacion.IdVendedor}. La asignación fue pausada.");
                    break;
                }

                foreach (var cliente in clientesDisponibles)
                {
                    cliente.IdUsuarioV = asignacion.IdVendedor;
                    cliente.FechaAsignacionVendedor = DateTime.Now;
                }
                _context.SaveChanges();
                mensajes.Add($"{nClientes} clientes fueron asignados correctamente al vendedor {asignacion.IdVendedor}.");
            }
            TempData["Message"] = "Las Siguientes Asignaciones fueron Hechas:" + string.Join(" ", mensajes);
            return RedirectToAction("VistaMainSupervisor");
        }
    }
}