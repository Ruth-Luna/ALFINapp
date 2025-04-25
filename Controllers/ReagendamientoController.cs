using ALFINapp.API.DTOs;
using ALFINapp.Application.Interfaces.Reagendamiento;
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
            DateTime FechaReagendamiento,
            int IdDerivacion)
        {
            var exec = await _useCaseReagendar.exec(
                IdDerivacion,
                FechaReagendamiento);
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
                TempData["MessageError"] = exec.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
            return PartialView("_Reagendamiento", exec.Data);
        }
    }
}