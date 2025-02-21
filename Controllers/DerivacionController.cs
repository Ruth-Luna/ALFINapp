using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Filters;
using ALFINapp.Models;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    [RequireSession]
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
        [PermissionAuthorization("Derivacion", "Derivacion")]
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
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                foreach (var item in getClientesDerivadosGenerales.Data)
                {
                    var getInformation = await _dBServicesDerivacion.GetDerivacionInformation(item);
                    if (!getInformation.IsSuccess)
                    {
                        TempData["MessageError"] = getInformation.Message;
                        return RedirectToAction("Index", "Home");
                    }
                    var newItem = new GestionDetalleDTO
                    {
                        IdAsignacion = item.IdDerivacion,
                        DocCliente = item.DniCliente ?? string.Empty,
                        Canal = "A365",
                        FechaEnvio = item.FechaDerivacion,
                        FechaGestion = item.FechaDerivacion,
                        Telefono = item.TelefonoCliente,
                        OrigenTelefono = "A365",
                        CodTip = 2,
                        DocAsesor = item.DniAsesor,
                        Origen = "A365",
                        ArchivoOrigen = "SISTEMA INTERNO",
                        IdDerivacion = item.IdDerivacion,
                        TraidoDe = "SISTEMA INTERNO",
                        EstadoDerivacion = item.EstadoDerivacion + " - " + item.FueProcesado,
                        TipoDerivacion = "AUTOMATICA",
                        //DATOS QUE DEBEN SER BUSCADOS POR EL ID ASIGNAICON
                        CodCampaña = getInformation.data != null ? getInformation.data.CodCampaña : "NO SE ENCONTRO CAMPAÑA",
                        Oferta = getInformation.data != null ? getInformation.data.Oferta : 0,
                        CodCanal = getInformation.data != null ? getInformation.data.CodCanal : "NO SE ENCONTRO UN CANAL",
                        FechaCarga = getInformation.data != null ? getInformation.data.FechaCarga : DateTime.Now,
                        IdSupervisor = getInformation.data != null ? getInformation.data.IdSupervisor : 0,
                        Supervisor = getInformation.data != null ? getInformation.data.Supervisor : "NO SE ENCONTRO SUPERVISOR",
                        IdDesembolso = getInformation.data != null ? getInformation.data.IdDesembolso : 0,
                        FechaDesembolso = getInformation.data != null ? getInformation.data.FechaDesembolso : null,
                        EstadoDesembolso = getInformation.data != null ? getInformation.data.EstadoDesembolso : "NO SE ENCONTRO ESTADO DE DESEMBOLSO",
                        Observacion = getInformation.data != null ? getInformation.data.Observacion : "NO SE ENCONTRO OBSERVACION"
                    };
                    getClientesDatosDTO.Add(newItem);
                }
                foreach (var item in getclientesDerivadosBSDial.Data)
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
                        IdDesembolso = item.IdDesembolso,
                        TraidoDe = "SISTEMA EXTERNO",
                        EstadoDerivacion = "DERIVADO MANUALMENTE",
                        TipoDerivacion = "MANUAL"
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
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                foreach (var item in getClientesDerivadosGenerales.Data)
                {
                    //var getInformation = await _dBServicesDerivacion.GetDerivacionInformation(item);
                    var getInformation = await _dBServicesDerivacion.GetDerivacionInformation(item);
                    if (!getInformation.IsSuccess)
                    {
                        TempData["MessageError"] = getInformation.Message;
                        return RedirectToAction("Index", "Home");
                    }
                    var newItem = new GestionDetalleDTO
                    {
                        IdAsignacion = item.IdDerivacion,
                        DocCliente = item.DniCliente ?? string.Empty,
                        Canal = "A365",
                        FechaEnvio = item.FechaDerivacion,
                        FechaGestion = item.FechaDerivacion,
                        Telefono = item.TelefonoCliente,
                        OrigenTelefono = "A365",
                        CodTip = 2,
                        DocAsesor = item.DniAsesor,
                        Origen = "A365",
                        ArchivoOrigen = "SISTEMA INTERNO",
                        IdDerivacion = item.IdDerivacion,
                        TraidoDe = "SISTEMA INTERNO",
                        EstadoDerivacion = item.EstadoDerivacion + " - " + item.FueProcesado,
                        TipoDerivacion = "AUTOMATICA",
                        //DATOS QUE DEBEN SER BUSCADOS POR EL ID ASIGNAICON
                        CodCampaña = getInformation.data != null ? getInformation.data.CodCampaña : "NO SE ENCONTRO CAMPAÑA",
                        Oferta = getInformation.data != null ? getInformation.data.Oferta : 0,
                        CodCanal = getInformation.data != null ? getInformation.data.CodCanal : "NO SE ENCONTRO UN CANAL",
                        FechaCarga = getInformation.data != null ? getInformation.data.FechaCarga : DateTime.Now,
                        IdSupervisor = getInformation.data != null ? getInformation.data.IdSupervisor : 0,
                        Supervisor = getInformation.data != null ? getInformation.data.Supervisor : "NO SE ENCONTRO SUPERVISOR",
                        IdDesembolso = getInformation.data != null ? getInformation.data.IdDesembolso : 0,
                        FechaDesembolso = getInformation.data != null ? getInformation.data.FechaDesembolso : null,
                        EstadoDesembolso = getInformation.data != null ? getInformation.data.EstadoDesembolso : "NO SE ENCONTRO ESTADO DE DESEMBOLSO",
                        Observacion = getInformation.data != null ? getInformation.data.Observacion : "NO SE ENCONTRO OBSERVACION"
                    };
                    getClientesDatosDTO.Add(newItem);
                }
                foreach (var item in getDerivaciones.Data)
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
                        IdDesembolso = item.IdDesembolso,
                        TraidoDe = "SISTEMA EXTERNO",
                        EstadoDerivacion = "DERIVADO MANUALMENTE",
                        TipoDerivacion = "MANUAL"
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
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                foreach (var item in getClientesDerivadosGenerales.Data)
                {
                    var getInformation = await _dBServicesDerivacion.GetDerivacionInformation(item);
                    if (!getInformation.IsSuccess)
                    {
                        TempData["MessageError"] = getInformation.Message;
                        return RedirectToAction("Index", "Home");
                    }
                    var newItem = new GestionDetalleDTO
                    {
                        IdAsignacion = item.IdDerivacion,
                        DocCliente = item.DniCliente ?? string.Empty,
                        Canal = "A365",
                        FechaEnvio = item.FechaDerivacion,
                        FechaGestion = item.FechaDerivacion,
                        Telefono = item.TelefonoCliente,
                        OrigenTelefono = "A365",
                        CodTip = 2,
                        DocAsesor = item.DniAsesor,
                        Origen = "A365",
                        ArchivoOrigen = "SISTEMA INTERNO",
                        IdDerivacion = item.IdDerivacion,
                        TraidoDe = "SISTEMA INTERNO",
                        EstadoDerivacion = item.EstadoDerivacion + " - " + item.FueProcesado,
                        TipoDerivacion = "AUTOMATICA",
                        //DATOS QUE DEBEN SER BUSCADOS POR EL ID ASIGNAICON
                        CodCampaña = getInformation.data != null ? getInformation.data.CodCampaña : "NO SE ENCONTRO CAMPAÑA",
                        Oferta = getInformation.data != null ? getInformation.data.Oferta : 0,
                        CodCanal = getInformation.data != null ? getInformation.data.CodCanal : "NO SE ENCONTRO UN CANAL",
                        FechaCarga = getInformation.data != null ? getInformation.data.FechaCarga : DateTime.Now,
                        IdSupervisor = getInformation.data != null ? getInformation.data.IdSupervisor : 0,
                        Supervisor = getInformation.data != null ? getInformation.data.Supervisor : "NO SE ENCONTRO SUPERVISOR",
                        IdDesembolso = getInformation.data != null ? getInformation.data.IdDesembolso : 0,
                        FechaDesembolso = getInformation.data != null ? getInformation.data.FechaDesembolso : null,
                        EstadoDesembolso = getInformation.data != null ? getInformation.data.EstadoDesembolso : "NO SE ENCONTRO ESTADO DE DESEMBOLSO",
                        Observacion = getInformation.data != null ? getInformation.data.Observacion : "NO SE ENCONTRO OBSERVACION"
                    };
                    getClientesDatosDTO.Add(newItem);
                }
                foreach (var item in getclientesDerivadosBSDial.Data)
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
                        IdDesembolso = item.IdDesembolso,
                        TraidoDe = "SISTEMA EXTERNO",
                        EstadoDerivacion = "DERIVADO MANUALMENTE",
                        TipoDerivacion = "MANUAL"
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
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                foreach (var item in getClientesDerivadosGenerales.Data)
                {
                    var newItem = new GestionDetalleDTO
                    {
                        IdAsignacion = item.IdDerivacion,
                        DocCliente = item.DniCliente ?? string.Empty,
                        Canal = "A365",
                        FechaEnvio = item.FechaDerivacion,
                        FechaGestion = item.FechaDerivacion,
                        Telefono = item.TelefonoCliente,
                        OrigenTelefono = "A365",
                        CodTip = 2,
                        DocAsesor = item.DniAsesor,
                        //DATOS QUE DEBEN SER BUSCADOS POR EL ID ASIGNAICON
                        CodCampaña = item.NombreAgencia,
                        Oferta = 0,
                        CodCanal = "DESCONOCIDO",
                        Origen = "A365",
                        ArchivoOrigen = "SISTEMA INTERNO",
                        FechaCarga = item.FechaDerivacion,
                        IdDerivacion = item.IdDerivacion,
                        IdSupervisor = 0,
                        Supervisor = "DESCONOCIDO",
                        IdDesembolso = 0,
                        TraidoDe = "SISTEMA INTERNO",
                        EstadoDerivacion = item.EstadoDerivacion + " - " + item.FueProcesado,
                        TipoDerivacion = "AUTOMATICA"
                    };
                    getClientesDatosDTO.Add(newItem);
                }

                foreach (var item in getDerivaciones.Data)
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
                        IdDesembolso = item.IdDesembolso,
                        TraidoDe = "SISTEMA EXTERNO",
                        EstadoDerivacion = "DERIVADO MANUALMENTE",
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