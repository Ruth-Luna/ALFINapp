using ALFINapp.API.Filters;
using ALFINapp.Datos;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class AsesoresController : Controller
    {
        private readonly DA_Usuario _da_usuario = new DA_Usuario();
        public AsesoresController(){}
        [HttpGet]
        [PermissionAuthorization("Asesores", "Usuarios")]
        public IActionResult Usuarios()
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                return RedirectToAction("Home", "Index");
            }
            int? rolSupervisorActual = HttpContext.Session.GetInt32("RolUser");
            if (rolSupervisorActual == null)
            {
                return Json(new { success = false, message = "Usted no ha iniciado sesion" });
            }
            ViewData["RolActual"] = rolSupervisorActual.Value;
            return View("Usuarios");
        }
        [HttpGet]
        [PermissionAuthorization("Asesores", "Estado")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> Estado()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                return RedirectToAction("Home", "Index");
            }
            var asesoresAsignadosaSupervisor = _da_usuario.ListarAsesores(idSupervisorActual.Value);
            if (asesoresAsignadosaSupervisor == null || !asesoresAsignadosaSupervisor.Any())
            {
                TempData["Error"] = "No se encontraron asesores asignados a su cuenta";
                return RedirectToAction("Supervisor", "Inicio");
            }
            return View("Estado", asesoresAsignadosaSupervisor);
        }
    }
}