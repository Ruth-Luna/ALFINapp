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
        public DBServicesDerivacion _dBServicesDerivacion;
        public ReferidosController(
            DBServicesGeneral dbServicesGeneral, 
            DBServicesReferido dbServicesReferido,
            DBServicesDerivacion dBServicesDerivacion)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesReferido = dbServicesReferido;
            _dBServicesDerivacion = dBServicesDerivacion;
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
        public async Task<IActionResult> EnviarDerivacionPorReferencia (
            string AgenciaDerivacion,
            string AsesorDerivacion,
            string DNIAsesorDerivacion,
            string DNIClienteDerivacion,
            DateTime FechaVisitaDerivacion,
            string NombreClienteDerivacion,
            string TelefonoDerivacion)
        {
            try
            {
                var enviarDerivacion = await _dBServicesDerivacion.GenerarDerivacion(
                    FechaVisitaDerivacion, 
                    AgenciaDerivacion, 
                    AsesorDerivacion, 
                    DNIAsesorDerivacion, 
                    TelefonoDerivacion, 
                    DNIClienteDerivacion, 
                    NombreClienteDerivacion);

                if (enviarDerivacion.IsSuccess == false)
                {
                    return Json(new { success = false, message = enviarDerivacion.Message });
                }
                return Json(new { success = true, message = "Derivacion enviada correctamente" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}