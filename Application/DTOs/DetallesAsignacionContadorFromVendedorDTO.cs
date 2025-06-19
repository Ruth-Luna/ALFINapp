using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Application.DTOs
{
    public class DetallesAsignacionContadorFromVendedorDTO
    {
        public List<DetalleAsignacionContadorFromVendedorDTO> DetallesAsignacionContadorFromVendedor { get; set; } = new List<DetalleAsignacionContadorFromVendedorDTO>();
        public List<DetalleBaseClienteDTO> DetallesClientesAsignados { get; set; } = new List<DetalleBaseClienteDTO>();
        public List<string> Destinos { get; set; } = new List<string>();
        public List<string> ListasAsignacion { get; set; } = new List<string>();
        public List<string> BasesAsignacion { get; set; } = new List<string>();
    }
    public class DetalleAsignacionContadorFromVendedorDTO
    {
        public string? NombresCompletos { get; set; }
        public int IdUsuario { get; set; }
        public int? NumeroClientes { get; set; }
        public int? NumeroClientesGestionados { get; set; }
        public int? NumeroClientesPendientes { get; set; }
        public bool? estaActivado { get; set; }
    }
}