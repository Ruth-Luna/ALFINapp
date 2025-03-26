using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Filters;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [PermissionAuthorization("Error", "ErrorRol")]
        public IActionResult ErrorRol()
        {
            return View("ErrorRol");
        }

        [HttpGet]
        [PermissionAuthorization("Error", "Redireccionar")]
        public IActionResult Redireccionar()
        {
            return View("Redireccionar");
        }
    }
}