using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    [Route("[controller]")]
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
    }
}