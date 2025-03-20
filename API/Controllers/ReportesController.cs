using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Filters;
using ALFINapp.API.Models;
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
            if (rol == 1)
            {
                
            }
            else if (rol == 2)
            {
                
            }
            else if (rol == 3)
            {
                
            }
            var reportes = new ViewReportesGeneral { };
            return View("Reportes");
        }
    }
}