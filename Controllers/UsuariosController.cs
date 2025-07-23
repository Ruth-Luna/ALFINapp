using ALFINapp.API.Filters;
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

        private readonly DBServicesConsultasAdministrador _DBServicesConsultasAdministrador;
        private readonly DBServicesUsuarios _DBServicesUsuarios;
        private readonly DBServicesGeneral _DBServicesGeneral;
        private readonly DBServicesRoles _DBServicesRoles;
        public UsuariosController(
            DBServicesConsultasAdministrador DBServicesConsultasAdministrador,
            DBServicesUsuarios DBServicesUsuarios,
            DBServicesGeneral DBServicesGeneral,
            DBServicesRoles DBServicesRoles)
        {
            _DBServicesConsultasAdministrador = DBServicesConsultasAdministrador;
            _DBServicesUsuarios = DBServicesUsuarios;
            _DBServicesGeneral = DBServicesGeneral;
            _DBServicesRoles = DBServicesRoles;
        }

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

        [HttpGet]
        public JsonResult ListarRoles()
        {
            var listarRol = _daUsuario.ListarRoles();
            return Json(listarRol);
        }



        //[HttpPost]
        //public async Task<IActionResult> ModificarUsuario([FromBody] Usuario usuario)
        //{
        //    try
        //    {
        //        var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
        //        if (idUsuario == null)
        //        {
        //            return Json(new { success = false, message = "No se ha podido modificar el usuario" });
        //        }
        //        var result = await _DBServicesUsuarios.ModificarUsuario(usuario, idUsuario.Value);
        //        if (result.IsSuccess)
        //        {
        //            return Json(new { success = true, message = "Datos actualizados correctamente" });
        //        }
        //        return Json(new { success = false, message = result.Message });
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> CambiarEstadoUsuario(int IdUsuario, int? accion)
        //{
        //    try
        //    {
        //        var UsuarioIdJefe = HttpContext.Session.GetInt32("UsuarioId");
        //        if (UsuarioIdJefe == null)
        //        {
        //            return Json(new { success = false, message = "No se ha podido cambiar el estado del usuario" });
        //        }
        //        if (accion == 0)
        //        {
        //            var result = await _DBServicesUsuarios.DesactivarUsuario(IdUsuario, UsuarioIdJefe.Value);
        //            if (result.IsSuccess)
        //            {
        //                return Json(new { success = true, message = "Los datos se han actualizado correctamente" });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = result.Message });
        //            }
        //        }
        //        else if (accion == 1)
        //        {
        //            var result = await _DBServicesUsuarios.ActivarUsuario(IdUsuario, UsuarioIdJefe.Value);
        //            if (result.IsSuccess)
        //            {
        //                return Json(new { success = true, message = "Los datos se han actualizado correctamente" });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = result.Message });
        //            }
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "No se ha podido cambiar el estado del usuario" });
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        //[HttpGet]
        //[PermissionAuthorization("Usuarios", "Nuevo")]
        //public async Task<IActionResult> Nuevo()
        //{
        //    var getSupervisores = await _DBServicesConsultasAdministrador.ConseguirTodosLosSupervisores();
        //    return View("Nuevo", getSupervisores.Data);
        //}

        //[HttpPost]
        //public async Task<IActionResult> CrearUsuario([FromBody] Usuario usuario)
        //{
        //    try
        //    {
        //        var UsuarioIdJefe = HttpContext.Session.GetInt32("UsuarioId");
        //        if (UsuarioIdJefe == null)
        //        {
        //            return Json(new { success = false, message = "No se ha podido crear el usuario" });
        //        }
        //        var result = await _daUsuario.CrearUsuario(usuario, UsuarioIdJefe.Value);
        //        if (result.IsSuccess)
        //        {
        //            return Json(new { success = true, message = "Usuario creado correctamente" });
        //        }
        //        return Json(new { success = false, message = result.Message });
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> ModificarUsuarioVista(int IdUsuario)
        //{
        //    try
        //    {
        //        var getUsuario = await _DBServicesGeneral.GetUserInformation(IdUsuario);
        //        if (!getUsuario.IsSuccess || getUsuario.Data == null)
        //        {
        //            return Json(new { success = false, message = getUsuario.Message });
        //        }
        //        var getSupervisores = await _DBServicesConsultasAdministrador.ConseguirTodosLosSupervisores();
        //        if (!getSupervisores.IsSuccess || getSupervisores.Data == null)
        //        {
        //            return Json(new { success = false, message = getSupervisores.Message });
        //        }
        //        var getRoles = await _DBServicesRoles.getRoles();
        //        if (!getRoles.IsSuccess || getRoles.Data == null)
        //        {
        //            return Json(new { success = false, message = getRoles.Message });
        //        }
        //        ViewData["Roles"] = getRoles.Data;
        //        ViewData["Supervisores"] = getSupervisores.Data;
        //        return PartialView("ModificarUsuario", getUsuario.Data);
        //    }
        //    catch (System.Exception)
        //    {
        //        return RedirectToAction("Administracion");
        //    }

        //}
    }
}