using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using ALFINapp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ALFINapp.Services;
using System.Threading.Tasks;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class ReagregacionesController : Controller
    {
        private readonly MDbContext _context;
        private readonly DBServicesAsignacionesAsesores _dbServicesAsignacionesAsores;
        private readonly DBServicesConsultasClientes _dbServicesConsultasClientes;

        public ReagregacionesController(MDbContext context, DBServicesAsignacionesAsesores dbServicesAsignacionesAsores, DBServicesConsultasClientes dbServicesConsultasClientes)
        {
            _context = context;
            _dbServicesAsignacionesAsores = dbServicesAsignacionesAsores;
            _dbServicesConsultasClientes = dbServicesConsultasClientes;
        }

        [HttpGet]
        public async Task<IActionResult> VerificarDNIenBDoBanco(string dni)
        {
            try
            {
                Console.WriteLine($"DNI recibido: {dni}");

                // Buscar el cliente por DNI
                var GetClienteExistente = await _dbServicesConsultasClientes.GetClientsFromDBandBank(dni);

                if (GetClienteExistente.IsSuccess == false || GetClienteExistente.Data == null)
                {
                    return Json(new { existe = false, error = true, message = GetClienteExistente.message });
                }

                if (GetClienteExistente.Data.TraidoDe == "BDA365")
                {
                    // Buscar a sus asesores asignados
                    var AsesoresGeneral = await (from ca in _context.clientes_asignados
                                                 join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                                 join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                                                 join u in _context.usuarios on ca.IdUsuarioV equals u.IdUsuario
                                                 where GetClienteExistente.Data.IdBase == ce.IdBase
                                                 select new AsesoresDeUnClienteDTO
                                                 {
                                                     NombreAsesorPrimario = u.NombresCompletos,
                                                     IDAsesorPrimario = ca.IdUsuarioV,
                                                     IDAsignacion = ca.IdAsignacion,
                                                     IDCliente = ce.IdCliente
                                                 }).ToListAsync();
                    ViewData["Asesores"] = AsesoresGeneral;

                    var detalleclienteExistenteBDA365 = GetClienteExistente.Data != null
                                ? await _context.base_clientes.FirstOrDefaultAsync(c => c.IdBase == GetClienteExistente.Data.IdBase)
                                : null;

                    ViewData["HayDetalleCliente"] = true;
                    ViewData["DetalleCliente"] = detalleclienteExistenteBDA365;
                    return PartialView("_DatosConsulta", GetClienteExistente.Data);
                }
                if (GetClienteExistente.Data.TraidoDe == "BDALFIN")
                {
                    var AsesoresGeneral = await (from ca in _context.clientes_asignados
                                                    join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                                    join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                                                    join u in _context.usuarios on ca.IdUsuarioV equals u.IdUsuario
                                                    where GetClienteExistente.Data.IdBase == ce.IdBaseBanco
                                                    select new AsesoresDeUnClienteDTO
                                                    {
                                                        NombreAsesorPrimario = u.NombresCompletos,
                                                        IDAsesorPrimario = ca.IdUsuarioV,
                                                        IDAsignacion = ca.IdAsignacion,
                                                        IDCliente = ce.IdCliente
                                                    }).ToListAsync();

                    ViewData["Asesores"] = AsesoresGeneral;
                    ViewData["HayDetalleCliente"] = false;
                    return PartialView("_DatosConsulta", GetClienteExistente.Data);
                }
                else
                {
                    return Json(new { existe = false, error = true, message = "La Base fue conseguida, pero no se le permite ver los datos de este cliente" });
                }
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