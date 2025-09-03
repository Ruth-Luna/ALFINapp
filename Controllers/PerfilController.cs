using ALFINapp.API.Filters;
using ALFINapp.API.Models;
using ALFINapp.Datos;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class PerfilController : Controller
    {
        private readonly DA_Usuario _da_usuario = new DA_Usuario();

        public PerfilController(){}

        [HttpGet]
        [PermissionAuthorization("Perfil", "Perfil")]
        public IActionResult Perfil()
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
                var userInformation = _da_usuario.getUsuario(usuarioId.Value);
                if (userInformation == null)
                {
                    TempData["MessageError"] = "No se ha encontrado el usuario.";
                    return RedirectToAction("Index", "Home");
                }
                ViewData["RolUser"] = RolUser;
                return View("Perfil", new ViewUsuario(userInformation));
            }
            catch (System.Exception ex)
            {
                TempData["MessageError"] = ex.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
        }

        [HttpGet]
        public IActionResult ObtenerDatosUsuario()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return Json(new { success = false, message = "Usted no ha iniciado sesion" });
            }

            var userInformation = _da_usuario.getUsuario(usuarioId.Value);
            if (userInformation == null)
            {
                return Json(new { success = false, message = "No se ha encontrado el usuario" });
            }

            return Json(new { success = true, data = userInformation });
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
                var changePassword = await _da_usuario.ActualizarUsuario(userv);
                if (changePassword.IsSuccess == false)
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
                // campo = campo.ToLower();
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
        [HttpPost]
        public async Task<IActionResult> ActualizarCampos(string Departamento, string Provincia, string Distrito, string Telefono, string Correo)
        {
            try
            {
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "Usted no ha iniciado sesión" });
                }

                // Validar el correo si no está vacío
                if (!string.IsNullOrEmpty(Correo) && !System.Text.RegularExpressions.Regex.IsMatch(Correo, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                {
                    return Json(new { success = false, message = "El correo ingresado no es válido" });
                }

                var result = await _da_usuario.ActualizarCamposUsuario(usuarioId.Value, Departamento, Provincia, Distrito, Telefono, Correo);
                return Json(new { success = result.IsSuccess, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al actualizar los campos: " + ex.Message });
            }
        }
    }
}