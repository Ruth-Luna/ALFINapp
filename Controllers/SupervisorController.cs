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
        public async Task<IActionResult> VistaMainSupervisor()
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
                                 join u in _context.usuarios on ca.IdUsuarioV equals u.IdUsuario
                                 where ca.IdUsuarioS == usuarioId
                                 select new SupervisorDTO
                                 {
                                     // Propiedades de la tabla clientes_asignados
                                     IdAsignacion = ca.IdAsignacion,
                                     IdCliente = ca.IdCliente,
                                     idUsuarioV = ca.IdUsuarioV,
                                     FechaAsignacionV = ca.FechaAsignacionVendedor,

                                     // Propiedades de la tabla base_clientes
                                     Dni = bc.Dni,
                                     XAppaterno = bc.XAppaterno,
                                     XApmaterno = bc.XApmaterno,
                                     XNombre = bc.XNombre,

                                     // Propiedades de la tabla usuarios
                                     NombresCompletos = u.NombresCompletos,
                                     ApellidoPaterno = u.ApellidoPaterno
                                 };
            if (supervisorData == null)
            {
                return NotFound("El presente Usuario Supervisor no tiene clientes Asignados");
            }

            var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);
            ViewData["UsuarioNombre"] = usuario != null ? usuario.NombresCompletos : "Usuario No Encontrado";
            return View("MainSupervisor", supervisorData);
        }

        [HttpGet]
        public IActionResult AsignarVendedorView()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["Message"] = "No ha iniciado sesion";
                return RedirectToAction("VistaMainSupervisor", "Supervisor");
            }
            Console.WriteLine("HEmos pasado la Comprobacion");
            var vendedoresConClientes = (from u in _context.usuarios
                                        where u.Rol == "VENDEDOR"
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
            int idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId").Value;
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
    }
}