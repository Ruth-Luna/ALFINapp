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

                var mensajereal = $@"
                
                <div>
                    <div style=""font-size: 12px;"">
                        <span>
                            Estimados <br> Buen día
                        </span>
                    </div>

                    <p style=""margin-top: 20px;"">
                        Desde el <strong style=""color:rgb(0, 91, 182);"">CANAL DE A365</strong>, originamos y compartimos un prospecto de cliente <br> 
                        interesado en la toma de un crédito en efectivo.
                    </p>

                    <div style=""margin-top: 30px;"">
                        <span style=""background-color: yellow; padding: 10px; border-radius: 5px; 
                                    font-family: Segoe UI, Tahoma, Geneva, Verdana, sans-serif; font-size: 24px;"">
                            <strong>Información del Prospecto del Cliente</strong>
                        </span>
                    </div>

                    <div style=""margin-top: 40px; font-family: 'Courier New', Courier, monospace;"">
                        <table>
                            <tr>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>CANAL TELECAMPO:</strong></td>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);"">A365</td>
                            </tr>
                            <tr>
                                <td style=""padding: 12px; background-color: rgb(226, 226, 226);""><strong>CODIGO DEL EJECUTIVO: </strong></td>
                                <td style=""padding: 12px; background-color: rgb(226, 226, 226);"">{getReferido.Data.DniAsesor}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 12px; background-color: rgb(226, 226, 226);""><strong>CDV ALFIN BANCO: </strong></td>
                                <td style=""padding: 12px; background-color: rgb(226, 226, 226);"">{getReferido.Data.NombreCompletoAsesor}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px;""><strong>DNI Cliente:</strong></td>
                                <td style=""padding: 12px;"">{getReferido.Data.DniCliente}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 12px;"">Nombre Cliente:</td>
                                <td style=""padding: 12px;"">{getReferido.Data.NombreCompletoCliente}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 12px;"">Monto Solicitado (S/.):</td>
                                <td style=""padding: 12px;"">{getReferido.Data.OfertaEnviada}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 12px;"">Celular:</td>
                                <td style=""padding: 12px;"">{getReferido.Data.Telefono}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 12px; background-color: rgb(226, 226, 226);""><strong>Agencia de Atencion: </strong></td>
                                <td style=""padding: 12px; background-color: yellow; font-weight: bold;"">{getReferido.Data.Agencia}</td>
                            </tr>
                            <tr style=""background-color: #f2f2f2;"">
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>Fecha de Visita a Agencia: </strong></td>
                                <td style=""padding: 12px; background-color: rgb(226, 226, 226);"">{getReferido.Data.FechaVisita:yyyy-MM-dd}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>Hora de Visita a Agencia: </strong></td>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);""> HORARIO DE AGENCIA </td>
                            </tr>
                        </table>
                    </div>
                </div>
                
                ";

                var enviarEmailDerivacion = await _dBServicesDerivacion.EnviarEmailDeDerivacion(getReferido.Data.Agencia, mensajereal, $"Asunto: Fwd: A365 FFVV CAMPO CLIENTE DNI: {getReferido.Data.DniCliente} / NOMBRE: {getReferido.Data.NombreCompletoCliente}");
                if (enviarEmailDerivacion.IsSuccess == false)
                {
                    return Json(new { success = false, message = enviarEmailDerivacion.message });
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