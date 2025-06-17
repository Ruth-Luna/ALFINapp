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
        private readonly IUseCaseAsignarClientesSup _useCaseAsignarClientesSup;
        private readonly IUseCaseGetAsignacionesDelSup _useCaseGetAsignacionesDelSup;
        private readonly IUseCaseDownloadAsignaciones useCaseDownloadAsignaciones;
        private readonly MDbContext _context;
        private readonly ILogger<AsignacionesController> _logger;
        public AsignacionesController(DBServicesGeneral dbServicesGeneral,
            MDbContext context,
            DBServicesConsultasSupervisores dbServicesConsultasSupervisores,
            IUseCaseCrossAssignments useCaseCrossAssignments,
            ILogger<AsignacionesController> logger,
            IUseCaseAsignarClientesSup useCaseAsignarClientesSup,
            IUseCaseGetAsignacionesDelSup useCaseGetAsignacionesDelSup,
            IUseCaseDownloadAsignaciones useCaseDownloadAsignaciones)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _context = context;
            _dbServicesConsultasSupervisores = dbServicesConsultasSupervisores;
            _useCaseCrossAssignments = useCaseCrossAssignments;
            _logger = logger;
            _useCaseAsignarClientesSup = useCaseAsignarClientesSup;
            _useCaseGetAsignacionesDelSup = useCaseGetAsignacionesDelSup;
            this.useCaseDownloadAsignaciones = useCaseDownloadAsignaciones;
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
        [HttpPost]
        public async Task<IActionResult> CrossAssignments([FromBody] List<DtoVAsignarClientesSupervisores> asignaciones)
        {
            try
            {
                var result = await _useCaseCrossAssignments.Execute(new Application.DTOs.DetallesAssignmentsMasive(asignaciones));
                if (result.IsSuccess)
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Message = result.Message,
                        Data = result.ClientesCruzados != null ? result.ClientesCruzados : null,
                    });
                }
                else
                {
                    _logger.LogError("Error al procesar las asignaciones: {Message}", result.Message);
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

        [HttpGet]
        public async Task<IActionResult> ObtenerCruceFinal(int pagina = 1)
        {
            try
            {
                var data = await _useCaseCrossAssignments.GetCrossed(pagina);
                return Json(new
                {
                    IsSuccess = true,
                    Message = "Cruce recuperado correctamente",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener cruce: {Message}", ex.Message);
                return Json(new
                {
                    IsSuccess = false,
                    Message = "Error al obtener el cruce final"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignBaseSupervisors([FromBody] List<DtoVAsignarClientesSupervisores> asignaciones)
        {
            if (asignaciones == null || asignaciones.Count == 0)
            {
                TempData["MessageError"] = "No se proporcionaron asignaciones para procesar.";
                return RedirectToAction("Modificar");
            }

            try
            {
                var result = await _useCaseAsignarClientesSup.AsignarMasivoAsync(new Application.DTOs.DetallesAssignmentsMasive(asignaciones));
                if (result.IsSuccess)
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Message = result.Message
                    });
                }
                else
                {
                    _logger.LogError("Error al asignar clientes: {Message}", result.Message);
                    return Json(new
                    {
                        IsSuccess = false,
                        Message = result.Message
                    });
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error al asignar clientes: {Message}", ex.Message);
                return Json(new
                {
                    IsSuccess = false,
                    Message = "Ha ocurrido un error al asignar los clientes. Por favor, inténtelo de nuevo más tarde."
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAsignaciones()
        {
            try
            {
                var asignaciones = await _useCaseGetAsignacionesDelSup.exec();
                if (!asignaciones.success)
                {
                    _logger.LogError("Error al obtener asignaciones: {Message}", asignaciones.message);
                    return Json(new
                    {
                        isSuccess = false,
                        message = asignaciones.message
                    });
                }
                return Json(new
                {
                    isSuccess = true,
                    message = "Asignaciones recuperadas correctamente",
                    data = asignaciones.data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener asignaciones: {Message}", ex.Message);
                return Json(new
                {
                    isSuccess = false,
                    message = "Error al obtener las asignaciones"
                });
            }
        }
        
    }
}