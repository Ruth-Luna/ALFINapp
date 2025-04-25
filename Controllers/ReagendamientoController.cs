using ALFINapp.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    public class ReagendamientoController : Controller
    {
        private readonly ILogger<ReagendamientoController> _logger;

        public ReagendamientoController(ILogger<ReagendamientoController> logger)
        {
            _logger = logger;
        }
        public async Task<IActionResult> Reagendar(
            DtoVReagendarClientes dtoReagendarClientes)
        {
            return Json(new { success = true, message = "Reagendamiento" });
        }

        public async Task<IActionResult> Reagendamiento(int id)
        {
            return PartialView("_Reagendamiento", id);
        }
    }
}