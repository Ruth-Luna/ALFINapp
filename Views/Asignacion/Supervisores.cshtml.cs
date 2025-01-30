using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Asignacion
{
    public class Supervisores : PageModel
    {
        private readonly ILogger<Supervisores> _logger;

        public Supervisores(ILogger<Supervisores> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}