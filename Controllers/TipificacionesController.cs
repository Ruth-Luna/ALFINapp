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
        private readonly DBServicesConsultasAsesores _dbServicesAsesores;
        private readonly MDbContext _context;
        public TipificacionesController(DBServicesGeneral dbServicesGeneral,
            DBServicesTipificaciones dbServicesTipificaciones,
            DBServicesDerivacion dBServicesDerivacion,
            MDbContext context,
            DBServicesConsultasAsesores dbServicesAsesores)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesTipificaciones = dbServicesTipificaciones;
            _dBServicesDerivacion = dBServicesDerivacion;
            _context = context;
            _dbServicesAsesores = dbServicesAsesores;
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

            var getDerivacion = await _dBServicesDerivacion.GetDerivacionXDNI(enviarDerivacion.DniCliente);
            if (getDerivacion.IsSuccess == false || getDerivacion.data == null)
            {
                return Json(new { success = false, message = getDerivacion.message });
            }
            int? activarCambio = HttpContext.Session.GetInt32("ActivarCambio");
            if (activarCambio != null && activarCambio.Value == 1)
            {
                var usuarioIdCambio = HttpContext.Session.GetInt32("UsuarioIdCambio");
                if (usuarioIdCambio == null)
                {
                    return Json(new { success = false, message = "No se consiguio el id de la sesion, inicie sesion nuevamente." });
                }
                usuarioId = usuarioIdCambio;
            }
            var usuarioinfo = await _dbServicesGeneral.GetUserInformation(usuarioId.Value);
            if (usuarioinfo.IsSuccess == false || usuarioinfo.Data == null)
            {
                return Json(new { success = false, message = usuarioinfo.Message });
            }
            var mensajereal = $@"
            <div>
                <div style=""font-size: 12px;"">
                    <span>
                        Estimados <br> Buen dia
                    </span>
                </div>
                <div style=""margin-top: 20px;"">
                    Desde el <strong>CANAL DE A365</strong> originamos y compratimos un prospecto de cliente <br>
                    interesado en la toma de un credito en efectivo
                </div>

                <div style=""margin-top: 30px;"">
                    <span style=""background-color: yellow; padding: 10px; border-radius: 5px; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size: 24px;"">
                        <strong>Informacion del Prospecto del Cliente</strong>
                    </span>
                </div>
                <div style=""margin-top: 40px; font-family: 'Courier New', Courier, monospace;"">
                    <table>
                        <tr>
                            <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>CANAL TELECAMPO: </strong></td>
                            <td style=""padding: 10px; background-color: rgb(226, 226, 226);"">A365</td>
                        </tr>
                        <tr>
                            <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>CODIGO DEL EJECUTIVO: </strong></td>
                            <td style=""padding: 10px; background-color: rgb(226, 226, 226);"">{usuarioinfo.Data.Dni}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>CDV ALFIN BANCO: </strong></td>
                            <td style=""padding: 10px; background-color: rgb(226, 226, 226);"">{usuarioinfo.Data.NombresCompletos}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 10px;""><strong>DNI cliente: </strong></td>
                            <td style=""padding: 10px;"">{enviarDerivacion.DniCliente}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 10px;""><strong>Nombre Cliente: </strong></td>
                            <td style=""padding: 10px;"">{enviarDerivacion.NombreCliente}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 10px;""><strong>Monto Solicitado (S/.): </strong></td>
                            <td style=""padding: 10px;"">{getDerivacion.data.Oferta}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 10px;""><strong>Celular: </strong></td>
                            <td style=""padding: 10px;"">{enviarDerivacion.TelefonoCliente}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>Agencia de Atencion: </strong></td>
                            <td style=""padding: 10px; background-color: yellow;""> {enviarDerivacion.NombreAgencia} </td>
                        </tr>
                        <tr>
                            <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>Fecha de Visita a Agencia: </strong></td>
                            <td style=""padding: 10px; background-color: rgb(226, 226, 226);""> {enviarDerivacion.FechaVisita:yyyy-MM-dd} </td>
                        </tr>
                        <tr>
                            <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>Hora de Visita a Agencia: </strong></td>
                            <td style=""padding: 10px; background-color: rgb(226, 226, 226);""> HORARIO DE AGENCIA </td>
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

        [HttpPost]
        public async Task<IActionResult> TipificarMotivo(int IdAsignacion, int? Tipificacion_1, int? Tipificacion_2,
                                                int? Tipificacion_3, int? Tipificacion_4, int? Tipificacion_5,
                                                string? Telefono_1, string? Telefono_2, string? Telefono_3,
                                                string? Telefono_4, string? Telefono_5, DateTime? DerivacionDb_1,
                                                DateTime? DerivacionDb_2, DateTime? DerivacionDb_3, DateTime? DerivacionDb_4,
                                                DateTime? DerivacionDb_5, string AgenciaComercialDb_1, string AgenciaComercialDb_2,
                                                string AgenciaComercialDb_3, string AgenciaComercialDb_4, string AgenciaComercialDb_5)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            var ClienteAsignado = await _dbServicesAsesores.ObtenerAsignacion(usuarioId.Value, IdAsignacion);
            if (ClienteAsignado.Data == null || !ClienteAsignado.IsSuccess)
            {
                TempData["MessageError"] = ClienteAsignado.Message;
                return RedirectToAction("Redireccionar", "Error");
            }

            var obtenerDataUsuario = await _dbServicesGeneral.GetUserInformation(usuarioId.Value);
            if (!obtenerDataUsuario.IsSuccess || obtenerDataUsuario.Data == null)
            {
                TempData["MessageError"] = obtenerDataUsuario.Message;
                return RedirectToAction("Redireccionar", "Error");
            }

            var telefonos = new List<string?> { Telefono_1, Telefono_2, Telefono_3, Telefono_4, Telefono_5 };
            var tipificaciones = new List<int?> { Tipificacion_1, Tipificacion_2, Tipificacion_3, Tipificacion_4, Tipificacion_5 };
            var derivaciones = new List<DateTime?> { DerivacionDb_1, DerivacionDb_2, DerivacionDb_3, DerivacionDb_4, DerivacionDb_5 };
            var agenciasComerciales = new List<string> { AgenciaComercialDb_1, AgenciaComercialDb_2, AgenciaComercialDb_3, AgenciaComercialDb_4, AgenciaComercialDb_5 };

            // Fecha de tipificación
            var fechaTipificacion = DateTime.Now;
            string? descripcionTipificacionMayorPeso = null;
            int? pesoMayor = 0;

            var ClientesEnriquecido = await _dbServicesAsesores.ObtenerEnriquecido(ClienteAsignado.Data.IdCliente);
            if (!ClientesEnriquecido.IsSuccess || ClientesEnriquecido.Data == null)
            {
                TempData["MessageError"] = ClientesEnriquecido.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
            var agregado = false;
            int countNonNull = 0;

            for (int i = 0; i < telefonos.Count; i++)
            {
                var telefono = telefonos[i];
                var tipificacion = tipificaciones[i];
                var derivacion = derivaciones[i];

                if (!tipificacion.HasValue)
                {
                    Console.WriteLine("TipificacionId es NULL, no se revisara este campo.");
                    continue; // Salta al siguiente registro sin hacer la inserción
                }

                if (string.IsNullOrEmpty(telefono) || telefono == "0")
                {
                    Console.WriteLine("El Telefono es NULL o 0, este campo debe tener un numero.");
                    TempData["MessageError"] = "Se debe mandar un número de teléfono válido (Se obviaron las inserciones).";
                    return RedirectToAction("Redireccionar", "Error");
                }

                if (derivacion == null && tipificacion == 2)
                {
                    TempData["MessageError"] = "Debe ingresar una fecha de derivación para la tipificación CLIENTE ACEPTO OFERTA DERIVACION (Se obviaron las inserciones).";
                    return RedirectToAction("Redireccionar", "Error");
                }

                if (agenciasComerciales[i] == null && tipificacion == 2)  // Verificamos si el valor no es null
                {
                    TempData["MessageError"] = "Debe ingresar una agencia comercial para la tipificación CLIENTE ACEPTO OFERTA DERIVACION (Se obviaron las inserciones).";
                    return RedirectToAction("Redireccionar", "Error");
                }

                if (tipificacion == 2)
                {
                    var verificarTipificacion = _context.derivaciones_asesores
                        .Where(d => d.DniAsesor == obtenerDataUsuario.Data.Dni
                            && d.IdCliente == ClienteAsignado.Data.IdCliente
                            && d.FechaDerivacion.Year == DateTime.Now.Year
                            && d.FechaDerivacion.Month == DateTime.Now.Month)
                        .ToList();
                    if (verificarTipificacion == null || verificarTipificacion.Count == 0)
                    {
                        TempData["MessageError"] = "No ha enviado la derivacion correspondiente. No se guardara ninguna Tipificacion";
                        return RedirectToAction("Redireccionar", "Error");
                    }
                    if (verificarTipificacion.Count > 1)
                    {
                        TempData["MessageError"] = "Usted ya ha derivado previamente a este cliente durante este mes, para ver su estado puede dirigirse a la pestana de Derivaciones. No se han subido al sistema ninguna de las tipificaciones.";
                        return RedirectToAction("Redireccionar", "Error");
                    }
                    var checkGestionDetalle = _context.GESTION_DETALLE
                        .Where(gd => gd.DocCliente == verificarTipificacion[0].DniCliente
                            && gd.FechaGestion.Year == DateTime.Now.Year
                            && gd.FechaGestion.Month == DateTime.Now.Month
                            && gd.DocAsesor == obtenerDataUsuario.Data.Dni)
                        .ToList();
                    if (checkGestionDetalle.Count != 0)
                    {
                        TempData["MessageError"] = "Usted ya ha derivado previamente a este cliente durante este mes, para ver su estado puede dirigirse a la pestana de Derivaciones. No se han subido al sistema ninguna de las tipificaciones.";
                        return RedirectToAction("Redireccionar", "Error");
                    }
                }

                if (tipificacion == 2 && derivacion != null && agenciasComerciales[i] != null)
                {
                    countNonNull++;
                }
                agregado = true;
            }

            if (agregado == false)
            {
                TempData["MessageError"] = "No se ha llenado ningun campo o se han llenado incorrectamente.";
                return RedirectToAction("Redireccionar", "Error");
            }

            for (int i = 0; i < telefonos.Count; i++)
            {
                var telefono = telefonos[i];
                var tipificacion = tipificaciones[i];
                var derivacion = derivaciones[i];

                if (!tipificacion.HasValue)
                {
                    Console.WriteLine($"Tipificacion {i + 1} es nulo, se omite la inserción.");
                    continue; // Salta al siguiente registro sin hacer la inserción
                }
                agregado = true;
                var tipificacionInfo = await _dbServicesTipificaciones.ObtenerTipificacion(tipificacion.Value);

                if (!tipificacionInfo.IsSuccess || tipificacionInfo.Data == null)
                {
                    TempData["MessageError"] = tipificacionInfo.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                // Actualizar tipificación de mayor peso si aplica
                if (tipificacionInfo.Data.Peso > pesoMayor)
                {
                    pesoMayor = tipificacionInfo.Data.Peso;
                    descripcionTipificacionMayorPeso = tipificacionInfo.Data.DescripcionTipificacion;
                }
                // Agregar un nuevo registro
                var nuevoClienteTipificado = new ClientesTipificado
                {
                    IdAsignacion = IdAsignacion,
                    IdTipificacion = tipificacion.Value,
                    FechaTipificacion = fechaTipificacion,
                    Origen = "nuevo",
                    TelefonoTipificado = telefono,
                    DerivacionFecha = derivacion
                };
                var result = await _dbServicesTipificaciones.GuardarNuevaTipificacion(nuevoClienteTipificado);
                if (!result.IsSuccess)
                {
                    TempData["MessageError"] = result.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }

                // Actualizar última tipificación en clientes_enriquecidos
                var tipificacion_guardada = _context.tipificaciones.FirstOrDefault(t => t.IdTipificacion == tipificacion.Value);
                if (tipificacion_guardada == null)
                {
                    TempData["MessageError"] = "No se ha encontrado la tipificación en la base de datos.";
                    return RedirectToAction("Redireccionar", "Error");
                }
                switch (i + 1)
                {
                    case 1:
                        ClientesEnriquecido.Data.UltimaTipificacionTelefono1 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.Data.FechaUltimaTipificacionTelefono1 = DateTime.Now;
                        ClientesEnriquecido.Data.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 2:
                        ClientesEnriquecido.Data.UltimaTipificacionTelefono2 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.Data.FechaUltimaTipificacionTelefono2 = DateTime.Now;
                        ClientesEnriquecido.Data.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 3:
                        ClientesEnriquecido.Data.UltimaTipificacionTelefono3 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.Data.FechaUltimaTipificacionTelefono3 = DateTime.Now;
                        ClientesEnriquecido.Data.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 4:
                        ClientesEnriquecido.Data.UltimaTipificacionTelefono4 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.Data.FechaUltimaTipificacionTelefono4 = DateTime.Now;
                        ClientesEnriquecido.Data.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 5:
                        ClientesEnriquecido.Data.UltimaTipificacionTelefono5 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.Data.FechaUltimaTipificacionTelefono5 = DateTime.Now;
                        ClientesEnriquecido.Data.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                }
                // Guardar cambios en la base de datos
                _context.clientes_enriquecidos.Update(ClientesEnriquecido.Data);
                await _context.SaveChangesAsync();
                var guardarGestionDetalle = await _dbServicesTipificaciones.GuardarGestionDetalle(ClienteAsignado.Data, nuevoClienteTipificado, ClientesEnriquecido.Data, usuarioId.Value);
                if (!guardarGestionDetalle.IsSuccess)
                {
                    TempData["MessageError"] = guardarGestionDetalle.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
            }
            if (!string.IsNullOrEmpty(descripcionTipificacionMayorPeso) && pesoMayor > (ClienteAsignado.Data.PesoTipificacionMayor ?? 0))
            {
                ClienteAsignado.Data.TipificacionMayorPeso = descripcionTipificacionMayorPeso; // Almacena la descripción
                ClienteAsignado.Data.PesoTipificacionMayor = pesoMayor; // Almacena el peso
                ClienteAsignado.Data.FechaTipificacionMayorPeso = fechaTipificacion; // Almacena la fecha
                _context.clientes_asignados.Update(ClienteAsignado.Data);
                await _context.SaveChangesAsync();
            }
            string message = "No se han encontrado tipificaciones de Derivacion";
            if (countNonNull >= 1)
            {
                message = "Hay una Tipificacion de Derivacion enviada, el formulario fue enviado correctamente.";
            }

            TempData["Message"] = "Las tipificaciones se han guardado correctamente (Se han Obviado los campos Vacios y los campos que fueron llenados con datos incorrectos). " + message;
            return RedirectToAction("Redireccionar", "Error");
        }
    }
}