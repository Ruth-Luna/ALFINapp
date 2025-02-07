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
            var rolesConVistas = new List<List<VistasPorRolDTO>>();
            for (int i = 1; i <= 3; i++)
            {
                var getRol = await _dbServicesRoles.getVistasPorRol(i);
                if (getRol.Data != null)
                {
                    rolesConVistas.Add(getRol.Data);
                }
            }
            var getTodasLasVistasRutas = await _dbServicesRoles.getTodasLasVistasRutas();
            if (!getTodasLasVistasRutas.IsSuccess || getTodasLasVistasRutas.Data == null)
            {
                TempData["MessageError"] = getTodasLasVistasRutas.Message;
                return RedirectToAction("Inicio", "Supervisor");
            }
            ViewData["Vistas"] = getTodasLasVistasRutas.Data;
            return View("Roles", rolesConVistas);
        }
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
    }
}