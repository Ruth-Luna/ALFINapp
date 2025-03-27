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
        private readonly IUseCaseGetReportesAsesor _useCaseGetReportesAsesor;
        private readonly IUseCaseGetReportesSupervisor _useCaseGetReportesSupervisor;
        public ReportesController(
            IUseCaseGetReportesAdministrador useCaseGetReportesAdministrador,
            IUseCaseGetReportesAsesor useCaseGetReportesAsesor,
            IUseCaseGetReportesSupervisor useCaseGetReportesSupervisor)
        {
            _useCaseGetReportesAdministrador = useCaseGetReportesAdministrador;
            _useCaseGetReportesAsesor = useCaseGetReportesAsesor;
            _useCaseGetReportesSupervisor = useCaseGetReportesSupervisor;
        }
        [HttpGet]
        [PermissionAuthorization("Reportes", "Reportes")]
        public async Task<IActionResult> Reportes()
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                TempData["MessageError"] = "Rol no valido.";
                return RedirectToAction("Index", "Home");
            }
            ViewData["RolUser"] = rol.Value;
            var idUsuario = HttpContext.Session.GetInt32("IdUser");
            if (idUsuario == null)
            {
                TempData["MessageError"] = "Id de usuario no valido.";
                return RedirectToAction("Index", "Home");
            }
            if (rol == 1 || rol == 4)
            {
                var reportesAdministrador = await _useCaseGetReportesAdministrador.Execute();
                if (!reportesAdministrador.IsSuccess)
                {
                    TempData["MessageError"] = reportesAdministrador.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                return View("Reportes", reportesAdministrador.Data);
            }
            else if (rol == 2)
            {
                var reportesSupervisor = await _useCaseGetReportesSupervisor.Execute(idUsuario.Value);
                if (!reportesSupervisor.IsSuccess)
                {
                    TempData["MessageError"] = reportesSupervisor.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                return View("Reportes", reportesSupervisor.Data);
            }
            else if (rol == 3)
            {
                var reportesAsesor = await _useCaseGetReportesAsesor.Execute(idUsuario.Value);
                if (!reportesAsesor.IsSuccess)
                {
                    TempData["MessageError"] = reportesAsesor.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                return View("Reportes", reportesAsesor.Data);
            }
            else
            {
                TempData["MessageError"] = "Rol no valido.";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public async Task<IActionResult> AsesorReportes(int idAsesor)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var reportesAdministrador = await _useCaseGetReportesAsesor.Execute(idAsesor);
            if (!reportesAdministrador.IsSuccess)
            {
                return Json(new { success = false, message = reportesAdministrador.Message });
            }
            return PartialView("_ReportesAsesor", reportesAdministrador.Data);
        }
        [HttpGet]
        public async Task<IActionResult> SupervisorReportes(int idSupervisor)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var reportesAdministrador = await _useCaseGetReportesSupervisor.Execute(idSupervisor);
            if (!reportesAdministrador.IsSuccess)
            {
                return Json(new { success = false, message = reportesAdministrador.Message });
            }
            return PartialView("_ReportesSupervisor", reportesAdministrador.Data);
        }
    }
}