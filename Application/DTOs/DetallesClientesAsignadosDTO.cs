using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Application.DTOs
{
    public class DetallesClientesAsignadosDTO
    {
        public int IdAsignacion { get; set; }
        public int IdCliente { get; set; }
        public int? IdUsuarioS { get; set; }
        public DateTime? FechaAsignacionSup { get; set; }
        public int? IdUsuarioV { get; set; }
        public DateTime? FechaAsignacionVendedor { get; set; }
        public string? FuenteBase { get; set; }
        public bool FinalizarTipificacion { get; set; }
        public string? ComentarioGeneral { get; set; }
        public string? TipificacionMayorPeso { get; set; }
        public int? PesoTipificacionMayor { get; set; }
        public bool? ClienteDesembolso { get; set; }
        public bool? ClienteRetirado { get; set; }
        public DateTime? FechaTipificacionMayorPeso { get; set; }
        public string? IdentificadorBase { get; set; }
        public string? Destino { get; set; }
    }
}