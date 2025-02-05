using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Referidos
{
    public class Referidos : PageModel
    {
        private readonly ILogger<Referidos> _logger;

        public Referidos(ILogger<Referidos> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}