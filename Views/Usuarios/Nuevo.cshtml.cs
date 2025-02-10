using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Usuarios
{
    public class Nuevo : PageModel
    {
        private readonly ILogger<Nuevo> _logger;

        public Nuevo(ILogger<Nuevo> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}