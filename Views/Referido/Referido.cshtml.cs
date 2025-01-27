using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Referido
{
    public class Referido : PageModel
    {
        private readonly ILogger<Referido> _logger;

        public Referido(ILogger<Referido> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}