using ALFINapp.API.DTOs;
using ALFINapp.Datos.DAO;
using ALFINapp.Datos.DAO.Miscelaneos;
using ALFINapp.Datos.DAO.Referidos;
using ALFINapp.Domain.Entities;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    public class ReferidoController : Controller
    {
        private readonly DAO_ClientesConsultas _dao_clientesConsultas;
        private readonly DAO_ConsultasMiscelaneas _dao_ConsultasMiscelaneas;
        private readonly DAO_ReferirCliente _dao_ReferirCliente;
        public ReferidoController(
            DAO_ClientesConsultas dao_clientesConsultas,
            DAO_ConsultasMiscelaneas dao_ConsultasMiscelaneas,
            DAO_ReferirCliente dao_ReferirCliente)
        {
            _dao_clientesConsultas = dao_clientesConsultas;
            _dao_ConsultasMiscelaneas = dao_ConsultasMiscelaneas;
            _dao_ReferirCliente = dao_ReferirCliente;
        }
        [HttpGet]
        public IActionResult Referido()
        {
            return View("Referido");
        }
        [HttpGet]
        public async Task<IActionResult> BuscarDNIReferido(string dniBusqueda)
        {
            var getDniReferido = await _dao_clientesConsultas.GetClienteByDniAsync(dniBusqueda);
            var getBases = await _dao_ConsultasMiscelaneas.GetAgencias();

            if (getDniReferido.IsSuccess == false || getDniReferido.Data == null)
            {
                return Json(new { success = false, message = getDniReferido.Message });
            }
            ViewData["Agencias"] = getBases.Agencias;
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

            var referirCliente = await _dao_ReferirCliente.ReferirClienteAsync(clienteEnt, asesorEnt);
            if (referirCliente.isSuccess == false)
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
            var getDNIConsulta = await _dao_ReferirCliente.GetClientesReferidos(DNI);

            if (getDNIConsulta.IsSuccess == false)
            {
                return Json(new { success = false, message = getDNIConsulta.Message });
            }

            return PartialView("_BuscarReferidoDeDNI", getDNIConsulta.Data);
        }
    }
}