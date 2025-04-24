using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Derivacion;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class DerivacionController : Controller
    {
        private readonly DBServicesDerivacion _dBServicesDerivacion;
        private readonly DBServicesConsultasSupervisores _dBServicesConsultasSupervisores;
        private readonly DBServicesConsultasAdministrador _dBServicesConsultasAdministrador;
        private readonly DBServicesGeneral _dBServicesGeneral;
        private readonly IUseCaseGetDerivacion _useCaseGetDerivacion;
        public DerivacionController(
            DBServicesDerivacion dBServicesDerivacion,
            DBServicesConsultasSupervisores dBServicesConsultasSupervisores,
            DBServicesGeneral dBServicesGeneral,
            DBServicesConsultasAdministrador dBServicesConsultasAdministrador,
            IUseCaseGetDerivacion useCaseGetDerivacion)
        {
            _dBServicesDerivacion = dBServicesDerivacion;
            _dBServicesConsultasSupervisores = dBServicesConsultasSupervisores;
            _dBServicesGeneral = dBServicesGeneral;
            _dBServicesConsultasAdministrador = dBServicesConsultasAdministrador;
            _useCaseGetDerivacion = useCaseGetDerivacion;
        }

        [HttpGet]
        [PermissionAuthorization("Derivacion", "Derivacion")]
        public async Task<IActionResult> Derivacion()
        {
            var UsuarioIdSupervisor = HttpContext.Session.GetInt32("UsuarioId");
            if (UsuarioIdSupervisor == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión.";
                return RedirectToAction("Redireccionar", "Error");
            }
            var rolUsuario = HttpContext.Session.GetInt32("RolUser");
            if (rolUsuario == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión.";
                return RedirectToAction("Redireccionar", "Error");
            }
            var execute = await _useCaseGetDerivacion.Execute(UsuarioIdSupervisor.Value, rolUsuario.Value);
            if (!execute.success)
            {
                TempData["MessageError"] = execute.message;
                return RedirectToAction("Redireccionar", "Error");
            }
            var viewDerivaciones = execute.data;
            return View("Derivacion", viewDerivaciones);
        }
        
        public async Task<IActionResult> ObtenerDerivacionesGestion(string DniAsesor)
        {
            try
            {
                var enviarDni = new List<Usuario>();
                var Asesor = new Usuario
                {
                    Dni = DniAsesor.ToString()
                };
                enviarDni.Add(Asesor);
                var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetEntradasBSDialXSupervisor(enviarDni);
                if (!getClientesDerivadosGenerales.IsSuccess)
                {
                    return Json(new { success = false, message = getClientesDerivadosGenerales.Message });
                }
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                if (getClientesDerivadosGenerales.Data != null)
                {
                    foreach (var item in getClientesDerivadosGenerales.Data)
                    {
                        var newItem = new GestionDetalleDTO
                        {
                            IdFeedback = item.IdFeedback,
                            IdAsignacion = item.IdAsignacion,
                            CodCanal = item.CodCanal,
                            Canal = item.Canal,
                            DocCliente = item.DocCliente,
                            FechaEnvio = item.FechaEnvio,
                            FechaGestion = item.FechaGestion,
                            HoraGestion = item.HoraGestion,
                            Telefono = item.Telefono,
                            OrigenTelefono = item.OrigenTelefono,
                            CodCampaña = item.CodCampaña,
                            CodTip = item.CodTip,
                            Oferta = item.Oferta,
                            DocAsesor = item.DocAsesor,
                            Origen = item.Origen,
                            ArchivoOrigen = item.ArchivoOrigen,
                            FechaCarga = item.FechaCarga,
                            IdDerivacion = item.IdDerivacion,
                            IdSupervisor = item.IdSupervisor,
                            Supervisor = item.Supervisor,
                            TraidoDe = "DETALLES DE GESTION",
                            EstadoDerivacion = "DERIVACION POR SISTEMA",
                            FueProcesadaLaDerivacion = true,
                        };
                        getClientesDatosDTO.Add(newItem);
                    }
                }
                getClientesDatosDTO = getClientesDatosDTO.OrderByDescending(a => a.FechaGestion).ToList();
                return PartialView("_DerivacionesGestion", getClientesDatosDTO);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerDerivacionesGestionSupervisor(string? DniAsesor = null)
        {
            try
            {
                var UsuarioIdSupervisor = HttpContext.Session.GetInt32("UsuarioId");
                if (UsuarioIdSupervisor == null)
                {
                    return Json (new { success = false, message = "No se ha iniciado sesión." });
                }
                var enviarDni = new List<Usuario>();
                var Asesor = new Usuario ();
                
                if (DniAsesor == null)
                {
                    var getAsesoresAsignados = await _dBServicesConsultasSupervisores.GetAsesorsFromSupervisor(UsuarioIdSupervisor.Value);
                    if (!getAsesoresAsignados.IsSuccess || getAsesoresAsignados.Data == null)
                    {
                        return Json (new { success = false, message = getAsesoresAsignados.Message });
                    }
                    enviarDni = getAsesoresAsignados.Data;
                }
                else
                {
                    Asesor.Dni = DniAsesor.ToString();
                    enviarDni.Add(Asesor);
                }

                var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetEntradasBSDialXSupervisor(enviarDni);
                if (!getClientesDerivadosGenerales.IsSuccess)
                {
                    return Json(new { success = false, message = getClientesDerivadosGenerales.Message });
                }
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                if (getClientesDerivadosGenerales.Data != null)
                {
                    foreach (var item in getClientesDerivadosGenerales.Data)
                    {
                        var newItem = new GestionDetalleDTO
                        {
                            IdFeedback = item.IdFeedback,
                            IdAsignacion = item.IdAsignacion,
                            CodCanal = item.CodCanal,
                            Canal = item.Canal,
                            DocCliente = item.DocCliente,
                            FechaEnvio = item.FechaEnvio,
                            FechaGestion = item.FechaGestion,
                            HoraGestion = item.HoraGestion,
                            Telefono = item.Telefono,
                            OrigenTelefono = item.OrigenTelefono,
                            CodCampaña = item.CodCampaña,
                            CodTip = item.CodTip,
                            Oferta = item.Oferta,
                            DocAsesor = item.DocAsesor,
                            Origen = item.Origen,
                            ArchivoOrigen = item.ArchivoOrigen,
                            FechaCarga = item.FechaCarga,
                            IdDerivacion = item.IdDerivacion,
                            IdSupervisor = item.IdSupervisor,
                            Supervisor = item.Supervisor,
                            TraidoDe = "DETALLES DE GESTION",
                            EstadoDerivacion = "DERIVACION POR SISTEMA",
                            FueProcesadaLaDerivacion = true,
                        };
                        getClientesDatosDTO.Add(newItem);
                    }
                }
                getClientesDatosDTO = getClientesDatosDTO.OrderByDescending(a => a.FechaGestion).ToList();
                return PartialView("_DerivacionesGestion", getClientesDatosDTO);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}