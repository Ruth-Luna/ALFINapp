using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.DTOs;
using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class EmailController : Controller
    {
        private readonly IUseCaseRegisterEmail _useCaseRegisterEmail;

        public EmailController(
            IUseCaseRegisterEmail useCaseRegisterEmail
        )
        {
            _useCaseRegisterEmail = useCaseRegisterEmail;
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult Email()
        {
            ViewData["Rol"] = HttpContext.Session.GetInt32("RolUser");
            return View("Email");
        }
        [HttpPost]
        public async Task<IActionResult> RegisterEmail([FromBody] DtoVRegisterEmail request)
        {
            var idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("UsuarioId"));
            var result = await _useCaseRegisterEmail.Execute(request.email_update_users, idUsuario);
            if (!result.IsSuccess)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }
        
    }
}