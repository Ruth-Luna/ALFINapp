using ALFINapp.API.DTOs;
using ALFINapp.Application.Interfaces.Consulta;
using ALFINapp.Application.Interfaces.Referidos;
using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    public class ReferidoController : Controller
    {
        public DBServicesGeneral _dbServicesGeneral;
        public DBServicesReferido _dbServicesReferido;
        public IUseCaseConsultaClienteDni _useCaseConsultaClienteDni;
        public IUseCaseReferirCliente _useCaseReferirCliente;
        public IRepositoryMiscellaneous _repositoryMiscellaneous;
        public ReferidoController(
            DBServicesGeneral dbServicesGeneral,
            DBServicesReferido dbServicesReferido,
            IUseCaseConsultaClienteDni useCaseConsultaClienteDni,
            IRepositoryMiscellaneous repositoryMiscellaneous,
            IUseCaseReferirCliente useCaseReferirCliente)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesReferido = dbServicesReferido;
            _useCaseConsultaClienteDni = useCaseConsultaClienteDni;
            _repositoryMiscellaneous = repositoryMiscellaneous;
            this._useCaseReferirCliente = useCaseReferirCliente;
        }
        [HttpGet]
        public IActionResult Referido()
        {
            return View("Referido");
        }
        [HttpGet]
        public async Task<IActionResult> BuscarDNIReferido(string dniBusqueda)
        {
            var getDniReferido = await _useCaseConsultaClienteDni.Execute(dniBusqueda);
            var getBases = await _repositoryMiscellaneous.GetUAgenciasConNumeros();

            if (getDniReferido.IsSuccess == false || getDniReferido.Data == null)
            {
                return Json(new { success = false, message = getDniReferido.Message });
            }
            ViewData["Agencias"] = getBases;
            return PartialView("_Detalle", getDniReferido.Data);
        }

        public async Task<IActionResult> ReferirCliente(DtoVReferirCliente cliente)
        {
            var clienteEnt = new Cliente 
            {
                Dni = cliente.dni_cliente,
                FuenteBase = cliente.fuente_base,
                NombresCompletosV = cliente.nombres_vendedor + " " + cliente.apellidos_vendedor,
                DniVendedor = cliente.dni_vendedor,
                Telefono = cliente.telefono,
                AgenciaComercial = cliente.agencia,
                FechaVisita = cliente.fecha_visita,
                XNombre = cliente.nombres_clientes,
                Correo = cliente.correo,
                Cci = cliente.cci,
                Departamento = cliente.departamento,
                Ubigeo = cliente.ubigeo,
                Banco = cliente.banco
            };
            var getReferido = await _useCaseConsultaClienteDni.Execute(cliente.dni_cliente);
            if (getReferido.IsSuccess == false || getReferido.Data == null)
            {
                return Json(new { success = false, message = getReferido.Message });
            }

            var referirCliente = await _useCaseReferirCliente.Execute(clienteEnt);
            if (referirCliente.IsSuccess == false)
            {
                return Json(new { success = false, message = referirCliente.Message });
            }

            var mandarReferido = await _dbServicesReferido.GuardarClienteReferido(
                cliente.dni_cliente ?? string.Empty, 
                cliente.fuente_base,
                cliente.nombres_vendedor,
                cliente.apellidos_vendedor,
                cliente.dni_vendedor,
                cliente.telefono, 
                cliente.agencia, 
                cliente.fecha_visita,
                cliente.nombres_clientes, 
                getReferido.Data.OfertaMax ?? 0,
                cliente.telefono,
                cliente.correo,
                cliente.cci,
                cliente.departamento,
                cliente.ubigeo,
                cliente.banco);

            if (!mandarReferido.Item1)
            {
                return Json(new { success = false, message = mandarReferido.Item2 }); // Segundo valor de la tupla (string Message)
            }
            var mensaje = $@"
            <h2>REFERIDOS</h2>
            <table>
                <tr>
                    <td>CANAL TELECAMPO</td>
                    <td>A365</td>
                </tr>
                <tr>
                    <td>CODIGO EJECUTIVO</td>
                    <td>{cliente.dni_vendedor}</td>
                </tr>
                <tr>
                    <td>CDV ALFINBANCO</td>
                    <td>{cliente.nombres_vendedor} {cliente.apellidos_vendedor}</td>
                </tr>
                <tr>
                    <td>DNI CLIENTE</td>
                    <td>{cliente.dni_cliente}</td>
                </tr>
                <tr>
                    <td>NOMBRE CLIENTE</td>
                    <td>{cliente.nombres_clientes}</td>
                </tr>
                <tr>
                    <td>MONTO SOLICITADO</td>
                    <td>{getReferido.Data.OfertaMax}</td>
                </tr>
                <tr>
                    <td>CELULAR</td>
                    <td>{cliente.telefono}</td>
                </tr>
                <tr>
                    <td>AGENCIA</td>
                    <td>{cliente.agencia}</td>
                </tr>
                <tr>
                    <td>FECHA DE VISITA A AGENCIA</td>
                    <td>{cliente.fecha_visita}</td>
                </tr>
                <tr>
                    <td>HORA DE VISITA A AGENCIA</td>
                    <td>NO ESPECIFICADO</td>
                </tr>
            </table>";

            var enviarCorreo = await _dbServicesReferido.EnviarCorreoReferido(
                                                                        "rperaltam@grupoa365.com.pe",
                                                                        mensaje,
                                                                        $"REFERIDOS - SISTEMA ALFIN {DateTime.Now.ToString("dd/MM/yyyy")}");

            if (enviarCorreo.IsSuccess == false)
            {
                return Json(new { success = false, message = enviarCorreo.Message });
            }

            return Json(new { success = true, message = getReferido.Message + ". " + enviarCorreo.Message });
        }

        public IActionResult Consulta()
        {
            return View("Consulta");
        }

        public async Task<IActionResult> BuscarReferidosDeDNI(string DNI)
        {
            var getDNIConsulta = await _dbServicesReferido.GetReferidosDelDNI(DNI);

            if (getDNIConsulta.IsSuccess == false)
            {
                return Json(new { success = false, message = getDNIConsulta.Message });
            }

            return PartialView("_BuscarReferidoDeDNI", getDNIConsulta.Data);
        }
    }
}