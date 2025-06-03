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
            var asesorEnt = new Vendedor
            {
                Dni = cliente.dni_vendedor,
                NombresCompletos = cliente.nombres_vendedor + " " + cliente.apellidos_vendedor,
                Telefono = cliente.telefono,
                Correo = cliente.correo,
                Cci = cliente.cci,
                Departamento = cliente.departamento,
                Ubigeo = cliente.ubigeo,
                Banco = cliente.banco
            };
            
            var referirCliente = await _useCaseReferirCliente.Execute(clienteEnt, asesorEnt);
            if (referirCliente.IsSuccess == false)
            {
                return Json(new { success = false, message = referirCliente.Message });
            }
            return Json(new { success = true, message = referirCliente.Message });
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