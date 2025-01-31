using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Asignacion
{
    public class TablaAsignacion : PageModel
    {
        private readonly ILogger<TablaAsignacion> _logger;

        public TablaAsignacion(ILogger<TablaAsignacion> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}