using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    public class DerivacionController : Controller
    {
        private readonly DBServicesDerivacion _dBServicesDerivacion;
        public DerivacionController(DBServicesDerivacion dBServicesDerivacion)
        {
            _dBServicesDerivacion = dBServicesDerivacion;
        }

        [HttpPost]
        public async Task<IActionResult> GenerarDerivacion(DateTime FechaVisitaDerivacion, 
                                                                string AgenciaDerivacion,
                                                                string AsesorDerivacion,
                                                                string DNIAsesorDerivacion,
                                                                string TelefonoDerivacion,
                                                                string DNIClienteDerivacion,
                                                                string NombreClienteDerivacion)
        {
            try
            {
                var enviarDerivacion = await _dBServicesDerivacion.GenerarDerivacion(FechaVisitaDerivacion, AgenciaDerivacion, AsesorDerivacion, DNIAsesorDerivacion, TelefonoDerivacion, DNIClienteDerivacion, NombreClienteDerivacion);
                if (enviarDerivacion.IsSuccess)
                {
                    return Json(new { success = false, message = enviarDerivacion.Message });
                }
                return Json(new { success = true, message = enviarDerivacion.Message });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}