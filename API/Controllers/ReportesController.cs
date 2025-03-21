using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Filters;
using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class ReportesController : Controller
    {
        private readonly IUseCaseGetReportesAdministrador _useCaseGetReportesAdministrador;
        public ReportesController(
            IUseCaseGetReportesAdministrador useCaseGetReportesAdministrador
        )
        {
            _useCaseGetReportesAdministrador = useCaseGetReportesAdministrador;
        }
        [HttpGet]
        [PermissionAuthorization("Reportes", "Reportes")]
        public async Task<IActionResult> Reportes()
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var reportesAdministrador = await _useCaseGetReportesAdministrador.Execute();
            if (!reportesAdministrador.IsSuccess)
            {
                TempData["MessageError"] = reportesAdministrador.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
            if (rol == 1)
            {
                
            }
            else if (rol == 2)
            {
                
            }
            else if (rol == 3)
            {
                
            }
            var reportes = new ViewReportesGeneral { };
            return View("Reportes", reportesAdministrador.Data);
        }
    }
}