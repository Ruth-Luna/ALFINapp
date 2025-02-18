using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    public class DerivacionController : Controller
    {
        private readonly DBServicesDerivacion _dBServicesDerivacion;
        private readonly DBServicesConsultasSupervisores _dBServicesConsultasSupervisores;
        private readonly DBServicesConsultasAdministrador _dBServicesConsultasAdministrador;
        private readonly DBServicesGeneral _dBServicesGeneral;
        public DerivacionController(
            DBServicesDerivacion dBServicesDerivacion, 
            DBServicesConsultasSupervisores dBServicesConsultasSupervisores, 
            DBServicesGeneral dBServicesGeneral,
            DBServicesConsultasAdministrador dBServicesConsultasAdministrador)
        {
            _dBServicesDerivacion = dBServicesDerivacion;
            _dBServicesConsultasSupervisores = dBServicesConsultasSupervisores;
            _dBServicesGeneral = dBServicesGeneral;
            _dBServicesConsultasAdministrador = dBServicesConsultasAdministrador;
        }

        [HttpGet]
        public async Task<IActionResult> Derivacion()
        {
            var rolUsuario = HttpContext.Session.GetInt32("RolUser");
            if (rolUsuario == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión.";
                return RedirectToAction("Index", "Home");
            }
            if (rolUsuario == 2 || rolUsuario == 1)
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
                    var newItem = new GESTIONDETALLE
                    {
                        DocCliente = item.DniCliente ?? string.Empty,
                        CodCanal = "DESCONOCIDO",
                        Canal = "A365",
                        FechaEnvio = item.FechaDerivacion,
                        Telefono = item.TelefonoCliente,
                        OrigenTelefono = "A365",
                        CodCampaña = item.NombreAgencia,
                        CodTip = 2,
                        DocAsesor = item.DniAsesor,
                    };
                    getClientesDatosDTO.Add(newItem);
                }
                ViewData["Asesores"] = getAsesoresAsignados.Data;
                return View("Derivacion", getClientesDatosDTO);
            }
            if (rolUsuario == 3)
            {
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (idUsuario == null)
                {
                    TempData["MessageError"] = "No se ha iniciado sesión.";
                    return RedirectToAction("Index", "Home");
                }
                var getDniAsesor = await _dBServicesGeneral.GetUserInformation(idUsuario.Value);
                if (!getDniAsesor.IsSuccess || getDniAsesor.Data == null)
                {
                    TempData["MessageError"] = getDniAsesor.Message;
                    return RedirectToAction("Index", "Home");
                }
                var DniAsesor = getDniAsesor.Data.Dni;
                var getDerivaciones = await _dBServicesDerivacion.GetEntradasBSDialXSupervisor(new List<Usuario> { new Usuario { Dni = DniAsesor } });
                if (!getDerivaciones.IsSuccess || getDerivaciones.Data == null)
                {
                    TempData["MessageError"] = getDerivaciones.Message;
                    return RedirectToAction("Index", "Home");
                }
                var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetClientesDerivadosGenerales(new List<Usuario> { new Usuario { Dni = DniAsesor } });
                if (!getClientesDerivadosGenerales.IsSuccess || getClientesDerivadosGenerales.Data == null)
                {
                    TempData["MessageError"] = getClientesDerivadosGenerales.Message;
                    return RedirectToAction("Index", "Home");
                }
                var getClientesDatosDTO = getDerivaciones.Data;
                foreach (var item in getClientesDerivadosGenerales.Data)
                {
                    var newItem = new GESTIONDETALLE
                    {
                        DocCliente = item.DniCliente ?? string.Empty,
                        CodCanal = "DESCONOCIDO",
                        Canal = "A365",
                        FechaEnvio = item.FechaDerivacion,
                        Telefono = item.TelefonoCliente,
                        OrigenTelefono = "A365",
                        CodCampaña = item.NombreAgencia,
                        CodTip = 2,
                        DocAsesor = item.DniAsesor,
                    };
                    getClientesDatosDTO.Add(newItem);
                }
                ViewData["Derivaciones"] = getClientesDatosDTO;
                return View("Derivacion", getClientesDatosDTO);
            }
            if (rolUsuario == 4)
            {
                var UsuarioIdSupervisor = HttpContext.Session.GetInt32("UsuarioId");
                if (UsuarioIdSupervisor == null)
                {
                    TempData["MessageError"] = "No se ha iniciado sesión.";
                    return RedirectToAction("Index", "Home");
                }
                var getAsesoresAsignados = await _dBServicesConsultasAdministrador.ConseguirTodosLosUsuarios();
                if (!getAsesoresAsignados.IsSuccess || getAsesoresAsignados.Data == null)
                {
                    TempData["MessageError"] = getAsesoresAsignados.Message;
                    return RedirectToAction("Index", "Home");
                }
                var todosAsesores = getAsesoresAsignados.Data.Where(a => a.IdRol == 3).ToList();
                var getclientesDerivadosBSDial = await _dBServicesDerivacion.GetEntradasBSDialXSupervisor(todosAsesores);
                if (!getclientesDerivadosBSDial.IsSuccess || getclientesDerivadosBSDial.Data == null)
                {
                    TempData["MessageError"] = getclientesDerivadosBSDial.Message;
                    return RedirectToAction("Index", "Home");
                }
                var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetClientesDerivadosGenerales(todosAsesores);
                if (!getClientesDerivadosGenerales.IsSuccess || getClientesDerivadosGenerales.Data == null)
                {
                    TempData["MessageError"] = getClientesDerivadosGenerales.Message;
                    return RedirectToAction("Index", "Home");
                }
                var getClientesDatosDTO = getclientesDerivadosBSDial.Data;
                foreach (var item in getClientesDerivadosGenerales.Data)
                {
                    var newItem = new GESTIONDETALLE
                    {
                        DocCliente = item.DniCliente ?? string.Empty,
                        CodCanal = "DESCONOCIDO",
                        Canal = "A365",
                        FechaEnvio = item.FechaDerivacion,
                        Telefono = item.TelefonoCliente,
                        OrigenTelefono = "A365",
                        CodCampaña = item.NombreAgencia,
                        CodTip = 2,
                        DocAsesor = item.DniAsesor,
                    };
                    getClientesDatosDTO.Add(newItem);
                }
                ViewData["Asesores"] = todosAsesores;
                return View("Derivacion", getClientesDatosDTO);
            }
            else
            {
                TempData["MessageError"] = "No tiene permisos para acceder a esta vista.";
                return RedirectToAction("Index", "Home");
            }
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
                    DNIAsesorDerivacion,
                    TelefonoDerivacion,
                    DNIClienteDerivacion,
                    NombreClienteDerivacion);
                if (enviarDerivacion.IsSuccess)
                {
                    return Json(new { success = false, message = enviarDerivacion.Message });
                }
                var verificarDerivacion = await _dBServicesDerivacion.VerificarDerivacionEnviada(DNIClienteDerivacion);
                if (verificarDerivacion.IsSuccess)
                {
                    return Json(new { success = false, message = verificarDerivacion.Message });
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
                    var newItem = new GESTIONDETALLE
                    {
                        DocCliente = item.DniCliente ?? string.Empty,
                        CodCanal = "DESCONOCIDO",
                        Canal = "CANAL DESCONOCIDO",
                        FechaEnvio = item.FechaDerivacion,
                        Telefono = item.TelefonoCliente,
                        OrigenTelefono = "CLIENTES ENRIQUECIDOS",
                        CodCampaña = item.NombreAgencia,
                        CodTip = 2,
                        DocAsesor = item.DniAsesor,
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