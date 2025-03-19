using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.API.Controllers
{
    [Route("[controller]")]
    public class ReportesController : Controller
    {
        public ReportesController()
        {
        }
        [HttpGet]
        [PermissionAuthorization("Reportes", "Reportes")]
        public IActionResult Reportes()
        {
            var rol = HttpContext.Session.GetInt32("UsuarioRol");
            if (rol == null)
            {
                TempData["MessageError"] = "No tiene permisos para acceder a esta sección";
                return RedirectToAction("Index", "Home");
            }
            if (rol == 1)
            {
                
            }
            else if (rol == 2)
            {
                
            }
            else if (rol == 3)
            {
                
            }
            return View("Reportes");
        }
    }
}