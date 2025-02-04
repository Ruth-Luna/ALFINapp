using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    public class RolController : Controller
    {
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesRoles _dbServicesRoles;
        public RolController( DBServicesGeneral dbServicesGeneral, DBServicesRoles dbServicesRoles)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesRoles = dbServicesRoles;
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