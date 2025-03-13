using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ALFINapp.Services;
using ALFINapp.Models;
using ALFINapp.Filters; // Replace with the correct namespace where DBServicesGeneral is defined

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class TipificacionesController : Controller
    {

        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesTipificaciones _dbServicesTipificaciones;
        private readonly DBServicesDerivacion _dBServicesDerivacion;
        public TipificacionesController(DBServicesGeneral dbServicesGeneral, DBServicesTipificaciones dbServicesTipificaciones, DBServicesDerivacion dBServicesDerivacion)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesTipificaciones = dbServicesTipificaciones;
            _dBServicesDerivacion = dBServicesDerivacion;
        }

        [HttpPost]
        public async Task<IActionResult> GenerarDerivacion(string agenciaComercial, DateTime FechaVisita, string Telefono, int idBase)
        {
            var getClienteBase = await _dbServicesGeneral.GetBaseClienteFunction(idBase);
            if (getClienteBase.data == null || !getClienteBase.IsSuccess)
            {
                return Json(new { success = false, message = getClienteBase.Message });
            }
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return Json(new { success = false, message = "No se consiguio el id de la sesion, inicie sesion nuevamente." });
            }
            var getAsesor = await _dbServicesGeneral.GetUserInformation(usuarioId.Value);
            if (getAsesor.IsSuccess == false || getAsesor.Data == null)
            {
                return Json(new { success = false, message = getAsesor.Message });
            }

            var getClienteEnriquecido = await _dbServicesGeneral.GetClienteEnriquecidoFunction(idBase);

            if (getClienteEnriquecido.data == null || !getClienteEnriquecido.IsSuccess)
            {
                return Json(new { success = false, message = getClienteEnriquecido.Message });
            }

            var enviarDerivacion = new DerivacionesAsesores
            {
                FechaDerivacion = FechaVisita,
                DniAsesor = getAsesor.Data?.Dni ?? throw new ArgumentNullException("DniAsesor", "DniAsesor cannot be null"),
                DniCliente = getClienteBase.data?.Dni ?? throw new ArgumentNullException("DniCliente", "DniCliente cannot be null"),
                IdCliente = getClienteEnriquecido.data.IdCliente,
                NombreCliente = getClienteBase.data.XNombre + " " + getClienteBase.data.XAppaterno + " " + getClienteBase.data.XApmaterno,
                TelefonoCliente = Telefono,
                NombreAgencia = agenciaComercial,
                FueProcesado = false,
                EstadoDerivacion = "DERIVACION PENDIENTE"
            };
            var enviarFomularioAsignacion = await _dBServicesDerivacion.GenerarDerivacion
                (enviarDerivacion.FechaDerivacion,
                enviarDerivacion.NombreAgencia,
                enviarDerivacion.DniAsesor,
                enviarDerivacion.TelefonoCliente,
                enviarDerivacion.DniCliente,
                enviarDerivacion.NombreCliente);
            if (enviarFomularioAsignacion.IsSuccess == false)
            {
                return Json(new { success = false, message = enviarFomularioAsignacion.Message });
            }
            var derivacionEnviada = await _dBServicesDerivacion.VerificarDerivacionEnviada(enviarDerivacion.DniCliente);
            if (derivacionEnviada.IsSuccess == false)
            {
                return Json(new { success = false, message = derivacionEnviada.Message });
            }

            var usuarioinfo = await _dbServicesGeneral.GetUserInformation(usuarioId.Value);
            if (usuarioinfo.IsSuccess == false || usuarioinfo.Data == null)
            {
                return Json(new { success = false, message = usuarioinfo.Message });
            }
            var getDerivacion = await _dBServicesDerivacion.GetDerivacionXDNI(enviarDerivacion.DniCliente);
            if (getDerivacion.IsSuccess == false || getDerivacion.data == null)
            {
                return Json(new { success = false, message = getDerivacion.message });
            }
            var mensajereal = $@"
            <div style=""font-family: 'Arial', sans-serif; color: #333;"">
                <p style=""font-size: 16px; margin: 0;"">
                    <strong>Estimados,</strong><br>
                    Buen día.
                </p>

                <p style=""margin-top: 20px; font-size: 16px;"">
                    Desde el <strong style=""color: #0073e6;"">CANAL DE A365</strong>, originamos y compartimos un prospecto de cliente interesado en la toma de un crédito en efectivo.
                </p>

                <div style=""margin-top: 30px; text-align: center;"">
                    <span style=""background-color: #ffcc00; padding: 15px 25px; border-radius: 8px; font-size: 20px; font-weight: bold; display: inline-block;"">
                        Información del Prospecto del Cliente
                    </span>
                </div>

                <div style=""margin-top: 40px;"">
                    <table style=""width: 100%; border-collapse: collapse; font-size: 16px; box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);"">
                        <tr style=""background-color: #0073e6; color: white; text-align: left;"">
                            <th style=""padding: 12px;"">Campo</th>
                            <th style=""padding: 12px;"">Información</th>
                        </tr>
                        <tr style=""background-color: #f2f2f2;"">
                            <td style=""padding: 12px; font-weight: bold;"">CANAL TELECAMPO:</td>
                            <td style=""padding: 12px;"">A365</td>
                        </tr>
                        <tr>
                            <td style=""padding: 12px; font-weight: bold;"">CÓDIGO DEL EJECUTIVO:</td>
                            <td style=""padding: 12px;"">{usuarioinfo.Data.Dni}</td>
                        </tr>
                        <tr style=""background-color: #f2f2f2;"">
                            <td style=""padding: 12px; font-weight: bold;"">CDV ALFIN BANCO:</td>
                            <td style=""padding: 12px;"">{usuarioinfo.Data.NombresCompletos}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 12px; font-weight: bold;"">DNI Cliente:</td>
                            <td style=""padding: 12px;"">{enviarDerivacion.DniCliente}</td>
                        </tr>
                        <tr style=""background-color: #f2f2f2;"">
                            <td style=""padding: 12px; font-weight: bold;"">Nombre Cliente:</td>
                            <td style=""padding: 12px;"">{enviarDerivacion.NombreCliente}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 12px; font-weight: bold;"">Monto Solicitado (S/.):</td>
                            <td style=""padding: 12px;"">{getDerivacion.data.Oferta}</td>
                        </tr>
                        <tr style=""background-color: #f2f2f2;"">
                            <td style=""padding: 12px; font-weight: bold;"">Celular:</td>
                            <td style=""padding: 12px;"">{enviarDerivacion.TelefonoCliente}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 12px; font-weight: bold;"">Agencia de Atención:</td>
                            <td style=""padding: 12px; background-color: #ffcc00; font-weight: bold;"">{enviarDerivacion.NombreAgencia}</td>
                        </tr>
                        <tr style=""background-color: #f2f2f2;"">
                            <td style=""padding: 12px; font-weight: bold;"">Fecha de Visita a Agencia:</td>
                            <td style=""padding: 12px;"">{enviarDerivacion.FechaDerivacion:yyyy-MM-dd}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 12px; font-weight: bold;"">Hora de Visita a Agencia:</td>
                            <td style=""padding: 12px;"">HORARIO DE AGENCIA</td>
                        </tr>
                    </table>
                </div>
            </div>";

            var enviarEmailDerivacion = await _dBServicesDerivacion.EnviarEmailDeDerivacion(agenciaComercial, mensajereal, $"Asunto: Fwd: A365 FFVV CAMPO CLIENTE DNI: {enviarDerivacion.DniCliente} / NOMBRE: {enviarDerivacion.NombreCliente}");
            if (enviarEmailDerivacion.IsSuccess == false)
            {
                return Json(new { success = false, message = enviarEmailDerivacion.message });
            }
            return Json(new { success = true, message = "La Derivacion se ha enviado correctamente, pero para guardar los cambios debe darle al boton Guardar Tipificaciones. " + enviarEmailDerivacion.message }); ;
        }
    }
}