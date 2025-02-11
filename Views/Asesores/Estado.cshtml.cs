using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Asesores
{
    public class Estado : PageModel
    {
        private readonly ILogger<Estado> _logger;

        public Estado(ILogger<Estado> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}