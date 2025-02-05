using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Filters;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ALFINapp.Models;

namespace ALFINapp.Controllers
{
    public class ReferidosController : Controller
    {
        public DBServicesGeneral _dbServicesGeneral;
        public DBServicesReferido _dbServicesReferido;
        public ReferidosController(DBServicesGeneral dbServicesGeneral, DBServicesReferido dbServicesReferido)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesReferido = dbServicesReferido;

        }
        public async Task<IActionResult> Referidos()
        { 
            try
            {
                var idUsuarioSupervisor = HttpContext.Session.GetInt32("UsuarioId");
                if (idUsuarioSupervisor == null)
                {
                    ViewData["MessageError"] = "No se ha podido obtener el id del usuario supervisor";
                    return RedirectToAction("Inicio", "Supervisor");
                }

                var getReferidos = await _dbServicesReferido.GetReferidosGeneral();
                if (getReferidos.IsSuccess == false || getReferidos.Data == null)
                {
                    ViewData["MessageError"] = getReferidos.Message;
                    return RedirectToAction("Inicio", "Supervisor");
                }
                return View("Referidos", getReferidos.Data);
            }
            catch (System.Exception ex)
            {
                ViewData["MessageError"] = ex.Message;
                return RedirectToAction("Inicio", "Supervisor");
            }
        }

        public IActionResult DatosEnviarDerivacion(
            DateTime FechaVisitaDerivacion,
            string AgenciaDerivacion,
            string AsesorDerivacion,
            string DNIAsesorDerivacion,
            string TelefonoDerivacion,
            string DNIClienteDerivacion,
            string NombreClienteDerivacion
        )
        {
            try
            {
                var generarDerivacion = new GenerarDerivacionDTO
                {
                    FechaVisitaDerivacion = FechaVisitaDerivacion,
                    AgenciaDerivacion = AgenciaDerivacion,
                    AsesorDerivacion = AsesorDerivacion,
                    DNIAsesorDerivacion = DNIAsesorDerivacion,
                    TelefonoDerivacion = TelefonoDerivacion,
                    DNIClienteDerivacion = DNIClienteDerivacion,
                    NombreClienteDerivacion = NombreClienteDerivacion
                };
                return PartialView("_DatosEnviarDerivacion", generarDerivacion);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}