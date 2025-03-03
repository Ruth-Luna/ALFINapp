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
                FechaDerivacion = DateTime.Now,
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
            <div>
                <div style='font-size: 12px;'>
                    <span>
                        Estimados <br> Buen día
                    </span>
                </div>
                <div style='margin-top: 20px;'>
                    Desde el <strong>CANAL DE A365</strong> originamos y compartimos un prospecto de cliente<br>
                    interesado en la toma de un crédito en efectivo.
                </div>

                <div style='margin-top: 30px;'>
                    <span style='background-color: yellow; padding: 10px; border-radius: 5px; font-family: Segoe UI, Tahoma, Geneva, Verdana, sans-serif; font-size: 24px;'>
                        <strong>Información del Prospecto del Cliente</strong>
                    </span>
                </div>

                <div style='margin-top: 40px; font-family: Courier New, Courier, monospace;'>
                    <table border='1' cellspacing='0' cellpadding='5' style='border-collapse: collapse; width: 100%;'>
                        <tr>
                            <td style='padding: 10px; background-color: rgb(226, 226, 226);'><strong>CANAL TELECAMPO:</strong></td>
                            <td style='padding: 10px; background-color: rgb(226, 226, 226);'>A365</td>
                        </tr>
                        <tr>
                            <td style='padding: 10px; background-color: rgb(226, 226, 226);'><strong>CÓDIGO DEL EJECUTIVO:</strong></td>
                            <td style='padding: 10px; background-color: rgb(226, 226, 226);'>{usuarioinfo.Data.Dni}</td>
                        </tr>
                        <tr>
                            <td style='padding: 10px; background-color: rgb(226, 226, 226);'><strong>CDV ALFIN BANCO:</strong></td>
                            <td style='padding: 10px; background-color: rgb(226, 226, 226);'>{usuarioinfo.Data.NombresCompletos}</td>
                        </tr>
                        <tr>
                            <td style='padding: 10px;'><strong>DNI Cliente:</strong></td>
                            <td style='padding: 10px;'>{enviarDerivacion.DniCliente}</td>
                        </tr>
                        <tr>
                            <td style='padding: 10px;'><strong>Nombre Cliente:</strong></td>
                            <td style='padding: 10px;'>{enviarDerivacion.NombreCliente}</td>
                        </tr>
                        <tr>
                            <td style='padding: 10px;'><strong>Monto Solicitado (S/.):</strong></td>
                            <td style='padding: 10px;'>{getDerivacion.data.Oferta}</td>
                        </tr>
                        <tr>
                            <td style='padding: 10px;'><strong>Celular:</strong></td>
                            <td style='padding: 10px;'>{enviarDerivacion.TelefonoCliente}</td>
                        </tr>
                        <tr>
                            <td style='padding: 10px; background-color: rgb(226, 226, 226);'><strong>Agencia de Atención:</strong></td>
                            <td style='padding: 10px; background-color: yellow;'>{enviarDerivacion.NombreAgencia}</td>
                        </tr>
                        <tr>
                            <td style='padding: 10px; background-color: rgb(226, 226, 226);'><strong>Fecha de Visita a Agencia:</strong></td>
                            <td style='padding: 10px; background-color: rgb(226, 226, 226);'>
                                {enviarDerivacion.FechaDerivacion:yyyy-MM-dd}
                            </td>
                        </tr>
                        <tr>
                            <td style='padding: 10px; background-color: rgb(226, 226, 226);'><strong>Hora de Visita a Agencia:</strong></td>
                            <td style='padding: 10px; background-color: rgb(226, 226, 226);'>HORARIO DE AGENCIA</td>
                        </tr>
                    </table>
                </div>
            </div>";

            var enviarEmailDerivacion = await _dBServicesDerivacion.EnviarEmailDeDerivacion("svilcalim@unsa.edu.pe", mensajereal, $"Asunto: Fwd: A365 FFVV CAMPO CLIENTE DNI: {enviarDerivacion.DniCliente} / NOMBRE: {enviarDerivacion.NombreCliente}");
            return Json(new { success = true, message = "La Derivacion se ha enviado correctamente, pero para guardar los cambios debe darle al boton Guardar Tipificaciones. " + enviarEmailDerivacion.message }); ;
        }
    }
}