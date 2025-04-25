using ALFINapp.API.Filters;
using ALFINapp.Infrastructure.Persistence.Models.DTOs;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class AdministradorController : Controller
    {
        private readonly DBServicesConsultasAdministrador _dBServicesConsultasAdministrador;
        public AdministradorController(
            DBServicesConsultasSupervisores dBServicesConsultasSupervisores,
            DBServicesConsultasAdministrador dBServicesConsultasAdministrador)
        {
            _dBServicesConsultasAdministrador = dBServicesConsultasAdministrador;
        }

        [HttpGet]
        [PermissionAuthorization("Administrador", "Inicio")]        
        public async Task<IActionResult> Inicio()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? IdRol = HttpContext.Session.GetInt32("RolUser");
            if (IdRol == null || usuarioId == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión";
                return RedirectToAction("Index", "Home");
            }

            var allSupervisor = await _dBServicesConsultasAdministrador.ConseguirTodosLosSupervisores();
            if (!allSupervisor.IsSuccess || allSupervisor.Data == null)
            {
                TempData["MessageError"] = allSupervisor.Message;
                return RedirectToAction("Redireccionar", "Error");
            }

            var consultasSupervisorEnriquecidas = new List<EnriquecerConsultasSupervisorDTO>();

            foreach (var supervisor in allSupervisor.Data)
            {
            }
            
            return View("Administrador");
        }
    }
}