using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using ALFINapp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ALFINapp.Services;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class ReagregacionesController : Controller
    {
        private readonly MDbContext _context;
        private readonly DBServicesAsignacionesAsesores _dbServicesAsignacionesAsores;

        public ReagregacionesController(MDbContext context, DBServicesAsignacionesAsesores dbServicesAsignacionesAsores)
        {
            _context = context;
            _dbServicesAsignacionesAsores = dbServicesAsignacionesAsores;
        }

        [HttpGet]
        public IActionResult VerificarDNI(string dni)
        {
            try
            {
                Console.WriteLine($"DNI recibido: {dni}");

                // Buscar el cliente por DNI
                var clienteExistente = _context.base_clientes.FirstOrDefault(c => c.Dni == dni);
                if (clienteExistente == null)
                {
                    return Json(new { existe = false, error = false, message = "El DNI no está registrado en la Base de Datos. No puede ser asignado a Usted." });
                }

                // Obtener detalles de la base asociados al cliente
                var detalleBaseClientes = _context.detalle_base
                                                  .Where(db => db.IdBase == clienteExistente.IdBase)
                                                  .ToList();

                if (!detalleBaseClientes.Any())
                {
                    // Cliente encontrado, pero sin detalles de campaña
                    Console.WriteLine($"Cliente encontrado pero sin detalle de campaña: {clienteExistente.XNombre} {clienteExistente.XAppaterno}");
                    return Json(new { existe = false, error = false, message = "El cliente no tiene detalles de campaña registrados en la Base de Datos. No puede ser asignado a Usted." });
                }

                // Buscar a sus asesores asignados
                var AsesoresGeneral = (from ca in _context.clientes_asignados
                                                join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                                join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                                                join u in _context.usuarios on ca.IdUsuarioV equals u.IdUsuario
                                                where clienteExistente.IdBase == bc.IdBase
                                                select new
                                                {
                                                    NombreAsesorPrimario = u.NombresCompletos,
                                                    IDAsesorPrimario = ca.IdUsuarioV,
                                                    IDAsignacion = ca.IdAsignacion,
                                                    IDCliente = ce.IdCliente
                                                }).ToList();
                // Pasar información a la vista
                ViewData["AsesoresGeneral"] = AsesoresGeneral;
                Console.WriteLine($"Cliente encontrado: {clienteExistente.XNombre} {clienteExistente.XAppaterno}");
                ViewData["DetalleGeneralCliente"] = clienteExistente;
                return PartialView("_DatosConsulta", detalleBaseClientes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar el DNI: {ex.Message}");
                return Json(new { existe = false, error = true, message = "Ocurrió un error interno. Por favor, intente nuevamente." });
            }
        }

        [HttpPost]
        public IActionResult ReAsignarClienteAUsuario(string DniAReasignar, string BaseTipo)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "Debe de iniciar la sesion." });
                }
                var baseClienteReasignar = _dbServicesAsignacionesAsores.GuardarReAsignacionCliente(DniAReasignar, BaseTipo, usuarioId.Value);

                if (baseClienteReasignar.Result.Item1 == false)
                {
                    return Json(new { success = false, message = $"{baseClienteReasignar.Result.Item2}" });
                }

                return Json(new { success = true, message = $"{baseClienteReasignar.Result.Item2}" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}