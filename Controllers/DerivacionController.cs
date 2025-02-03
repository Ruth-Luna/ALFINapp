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
        private readonly DBServicesConsultasSupervisores _dBServicesConsultasSupervisores;
        public DerivacionController(DBServicesDerivacion dBServicesDerivacion, DBServicesConsultasSupervisores dBServicesConsultasSupervisores)
        {
            _dBServicesDerivacion = dBServicesDerivacion;
            _dBServicesConsultasSupervisores = dBServicesConsultasSupervisores;
        }

        [HttpGet]
        public async Task<IActionResult> Derivacion()
        {
            var UsuarioIdSupervisor = HttpContext.Session.GetInt32("UsuarioId");
            if (UsuarioIdSupervisor == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión.";
                return RedirectToAction("Index", "Home");
            }
            var getAsesoresAsignados = await _dBServicesConsultasSupervisores.GetAsesorsFromSupervisor(UsuarioIdSupervisor.Value);
            if (!getAsesoresAsignados.IsSuccess || getAsesoresAsignados.Data == null)
            {
                TempData["MessageError"] = getAsesoresAsignados.Message;
                return RedirectToAction("Index", "Home");
            }
            ViewData["Asesores"] = getAsesoresAsignados.Data;
            return View("Derivacion");
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
        [HttpGet]
        public async Task<IActionResult> ObtenerDerivacionesXAsesor(string DniAsesor)
        {
            try
            {
                var getDerivaciones = await _dBServicesDerivacion.GetDerivacionesXAsesor(DniAsesor);
                if (!getDerivaciones.IsSuccess || getDerivaciones.Data == null)
                {
                    return Json(new { success = false, message = getDerivaciones.Message });
                }
                ViewData["Derivaciones"] = getDerivaciones.Data;
                return PartialView("_DerivacionesAsesor");
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}