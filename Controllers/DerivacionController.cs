using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using ALFINapp.Services;
using ALFINapp.Views.Derivacion;
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
            var getclientesDerivadosBSDial = await _dBServicesDerivacion.GetEntradasBSDialXSupervisor(getAsesoresAsignados.Data);
            if (!getclientesDerivadosBSDial.IsSuccess || getclientesDerivadosBSDial.Data == null)
            {
                TempData["MessageError"] = getclientesDerivadosBSDial.Message;
                return RedirectToAction("Index", "Home");
            }
            var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetClientesDerivadosGenerales(getAsesoresAsignados.Data);
            if (!getClientesDerivadosGenerales.IsSuccess || getClientesDerivadosGenerales.Data == null)
            {
                TempData["MessageError"] = getClientesDerivadosGenerales.Message;
                return RedirectToAction("Index", "Home");
            }
            var getClientesDatosDTO = getclientesDerivadosBSDial.Data;
            foreach (var item in getClientesDerivadosGenerales.Data)
            {
                var newItem = new FeedGReportes
                {
                    DNI = item.DniCliente ?? string.Empty,
                    COD_CANAL = "DESCONOCIDO",
                    CANAL = "CANAL DESCONOCIDO",
                    FECHA_ENVIO = item.FechaDerivacion,
                    TELEFONO = item.TelefonoCliente,
                    ORIGEN_TELEFONO = "CLIENTES ENRIQUECIDOS",
                    COD_CAMPAÑA = item.NombreAgencia,
                    COD_TIP = 2,
                    DNI_ASESOR = item.DniAsesor,
                };
                getClientesDatosDTO.Add(newItem);
            }
            ViewData["Asesores"] = getAsesoresAsignados.Data;
            return View("Derivacion", getClientesDatosDTO);
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
                var enviarDerivacion = await _dBServicesDerivacion.GenerarDerivacion(
                    FechaVisitaDerivacion, 
                    AgenciaDerivacion, 
                    AsesorDerivacion, 
                    DNIAsesorDerivacion, 
                    TelefonoDerivacion, 
                    DNIClienteDerivacion, 
                    NombreClienteDerivacion);
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
                var DniAsesorGet = new Usuario
                {
                    Dni = DniAsesor
                };

                var enviarDni = new List<Usuario>();
                enviarDni.Add(DniAsesorGet);
                var getDerivaciones = await _dBServicesDerivacion.GetEntradasBSDialXSupervisor(enviarDni);
                if (!getDerivaciones.IsSuccess || getDerivaciones.Data == null)
                {
                    return Json(new { success = false, message = getDerivaciones.Message });
                }
                var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetClientesDerivadosGenerales(enviarDni);
                if (!getClientesDerivadosGenerales.IsSuccess || getClientesDerivadosGenerales.Data == null)
                {
                    return Json(new { success = false, message = getClientesDerivadosGenerales.Message });
                }
                var getClientesDatosDTO = getDerivaciones.Data;
                foreach (var item in getClientesDerivadosGenerales.Data)
                {
                    var newItem = new FeedGReportes
                    {
                        DNI = item.DniCliente ?? string.Empty,
                        COD_CANAL = "DESCONOCIDO",
                        CANAL = "CANAL DESCONOCIDO",
                        FECHA_ENVIO = item.FechaDerivacion,
                        TELEFONO = item.TelefonoCliente,
                        ORIGEN_TELEFONO = "CLIENTES ENRIQUECIDOS",
                        COD_CAMPAÑA = item.NombreAgencia,
                        COD_TIP = 2,
                        DNI_ASESOR = item.DniAsesor,
                    };
                    getClientesDatosDTO.Add(newItem);
                }
                ViewData["Derivaciones"] = getClientesDatosDTO;
                return PartialView("_DerivacionesAsesor");
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}