using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Views.Asignacion
{
    public class CamposSupervisoresDestino : PageModel
    {
        private readonly ILogger<CamposSupervisoresDestino> _logger;

        public CamposSupervisoresDestino(ILogger<CamposSupervisoresDestino> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}