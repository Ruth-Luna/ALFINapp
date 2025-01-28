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
    public class ReferidoController : Controller
    {
        public DBServicesGeneral _dbServicesGeneral;
        public DBServicesReferido _dbServicesReferido;
        private IEmailService _emailService;
        public ReferidoController( DBServicesGeneral dbServicesGeneral, DBServicesReferido dbServicesReferido, IEmailService emailService)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesReferido = dbServicesReferido;
            _emailService = emailService;
        }
        public async Task<IActionResult> Referido()
        {
            return View("Referido");
        }

        public async Task<IActionResult> BuscarDNIReferido(string dniBusqueda)
        {
            var getDNIReferido = await _dbServicesReferido.GetDataFromDNI(dniBusqueda);
            var getBases = await _dbServicesGeneral.GetAgenciasDisponibles();

            if (getDNIReferido.IsSuccess == false)
            {
                return Json(new { success = false, message = getDNIReferido.Message});
            }

            ViewData["Agencias"] = getBases.data;

            return PartialView("DetalleDNIReferido", getDNIReferido.data);
        }

        public async Task<IActionResult> ReferirCliente(string dniReferir,
                                                                        string fuenteBase,
                                                                        string nombres,
                                                                        string apellidos,
                                                                        string dniUsuario)
        {
            var mandarReferido = await _dbServicesReferido.GuardarClienteReferido(dniReferir, fuenteBase, nombres, apellidos, dniUsuario);
            if (mandarReferido.IsSuccess == false)
            {
                //El cliente referido no ha podido ser guardado
                return Json(new { success = false, message = mandarReferido.Message});
            }

            var getReferido = await _dbServicesReferido.GetClienteReferido(dniReferir);
            await _emailService.SendEmailAsync(
                "santiagovl0308@gmail.com", // Destinatario
                "Correo de prueba",           // Asunto
                "<h1>Este es un correo de prueba</h1><p>Saludos, equipo A365.</p>" // Cuerpo en HTML
            );
            return Json(new { success = true, message = getReferido.Message});
        }
    }
}