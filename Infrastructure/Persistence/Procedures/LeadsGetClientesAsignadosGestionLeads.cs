using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class LeadsGetClientesAsignadosGestionLeads
    {
        [Column("Dni")]
        public string? Dni { get; set; }
        [Column("XAppaterno")]
        public string? XAppaterno { get; set; }
        [Column("XApmaterno")]
        public string? XApmaterno { get; set; }
        [Column("XNombre")]
        public string? XNombre { get; set; }
        [Column("OfertaMax")]
        public decimal? OfertaMax { get; set; }
        [Column("Campaña")]
        public string? Campaña { get; set; }
        [Column("IdBase")]
        public int IdBase { get; set; }
        [Column("IdAsignacion")]
        public int? IdAsignacion { get; set; }
        [Column("FechaAsignacionVendedor")]
        public DateTime? FechaAsignacionVendedor { get; set; }
        [Column("ComentarioGeneral")]
        public string? ComentarioGeneral { get; set; }
        [Column("TipificacionDeMayorPeso")]
        public string? TipificacionDeMayorPeso { get; set; }
        [Column("PesoTipificacionMayor")]
        public int? PesoTipificacionMayor { get; set; }
        [Column("FechaTipificacionDeMayorPeso")]
        public DateTime? FechaTipificacionDeMayorPeso { get; set; }
        [Column("PrioridadSistema")]
        public string? PrioridadSistema { get; set; }
        [Column("TraidoDe")]
        public string? TraidoDe { get; set; }
    }
}