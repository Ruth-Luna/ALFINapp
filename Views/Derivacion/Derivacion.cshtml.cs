using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Derivacion
{
    public class Derivacion : PageModel
    {
        private readonly ILogger<Derivacion> _logger;

        public Derivacion(ILogger<Derivacion> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}