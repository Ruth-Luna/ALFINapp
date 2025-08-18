using ALFINapp.API.Filters;
using ALFINapp.Datos.DAO.Reportes;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class ReportesController : Controller
    {
        private readonly DAO_GetReportes _daoGetReportes;
        private readonly DAO_ReportesAsesor _daoReportesAsesor;
        private readonly DAO_ReportesSupervisor _daoReportesSupervisor;
        private readonly DAO_ReportesMetas _daoReportesMetas;
        private readonly DAO_ReportesFechas _daoReportesFechas;
        public ReportesController(
            DAO_GetReportes daoGetReportes,
            DAO_ReportesAsesor daoReportesAsesor,
            DAO_ReportesSupervisor daoReportesSupervisor,
            DAO_ReportesMetas daoReportesMetas,
            DAO_ReportesFechas daoReportesFechas)
        {
            _daoGetReportes = daoGetReportes;
            _daoReportesAsesor = daoReportesAsesor;
            _daoReportesSupervisor = daoReportesSupervisor;
            _daoReportesMetas = daoReportesMetas;
            _daoReportesFechas = daoReportesFechas;
        }
        [HttpGet]
        [PermissionAuthorization("Reportes", "Reportes")]
        public async Task<IActionResult> Reportes(int? anio = null, int? mes = null)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                TempData["MessageError"] = "Rol no valido.";
                return RedirectToAction("Index", "Home");
            }
            ViewData["RolUser"] = rol.Value;
            var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            if (idUsuario == null)
            {
                TempData["MessageError"] = "Id de usuario no valido.";
                return RedirectToAction("Index", "Home");
            }
            ViewData["UsuarioId"] = idUsuario.Value;
            var reportesAdministrador = await _daoGetReportes.getAllReportes(
                idUsuario.Value,
                anio ?? DateTime.Now.Year,
                mes ?? DateTime.Now.Month);
            if (!reportesAdministrador.IsSuccess)
            {
                TempData["MessageError"] = reportesAdministrador.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
            return View("Reportes", reportesAdministrador.Data);
        }
        
        [HttpGet]
        public async Task<IActionResult> AsesorReportes(
            int idAsesor,
            int? anio = null,
            int? mes = null)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var reportesAdministrador = await _daoReportesAsesor.getAllReportes(
                idAsesor,
                anio ?? DateTime.Now.Year,
                mes ?? DateTime.Now.Month);
            if (!reportesAdministrador.IsSuccess)
            {
                return Json(new { success = false, message = reportesAdministrador.Message });
            }
            return PartialView("_ReportesAsesor", reportesAdministrador.Data);
        }
        [HttpGet]
        public async Task<IActionResult> SupervisorReportes(int idSupervisor, int? anio = null, int? mes = null)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var reportesAdministrador = await _daoReportesSupervisor.getAllReportes(idSupervisor, anio, mes);
            if (!reportesAdministrador.IsSuccess)
            {
                return Json(new { success = false, message = reportesAdministrador.Message });
            }
            return PartialView("_ReportesSupervisor", reportesAdministrador.Data);
        }
        [HttpGet]
        public async Task<IActionResult> ReportesFechas(DateTime fecha)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            if (idUsuario == null)
            {
                return Json(new { success = false, message = "Id de usuario no valido." });
            }
            var fechaString = fecha.ToString("yyyy-MM-dd");
            var reportesFechas = await _daoReportesFechas.getReportByDate(fechaString, idUsuario.Value, rol.Value);
            if (!reportesFechas.IsSuccess)
            {
                return Json(new { success = false, message = reportesFechas.Message });
            }
            return PartialView("_ReportesFechas", reportesFechas.Data);
        }
        [HttpGet]
        public async Task<IActionResult> Metas()
        {
            var id = HttpContext.Session.GetInt32("UsuarioId");
            if (id == null)
            {
                TempData["MessageError"] = "No ha iniciado sesion.";
                return RedirectToAction("Index", "Home");
            }
            var reportes = await _daoReportesMetas.getReportsByGoal(id.Value);
            return View("Metas", reportes.Data);
        }
        [HttpGet]
        public async Task<IActionResult> ReportesPorMes(int mes, int año)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            if (idUsuario == null)
            {
                return Json(new { success = false, message = "Id de usuario no valido." });
            }
            var reportesAdministrador = await _daoReportesFechas.getReportByDate(DateTime.Now.ToString(), idUsuario.Value, rol.Value, mes, año);
            if (!reportesAdministrador.IsSuccess)
            {
                return Json(new { success = false, message = reportesAdministrador.Message });
            }
            return PartialView("_ReportesMeses", reportesAdministrador.Data);
        }
    }
}