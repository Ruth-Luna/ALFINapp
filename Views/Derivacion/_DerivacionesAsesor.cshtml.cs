using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Derivacion
{
    public class _DerivacionesAsesor : PageModel
    {
        private readonly ILogger<_DerivacionesAsesor> _logger;

        public _DerivacionesAsesor(ILogger<_DerivacionesAsesor> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}