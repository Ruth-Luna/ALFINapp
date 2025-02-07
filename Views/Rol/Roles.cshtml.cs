using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Rol
{
    public class Roles : PageModel
    {
        private readonly ILogger<Roles> _logger;

        public Roles(ILogger<Roles> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}