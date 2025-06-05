using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Domain.Entities
{
    public class Cliente
    {
        public string? Dni { get; set; } = String.Empty;
        public string? XAppaterno { get; set; } = String.Empty;
        public string? XApmaterno { get; set; } = String.Empty;
        public string? XNombre { get; set; } = String.Empty;
        public int? Edad { get; set; } = 0;
        public string? Telefono { get; set; } = String.Empty;
        public string? Correo { get; set; } = String.Empty;
        public string? Cci { get; set; } = String.Empty;
        public string? Ubigeo { get; set; } = String.Empty;
        public string? Banco { get; set; } = String.Empty;
        public decimal? OfertaMax { get; set; } = 0;
        public string? Campaña { get; set; } = String.Empty;
        public DateTime? FechaVisita { get; set; } = null;
        public DateTime? FechaSubido { get; set; } = DateTime.Now;
        public decimal? Cuota { get; set; } = 0;
        public decimal? Oferta12m { get; set; } = 0;
        public decimal? Tasa12m { get; set; } = 0;
        public decimal? Tasa18m { get; set; } = 0;
        public decimal? Cuota18m { get; set; } = 0;
        public decimal? Oferta24m { get; set; } = 0;
        public decimal? Tasa24m { get; set; } = 0;
        public decimal? Cuota24m { get; set; } = 0;
        public decimal? Oferta36m { get; set; } = 0;
        public decimal? Tasa36m { get; set; } = 0;
        public decimal? Cuota36m { get; set; } = 0;
        public string? FuenteBase { get; set; } = String.Empty;
        //Detalles relevantes de Bse Cliente
        public string? Departamento { get; set; } = String.Empty;
        public string? Provincia { get; set; } = String.Empty;
        public string? Distrito { get; set; } = String.Empty;

        //Detalles relevantes de DetalleBase
        public string? Sucursal { get; set; } = String.Empty;
        public string? AgenciaComercial { get; set; } = String.Empty;
        public string? TipoCliente { get; set; } = String.Empty;
        public string? ClienteNuevo { get; set; } = String.Empty;
        public string? GrupoTasa { get; set; } = String.Empty;
        public string? GrupoMonto { get; set; } = String.Empty;
        public int? Propension { get; set; } = 0;
        public string? PrioridadSistema { get; set; } = String.Empty;

        //Ids y demas NO MOSTRABLE
        public int? IdAsignacion { get; set; } = 0;
        public int? IdBase { get; set; } = 0;
        public int? IdCliente { get; set; } = 0;
        public int? IdUsuarioV { get; set; } = 0;

        //Tabla clientes_asignados
        public DateTime? FechaAsignacionVendedor { get; set; } = null;
        public bool FinalizarTipificacion { get; set; } = false;
        public string? ComentarioGeneral { get; set; } = "";
        public string? TipificacionDeMayorPeso { get; set; } = "";
        public string? UltimaTipificacion { get; set; } = "";
        public int? PesoTipificacionMayor { get; set; } = 0;
        public DateTime? FechaTipificacionDeMayorPeso { get; set; } = null;
        public string? TraidoDe { get; set; } = String.Empty; // Indica de dónde se trajo el cliente, por ejemplo, "DBA365" o "DBALFIN"
        
        //DETALLES DEL VENDEDOR ASIGNADO
        public string? DniVendedor { get; set; } = String.Empty;
        public string NombresCompletosV { get; set; } = "";
        public string? ApellidoPaternoV { get; set; } = "";
        
    }
}