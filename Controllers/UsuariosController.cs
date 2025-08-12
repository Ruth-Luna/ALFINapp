using ALFINapp.API.Filters;
using ALFINapp.API.Models;
using ALFINapp.Datos;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class UsuariosController : Controller
    {
        DA_Usuario _daUsuario = new DA_Usuario();

        public UsuariosController(){}

        [HttpGet]
        [PermissionAuthorization("Usuarios", "Administracion")]
        public IActionResult Administracion()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ListarUsuarioAdministrador(int? idUsuario)
        {
            var listarUsuario = _daUsuario.ListarUsuarios(idUsuario);
            return Json(listarUsuario);
        }
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] ViewUsuario usuario)
        {
            try
            {
                var UsuarioIdJefe = HttpContext.Session.GetInt32("UsuarioId");
                if (UsuarioIdJefe == null)
                {
                    return Json(new { success = false, message = "No se ha podido crear el usuario" });
                }
                var result = await _daUsuario.CrearUsuario(usuario, UsuarioIdJefe.Value);
                if (result.IsSuccess)
                {
                    return Json(new { success = true, message = "Usuario creado correctamente" });
                }
                return Json(new { success = false, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult ActualizarUsuario([FromBody] ViewUsuario usuario)
        {
            try
            {
                bool actualizado = _daUsuario.ActualizarUsuario(usuario);
                if (actualizado)
                    return Ok(new { success = true, mensaje = "Usuario actualizado correctamente." });
                else
                    return StatusCode(500, new { success = false, mensaje = "No se pudo actualizar el usuario. No se afectaron filas." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    mensaje = "Ocurrió un error interno al intentar actualizar el usuario.",
                    detalle = ex.Message
                });
            }
        }

        public IActionResult ActualizarEstadoUsuario([FromBody] ViewUsuario usuario)
        {
            try
            {
                string estado = usuario.Estado == "1" ? "ACTIVO" : "INACTIVO";

                bool actualizado = _daUsuario.ActualizarEstado(usuario.IdUsuario, estado);

                if (actualizado)
                    return Ok(new { success = true, mensaje = "Se ha cambiado el estado del Usuario" });
                else
                    return StatusCode(500, new { success = false, mensaje = "No se pudo actualizar el estado del usuario." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    mensaje = "Ocurrió un error interno al intentar actualizar el estado.",
                    detalle = ex.Message
                });
            }
        }
        

        [HttpGet]
        public JsonResult ListarRoles()
        {
            var listarRol = _daUsuario.ListarRoles();
            return Json(listarRol);
        }
    }
}