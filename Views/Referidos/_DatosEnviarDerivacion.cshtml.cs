using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Referidos
{
    public class _DatosEnviarDerivacion : PageModel
    {
        private readonly ILogger<_DatosEnviarDerivacion> _logger;

        public _DatosEnviarDerivacion(ILogger<_DatosEnviarDerivacion> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}