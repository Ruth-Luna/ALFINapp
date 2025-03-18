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
    [RequireSession]
    public class GerenteZonalController : Controller
    {

        public GerenteZonalController()
        {
        }
        [PermissionAuthorization("GerenteZonal", "Inicio")]        
        public IActionResult Inicio()
        {
            return View();
        }
    }
}