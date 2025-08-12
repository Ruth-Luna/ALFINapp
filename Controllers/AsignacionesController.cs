using ALFINapp.API.Filters;
using Microsoft.AspNetCore.Mvc;
using ALFINapp.API.DTOs;
using ALFINapp.Application.Interfaces.Asignaciones;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class AsignacionesController : Controller
    {
        private readonly IUseCaseCrossAssignments _useCaseCrossAssignments;
        private readonly IUseCaseAsignarClientesSup _useCaseAsignarClientesSup;
        private readonly IUseCaseGetAsignacionesDelSup _useCaseGetAsignacionesDelSup;
        private readonly IUseCaseDownloadAsignaciones useCaseDownloadAsignaciones;
        private readonly MDbContext _context;
        private readonly ILogger<AsignacionesController> _logger;
        public AsignacionesController(
            MDbContext context,
            IUseCaseCrossAssignments useCaseCrossAssignments,
            ILogger<AsignacionesController> logger,
            IUseCaseAsignarClientesSup useCaseAsignarClientesSup,
            IUseCaseGetAsignacionesDelSup useCaseGetAsignacionesDelSup,
            IUseCaseDownloadAsignaciones useCaseDownloadAsignaciones)
        {
            _context = context;
            _useCaseCrossAssignments = useCaseCrossAssignments;
            _logger = logger;
            _useCaseAsignarClientesSup = useCaseAsignarClientesSup;
            _useCaseGetAsignacionesDelSup = useCaseGetAsignacionesDelSup;
            this.useCaseDownloadAsignaciones = useCaseDownloadAsignaciones;
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