using ALFINapp.API.Filters;
using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Perfil;
using ALFINapp.Datos;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class PerfilController : Controller
    {
        private readonly DA_Usuario _da_usuario = new DA_Usuario();
        private readonly IUseCaseGetPerfil _useCaseGetPerfil;

        public PerfilController(
            IUseCaseGetPerfil useCaseGetPerfil)
        {
            _useCaseGetPerfil = useCaseGetPerfil;
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
                var userInformation = await _useCaseGetPerfil.exec(usuarioId.Value);
                if (userInformation.success == false)
                {
                    TempData["MessageError"] = userInformation.message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                ViewData["RolUser"] = RolUser;
                return View("Perfil", userInformation.data);
            }
            catch (System.Exception ex)
            {
                TempData["MessageError"] = ex.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
        }

        [HttpPost]
        public IActionResult SubmitNewPassword(string newPassword)
        {
            try
            {
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "Usted no ha iniciado sesion" });
                }
                if (string.IsNullOrEmpty(newPassword) || newPassword.Trim().Length == 0)
                {
                    return Json(new { success = false, message = "La nueva contraseña no puede estar vacía" });
                }
                if (newPassword.Length < 8)
                {
                    return Json(new { success = false, message = "La nueva contraseña debe tener al menos 8 caracteres" });
                }
                var user = _da_usuario.getUsuario(usuarioId.Value);
                if (user == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado" });
                }
                var userv = new ViewUsuario(user);
                userv.Contrasenia = newPassword;
                var changePassword = _da_usuario.ActualizarUsuario(userv);
                if (changePassword == false)
                {
                    return Json(new { success = false, message = "Error al cambiar la contraseña" });
                }
                return Json(new { success = true, message = "Contraseña cambiada con exito" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult EnviarEdicion(string campo, string nuevoValor)
        {
            try
            {
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "Usted no ha iniciado sesion" });
                }
                campo = campo.ToLower();
                var changePassword = _da_usuario.UpdateCampo(usuarioId.Value, campo, nuevoValor);
                if (changePassword.IsSuccess == false)
                {
                    return Json(new { success = false, message = changePassword.Message });
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