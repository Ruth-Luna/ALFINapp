using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Filters;
using ALFINapp.Models;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class RolController : Controller
    {
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesRoles _dbServicesRoles;
        public RolController( DBServicesGeneral dbServicesGeneral, DBServicesRoles dbServicesRoles)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesRoles = dbServicesRoles;
        }
        [HttpGet]
        [PermissionAuthorization("Rol", "Roles")]

        public async Task<IActionResult> Roles()
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión";
                return RedirectToAction("Index", "Home");
            }
            /*var tienePermiso = await _dbServicesRoles.tienePermiso(rol.Value, "Rol", "Roles");
            if (!tienePermiso.IsSuccess || tienePermiso.Data == false)
            {
                var vista = await _dbServicesRoles.getVistaPorDefecto(rol.Value);
                TempData["MessageError"] = "No tiene permiso para acceder a esta vista";
                if (vista.Data != null)
                {
                    return RedirectToAction(vista.Data.nombre_vista, vista.Data.ruta_vista);
                }
                TempData["MessageError"] = "No se pudo obtener la vista por defecto";
                return RedirectToAction("Index", "Home");
            }*/
            var getTodosLosRoles = await _dbServicesRoles.getAllRoles();
            if (!getTodosLosRoles.IsSuccess || getTodosLosRoles.Data == null)
            {
                TempData["MessageError"] = getTodosLosRoles.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
            var rolesConVistas = new List<List<VistasPorRolDTO>>();
            foreach (var rolItem in getTodosLosRoles.Data)
            {
                var vistas = await _dbServicesRoles.getVistasPorRol(rolItem.IdRol);
                if (vistas.IsSuccess && vistas.Data != null)
                {
                    rolesConVistas.Add(vistas.Data);
                }
            }
            var getTodasLasVistasRutas = await _dbServicesRoles.getTodasLasVistasRutas();
            if (!getTodasLasVistasRutas.IsSuccess || getTodasLasVistasRutas.Data == null)
            {
                TempData["MessageError"] = getTodasLasVistasRutas.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
            ViewData["Vistas"] = getTodasLasVistasRutas.Data;
            return View("Roles", rolesConVistas);
        }
        [HttpGet]
        public async Task<IActionResult> Sidebar()
        {
            int? RolUser = HttpContext.Session.GetInt32("RolUser");
            if (RolUser == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var vistas = await _dbServicesRoles.getVistasPorRol(RolUser.Value);
            if (!vistas.IsSuccess || vistas.Data == null)
            {
                return Json(new { success = false, message = vistas.Message });
            }
            return PartialView("_Sidebar", vistas.Data);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarPermisosRoles (string rol, int idVista, int idRol)
        {
            try
            {
                var actualizarPermiso = await _dbServicesRoles.actualizarPermisoRol(idVista, idRol);
                if (!actualizarPermiso.IsSuccess)
                {
                    return Json(new { success = false, message = actualizarPermiso.Message });
                }
                return Json(new { success = true, message = "Permiso actualizado correctamente" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}