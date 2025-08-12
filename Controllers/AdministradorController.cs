using ALFINapp.API.Filters;
using ALFINapp.Infrastructure.Persistence.Models.DTOs;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class AdministradorController : Controller
    {
        public AdministradorController(){}

        [HttpGet]
        [PermissionAuthorization("Administrador", "Inicio")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> Inicio()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? IdRol = HttpContext.Session.GetInt32("RolUser");
            if (IdRol == null || usuarioId == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión";
                return RedirectToAction("Index", "Home");
            }
            return View("Administrador");
        }
    }
}