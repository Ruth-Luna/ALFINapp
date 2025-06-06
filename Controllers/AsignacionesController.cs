using ALFINapp.API.Filters;
using ALFINapp.Infrastructure.Services;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using ALFINapp.API.DTOs;
using ALFINapp.Application.Interfaces.Asignaciones;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class AsignacionesController : Controller
    {
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesConsultasSupervisores _dbServicesConsultasSupervisores;
        private readonly IUseCaseCrossAssignments _useCaseCrossAssignments;
        private readonly MDbContext _context;
        private readonly ILogger<AsignacionesController> _logger;
        public AsignacionesController(DBServicesGeneral dbServicesGeneral,
            MDbContext context,
            DBServicesConsultasSupervisores dbServicesConsultasSupervisores,
            IUseCaseCrossAssignments useCaseCrossAssignments,
            ILogger<AsignacionesController> logger)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _context = context;
            _dbServicesConsultasSupervisores = dbServicesConsultasSupervisores;
            _useCaseCrossAssignments = useCaseCrossAssignments;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult CargarActualizarAsignacion(int idUsuario)
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
                                      TotalClientesAsignados = _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario
                                                                && ca.FechaAsignacionVendedor.HasValue
                                                                && ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                && ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month
                                                                ),
                                      ClientesTrabajando = _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario
                                                                && ca.TipificacionMayorPeso != null
                                                                && ca.FechaAsignacionVendedor.HasValue
                                                                && ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                && ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month
                                                                ),
                                      ClientesSinTrabajar = _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario
                                                                && ca.FechaAsignacionVendedor.HasValue
                                                                && ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                && ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month
                                                                ) -
                                                            _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario
                                                                && ca.TipificacionMayorPeso != null
                                                                && ca.FechaAsignacionVendedor.HasValue
                                                                && ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                && ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month
                                                                )
                                  }).FirstOrDefault();
            if (asesorBusqueda == null)
            {
                TempData["MessageError"] = "El Asesor no ha sido encontrado";
                return RedirectToAction("Index", "Home");
            }
            Console.WriteLine($"El Asesor {asesorBusqueda.NombresCompletos} ha sido encontrado");
            if (asesorBusqueda == null)
            {
                TempData["MessageError"] = "La entrada no ha sido ocurrido ha ocurrido un error";
                return RedirectToAction("Index", "Home");
            }
            return PartialView("ActualizarAsignacion", asesorBusqueda); // Retorna una vista parcial
        }
        [HttpGet]
        [PermissionAuthorization("Asignaciones", "Modificar")]
        public async Task<IActionResult> Modificar()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            var GetVendedoresAsignados = await _dbServicesConsultasSupervisores.GetAsesorsFromSupervisor(usuarioId.Value);
            if (GetVendedoresAsignados.IsSuccess == false || GetVendedoresAsignados.Data == null)
            {
                TempData["MessageError"] = GetVendedoresAsignados.Message;
                return RedirectToAction("Index", "Home");
            }
            var vendedoresConClientes = new List<VendedorConClientesDTO>();
            foreach (var vendedorIndividual in GetVendedoresAsignados.Data)
            {
                var vendedorIndividualMapeado = await _dbServicesConsultasSupervisores.GetNumberTipificacionesPlotedOnDTO(vendedorIndividual, usuarioId.Value);
                if (vendedorIndividualMapeado.IsSuccess == false || vendedorIndividualMapeado.Data == null)
                {
                    TempData["MessageError"] = GetVendedoresAsignados.Message;
                    return RedirectToAction("Index", "Home");
                }
                vendedoresConClientes.Add(vendedorIndividualMapeado.Data);
            }
            if (vendedoresConClientes == null)
            {
                TempData["MessageError"] = GetVendedoresAsignados.Message;
                return RedirectToAction("Index", "Home");
            }
            return View("Modificar", vendedoresConClientes);
        }

        [HttpGet]
        [PermissionAuthorization("Asignaciones", "Tipificar")]
        public IActionResult Tipificar()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            return View("Tipificar");
        }
        [HttpGet]
        public async Task<IActionResult> CrossAssignments([FromBody] List<DtoVAsignarClientesSupervisores> asignaciones)
        {
            try
            {
                var result = await _useCaseCrossAssignments.Execute(new Application.DTOs.DetallesAssignmentsMasive(asignaciones));
                if (result.IsSuccess)
                {
                    return Json(new
                    {
                        Message = result.Message,
                        Data = result.Data
                    });
                }
                else
                {
                    _logger.LogError("Error al procesar las asignaciones: {Message}", result.Message);
                    // Aquí puedes registrar el error en un sistema de logging o manejarlo de otra manera
                    // Retorna un mensaje de error genérico
                    return Json(new
                    {
                        IsSuccess = false,
                        Message = result.Message,
                    });
                }
            }
            catch (System.Exception)
            {
                _logger.LogError("Error al procesar las asignaciones");
                return Json(new
                {
                    IsSuccess = false,
                    Message = "Ha ocurrido un error al procesar las asignaciones. Por favor, inténtelo de nuevo más tarde."
                });
            }
        }
    }
}