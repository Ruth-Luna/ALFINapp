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
                    TempData["MessageError"] = "No se ha podido obtener el id del usuario supervisor";
                    return RedirectToAction("Inicio", "Supervisor");
                }

                var getReferidos = await _dbServicesReferido.GetReferidosGeneral();
                if (getReferidos.IsSuccess == false || getReferidos.Data == null)
                {
                    TempData["MessageError"] = getReferidos.Message;
                    return RedirectToAction("Inicio", "Supervisor");
                }
                return View("Referidos", getReferidos.Data);
            }
            catch (System.Exception ex)
            {
                TempData["MessageError"] = ex.Message;
                return RedirectToAction("Inicio", "Supervisor");
            }
        }

        public IActionResult DatosEnviarDerivacion(
            DateTime FechaVisitaDerivacion,
            string AgenciaDerivacion,
            string NombreAsesorDerivacion,
            string DNIAsesorDerivacion,
            string TelefonoDerivacion,
            string DNIClienteDerivacion,
            string NombreClienteDerivacion,
            decimal OfertaEnviadaDerivacion
        )
        {
            try
            {
                var generarDerivacion = new GenerarDerivacionDTO
                {
                    FechaVisitaDerivacion = FechaVisitaDerivacion,
                    AgenciaDerivacion = AgenciaDerivacion,
                    NombreAsesorDerivacion = NombreAsesorDerivacion,
                    DNIAsesorDerivacion = DNIAsesorDerivacion,
                    TelefonoDerivacion = TelefonoDerivacion,
                    DNIClienteDerivacion = DNIClienteDerivacion,
                    NombreClienteDerivacion = NombreClienteDerivacion,
                    OfertaEnviadaDerivacion = OfertaEnviadaDerivacion
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
                    DateTime.Now, 
                    AgenciaDerivacion, 
                    DNIAsesorDerivacion, 
                    TelefonoDerivacion, 
                    DNIClienteDerivacion, 
                    NombreClienteDerivacion);

                if (enviarDerivacion.IsSuccess == false)
                {
                    return Json(new { success = false, message = enviarDerivacion.Message });
                }
                FechaVisitaDerivacion = FechaVisitaDerivacion.AddHours(-10);
                var modificarEstadoReferido = await _dbServicesReferido.ModificarEstadoReferido(DNIAsesorDerivacion, DNIClienteDerivacion, AgenciaDerivacion, TelefonoDerivacion, FechaVisitaDerivacion);
                if (modificarEstadoReferido.IsSuccess == false)
                {
                    return Json(new { success = false, message = modificarEstadoReferido.Message });
                }

                var verificarDerivacion = await _dBServicesDerivacion.VerificarDerivacionEnviada(DNIClienteDerivacion);

                if (verificarDerivacion.IsSuccess == false)
                {
                    return Json(new { success = false, message = verificarDerivacion.Message });
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