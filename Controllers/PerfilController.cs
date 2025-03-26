using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Filters;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class PerfilController : Controller
    {
        private readonly DBServicesGeneral _dbservicesgeneral; // Add this line
        private readonly DBServicesUsuarios _dbservicesusuarios; // Add this line

        public PerfilController(DBServicesGeneral dbservicesgeneral, DBServicesUsuarios dbservicesusuarios) // Add this parameter
        {
            _dbservicesgeneral = dbservicesgeneral;
            _dbservicesusuarios = dbservicesusuarios; // Add this line
        }
        
        [HttpGet]
        [PermissionAuthorization("Perfil", "Perfil")]        
        public async Task<IActionResult> Perfil()
        {
            int? RolUser = HttpContext.Session.GetInt32("RolUser");
            try
            {
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                    return RedirectToAction("Index", "Home");
                }
                var userInformation = await _dbservicesgeneral.GetUserInformation(usuarioId.Value);
                if (userInformation.IsSuccess == false)
                {
                    TempData["MessageError"] = userInformation.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                ViewData["RolUser"] = RolUser;
                return View("Perfil", userInformation.Data);
            }
            catch (System.Exception ex)
            {
                TempData["MessageError"] = ex.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitNewPassword(string newPassword)
        {
            try
            {
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "Usted no ha iniciado sesion" });
                }
                var changePasswordResult = await _dbservicesgeneral.UpdatePasswordGeneralFunction(usuarioId.Value, newPassword);
                return Json(new { success = true, message = "Contraseña cambiada con exito" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EnviarEdicion(string campo, string nuevoValor)
        {
            try
            {
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "Usted no ha iniciado sesion" });
                }
                campo = campo.ToLower();
                var changePasswordResult = await _dbservicesusuarios.UpdateUsuarioXCampo(usuarioId.Value, campo, nuevoValor);
                if (changePasswordResult.IsSuccess == false)
                {
                    return Json(new { success = false, message = changePasswordResult.Message });
                }
                return Json(new { success = true, message = "Campo actualizado con exito" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}