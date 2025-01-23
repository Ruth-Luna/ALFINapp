using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Asignaciones
{
    public class ActualizarAsignacion : PageModel
    {
        private readonly ILogger<ActualizarAsignacion> _logger;

        public ActualizarAsignacion(ILogger<ActualizarAsignacion> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}