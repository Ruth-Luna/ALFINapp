using ALFINapp.API.DTOs;
using ALFINapp.API.Filters;
using ALFINapp.Datos;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class EmailController : Controller
    {
        private DA_Usuario dA_Usuario = new DA_Usuario();
        public EmailController(){}
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult Email()
        {
            ViewData["Rol"] = HttpContext.Session.GetInt32("RolUser");
            return View("Email");
        }
        [HttpPost]
        public IActionResult RegisterEmail([FromBody] DtoVRegisterEmail request)
        {
            var idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("UsuarioId"));
            var result = dA_Usuario.UpdateCampo(idUsuario, "Correo", request.email_update_users ?? string.Empty);
            if (!result.IsSuccess)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }

    }
}