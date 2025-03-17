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
    [RequireSession]
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
        [HttpGet]
        [PermissionAuthorization("Referidos", "Referidos")]
        public async Task<IActionResult> Referidos()
        {
            try
            {
                var idUsuarioSupervisor = HttpContext.Session.GetInt32("UsuarioId");
                if (idUsuarioSupervisor == null)
                {
                    TempData["MessageError"] = "No se ha podido obtener el id del usuario supervisor";
                    return RedirectToAction("Index", "Home");
                }

                var getReferidos = await _dbServicesReferido.GetReferidosGeneral();
                if (getReferidos.IsSuccess == false || getReferidos.Data == null)
                {
                    TempData["MessageError"] = getReferidos.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                return View("Referidos", getReferidos.Data);
            }
            catch (System.Exception ex)
            {
                TempData["MessageError"] = ex.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DatosEnviarDerivacion(int IdReferido)
        {
            try
            {
                var referido = await _dbServicesReferido.GetClienteReferidoPorId(IdReferido);
                if (referido.IsSuccess == false || referido.Data == null)
                {
                    return Json(new { success = false, message = referido.Message });
                }
                return PartialView("_DatosEnviarDerivacion", referido.Data);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> EnviarDerivacionPorReferencia(int IdReferido)
        {
            try
            {
                var getReferido = await _dbServicesReferido.GetClienteReferidoPorId(IdReferido);
                if (getReferido.IsSuccess == false || getReferido.Data == null)
                {
                    return Json(new { success = false, message = getReferido.Message });
                }

                var enviarDerivacion = await _dBServicesDerivacion.GenerarDerivacion(
                    getReferido.Data.FechaVisita ?? DateTime.Now,
                    getReferido.Data.Agencia ?? throw new ArgumentNullException(nameof(getReferido.Data.Agencia) + "no puede ser nulo"),
                    getReferido.Data.DniAsesor ?? throw new ArgumentNullException(nameof(getReferido.Data.DniAsesor) + "no puede ser nulo"),
                    getReferido.Data.Telefono ?? throw new ArgumentNullException(nameof(getReferido.Data.Telefono) + "no puede ser nulo"),
                    getReferido.Data.DniCliente ?? throw new ArgumentNullException(nameof(getReferido.Data.DniCliente) + "no puede ser nulo"),
                    getReferido.Data.NombreCompletoCliente ?? throw new ArgumentNullException(nameof(getReferido.Data.NombreCompletoCliente) + "no puede ser nulo"));

                if (enviarDerivacion.IsSuccess == false)
                {
                    return Json(new { success = false, message = enviarDerivacion.Message });
                }
                var verificarDerivacion = await _dBServicesDerivacion.VerificarDerivacionEnviada(getReferido.Data.DniCliente);
                if (verificarDerivacion.IsSuccess == false)
                {
                    return Json(new { success = false, message = verificarDerivacion.Message });
                }
                var modificarEstadoReferido = await _dbServicesReferido.ModificarEstadoReferido(IdReferido);
                if (modificarEstadoReferido.IsSuccess == false)
                {
                    return Json(new { success = false, message = modificarEstadoReferido.Message });
                }

                return Json(new { success = true, message = "Derivacion enviada correctamente, se ha enviado automaticamente el correo de la derivacion a todos los destinatarios correspondientes" });
                //Si alguien lee esto, por favor debes de refactorizar este codigo, aun no es tarde para hacerlo - 2025 =wT
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}