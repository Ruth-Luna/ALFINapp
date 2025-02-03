using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Consulta
{
    public class _VistaDerivacionManual : PageModel
    {
        private readonly ILogger<_VistaDerivacionManual> _logger;

        public _VistaDerivacionManual(ILogger<_VistaDerivacionManual> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}