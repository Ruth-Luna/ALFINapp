using ALFINapp.API.Filters;
using Microsoft.AspNetCore.Mvc;
using ALFINapp.API.DTOs;
using ALFINapp.Datos.DAO.Administrador;
using ALFINapp.Models.DTOs;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class AsignacionesController : Controller
    {
        private readonly DAO_CruzarAsignaciones dAO_CruzarAsignaciones;
        private readonly DAO_AsignacionesASupervisores _daoAsignacionesASupervisores;
        private readonly DAO_GetAsignaciones _daoGetAsignaciones;
        private readonly MDbContext _context;
        private readonly ILogger<AsignacionesController> _logger;
        public AsignacionesController(
            MDbContext context,
            ILogger<AsignacionesController> logger,
            DAO_CruzarAsignaciones dAO_CruzarAsignaciones,
            DAO_AsignacionesASupervisores daoAsignacionesASupervisores,
            DAO_GetAsignaciones daoGetAsignaciones)
        {
            _context = context;
            _logger = logger;
            this.dAO_CruzarAsignaciones = dAO_CruzarAsignaciones;
            this._daoAsignacionesASupervisores = daoAsignacionesASupervisores;
            this._daoGetAsignaciones = daoGetAsignaciones;
        }
        [HttpPost]
        public async Task<IActionResult> CrossAssignments([FromBody] List<DtoVAsignarClientesSupervisores> asignaciones)
        {
            try
            {
                var asignacionesMasivas = new DetallesAssignmentsMasive(asignaciones);
                var result = await dAO_CruzarAsignaciones.cruzar(asignacionesMasivas);
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
                var data = await dAO_CruzarAsignaciones.GetCrossed(pagina);
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
                var asignacionesMasivas = new DetallesAssignmentsMasive(asignaciones);
                var result = await _daoAsignacionesASupervisores.AsignarMasivoAsync(asignacionesMasivas);
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
                var asignaciones = await _daoGetAsignaciones.GetAllAssignmentsFromSupervisor();
                if (!asignaciones.IsSuccess)
                {
                    _logger.LogError("Error al obtener asignaciones: {Message}", asignaciones.Message);
                    return Json(new
                    {
                        isSuccess = false,
                        message = asignaciones.Message,
                    });
                }
                return Json(new
                {
                    isSuccess = true,
                    message = "Asignaciones recuperadas correctamente",
                    data = asignaciones.asignaciones
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