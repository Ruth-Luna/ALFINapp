using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Rol
{
    public class _Sidebar : PageModel
    {
        private readonly ILogger<_Sidebar> _logger;

        public _Sidebar(ILogger<_Sidebar> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}