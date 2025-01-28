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
    public class ReferidoController : Controller
    {
        public DBServicesGeneral _dbServicesGeneral;
        public DBServicesReferido _dbServicesReferido;
        public ReferidoController( DBServicesGeneral dbServicesGeneral, DBServicesReferido dbServicesReferido)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesReferido = dbServicesReferido;
            
        }
        public async Task<IActionResult> Referido()
        {
            return View("Referido");
        }

        public async Task<IActionResult> BuscarDNIReferido(string dniBusqueda)
        {
            var getDNIReferido = await _dbServicesReferido.GetDataFromDNI(dniBusqueda);
            var getBases = await _dbServicesGeneral.GetAgenciasDisponibles();

            if (getDNIReferido.IsSuccess == false)
            {
                return Json(new { success = false, message = getDNIReferido.Message});
            }

            ViewData["Agencias"] = getBases.data;

            return PartialView("DetalleDNIReferido", getDNIReferido.data);
        }

        public async Task<IActionResult> ReferirCliente(string dniReferir,
                                                                        string fuenteBase,
                                                                        string nombresUsuario,
                                                                        string apellidosUsuario,
                                                                        string nombrescliente,
                                                                        string dniUsuario,
                                                                        string telefono,
                                                                        string agencia,
                                                                        DateTime fechaVisita)
        {
            var mandarReferido = await _dbServicesReferido.GuardarClienteReferido(dniReferir, fuenteBase, nombresUsuario, apellidosUsuario, dniUsuario);
            if (mandarReferido.IsSuccess == false)
            {
                //El cliente referido no ha podido ser guardado
                return Json(new { success = false, message = mandarReferido.Message});
            }
            var getReferido = await _dbServicesReferido.GetDataParaReferir(dniReferir);
            if (getReferido.IsSuccess == false || getReferido.Data == null)
            {
                return Json(new { success = false, message = getReferido.Message});
            }
            var mensaje = $@"
            <table>
                <tr>
                    <td>CANAL TELECAMPO</td>
                    <td>A365</td>
                </tr>
                <tr>
                    <td>CODIGO EJECUTIVO</td>
                    <td>{dniUsuario}</td>
                </tr>
                <tr>
                    <td>CDV ALFINBANCO</td>
                    <td>{nombresUsuario} {apellidosUsuario}</td>
                </tr>
                <tr>
                    <td>DNI CLIENTE</td>
                    <td>{dniReferir}</td>
                </tr>
                <tr>
                    <td>NOMBRE CLIENTE</td>
                    <td>{nombrescliente}</td>
                </tr>
                <tr>
                    <td>MONTO SOLICITADO</td>
                    <td>{getReferido.Data?.OfertaMax ?? "No especificada"}</td>
                </tr>
                <tr>
                    <td>CELULAR</td>
                    <td>{telefono}</td>
                </tr>
                <tr>
                    <td>AGENCIA</td>
                    <td>{agencia}</td>
                </tr>
                <tr>
                    <td>FECHA DE VISITA A AGENCIA</td>
                    <td>{fechaVisita}</td>
                </tr>
                <tr>
                    <td>HORA DE VISITA A AGENCIA</td>
                    <td>NO ESPECIFICADO</td>
                </tr>
            </table>";

            var enviarCorreo = await _dbServicesReferido.EnviarCorreoReferido(
                                                                        "weba365.no.reply@gmail.com",
                                                                        mensaje,
                                                                        $"Nuevos Datos {DateTime.Now.ToString("dd/MM/yyyy")}");

            if (enviarCorreo.IsSuccess == false)
            {
                return Json(new { success = false, message = enviarCorreo.Message});
            }

            return Json(new { success = true, message = getReferido.Message + ". " +enviarCorreo.Message});
        }
    }
}