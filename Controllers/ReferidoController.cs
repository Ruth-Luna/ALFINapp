using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    public class ReferidoController : Controller
    {
        public ReferidoController()
        {
            
        }

        public async Task<IActionResult> Referido()
        {
            return View("Referido");
        }
    }
}