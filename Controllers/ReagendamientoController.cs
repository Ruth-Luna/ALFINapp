using ALFINapp.API.DTOs;
using ALFINapp.Application.Interfaces.Reagendamiento;
using ALFINapp.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    public class ReagendamientoController : Controller
    {
        private readonly ILogger<ReagendamientoController> _logger;
        private readonly IUseCaseGetReagendamiento _useCaseGetReagendamiento;
        private readonly IUseCaseReagendar _useCaseReagendar;
        public ReagendamientoController(
            ILogger<ReagendamientoController> logger,
            IUseCaseGetReagendamiento useCaseGetReagendamiento,
            IUseCaseReagendar useCaseReagendar)
        {
            _logger = logger;
            _useCaseGetReagendamiento = useCaseGetReagendamiento;
            _useCaseReagendar = useCaseReagendar;
        }
        public async Task<IActionResult> Reagendar(
            [FromBody]DtoVReagendar dtovreagendar)
        {
            if (dtovreagendar.FechaReagendamiento == null || dtovreagendar.FechaReagendamiento == DateTime.MinValue)
                {
                    return Json(new { success = false, message = "La fecha de reagendamiento es obligatoria." });
                }
            if (dtovreagendar.IdDerivacion == null || dtovreagendar.IdDerivacion == 0)
            {
                return Json(new { success = false, message = "El id de derivación es obligatorio." });
            }
            var exec = await _useCaseReagendar.exec(
                dtovreagendar.IdDerivacion.Value,
                dtovreagendar.FechaReagendamiento.Value,
                dtovreagendar.evidencias);
            if (!exec.IsSuccess)
            {
                return Json(new { success = false, message = exec.Message });
            }
            return Json(new { success = true, message = exec.Message });
        }

        public async Task<IActionResult> Reagendamiento(int id)
        {
            var exec = await _useCaseGetReagendamiento.exec(id);
            if (!exec.IsSuccess)
            {
                return Json(new { success = false, message = exec.Message });
            }
            return Json(new
            {
                success = true,
                message = exec.Message,
                data = exec.Data
            });
        }
    }
}