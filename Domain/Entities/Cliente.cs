using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Domain.Entities
{
    public class Cliente
    {
        public string? Dni { get; set; }
        public string? XAppaterno { get; set; }
        public string? XApmaterno { get; set; }
        public string? XNombre { get; set; }
        public int? Edad { get; set; }
        public decimal? OfertaMax { get; set; }
        public string? Campaña { get; set; }
        public decimal? Cuota { get; set; }
        public decimal? Oferta12m { get; set; }
        public decimal? Tasa12m { get; set; }
        public decimal? Tasa18m { get; set; }
        public decimal? Cuota18m { get; set; }
        public decimal? Oferta24m { get; set; }
        public decimal? Tasa24m { get; set; }
        public decimal? Cuota24m { get; set; }
        public decimal? Oferta36m { get; set; }
        public decimal? Tasa36m { get; set; }
        public decimal? Cuota36m { get; set; }

        //Detalles relevantes de Bse Cliente
        public string? Departamento { get; set; }
        public string? Provincia { get; set; }
        public string? Distrito { get; set; }

        //Detalles relevantes de DetalleBase

        public string? Sucursal { get; set; }
        public string? AgenciaComercial { get; set; }
        public string? TipoCliente { get; set; }
        public string? ClienteNuevo { get; set; }
        public string? GrupoTasa { get; set; }
        public string? GrupoMonto { get; set; }
        public int? Propension { get; set; }
        public string? PrioridadSistema { get; set; }
        //Ids y demas NO MOSTRABLE
        public int? IdAsignacion { get; set; }
        public int? IdBase { get; set; }

        //Tabla clientes_asignados
        public DateTime? FechaAsignacionVendedor { get; set; }
        public bool FinalizarTipificacion { get; set; }
        public string? ComentarioGeneral { get; set; }
        public string? TipificacionDeMayorPeso { get; set; }
        public int? PesoTipificacionMayor { get; set; }
        public DateTime? FechaTipificacionDeMayorPeso { get; set; }
    }
}