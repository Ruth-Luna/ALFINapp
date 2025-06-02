namespace ALFINapp.API.Models
{
    public class ViewClienteDetalles
    {
        public int? IdBase { get; set; }
        public string? Dni { get; set; }
        public string? ColorFinal { get; set; }
        public string? Color { get; set; }
        public string? Campaña { get; set; }
        public decimal? OfertaMax { get; set; }
        public int? Plazo { get; set; }
        public decimal? CapacidadMax { get; set; }
        public decimal? SaldoDiferencialReeng { get; set; }
        public string? ClienteNuevo { get; set; }
        public string? Deuda1 { get; set; }
        public string? Entidad1 { get; set; }
        public decimal? Tasa1 { get; set; } // Tasa1
        public decimal? Tasa2 { get; set; } // Tasa2
        public decimal? Tasa3 { get; set; }
        public decimal? Tasa4 { get; set; }
        public decimal? Tasa5 { get; set; }
        public decimal? Tasa6 { get; set; }
        public decimal? Tasa7 { get; set; } // Tasa7
        public string? GrupoTasa { get; set; }
        public string? Usuario { get; set; }
        public string? SegmentoUser { get; set; }
        public string? TraidoDe { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Nombres { get; set; }
        public string? UserV3 { get; set; }
        public int? FlagDeudaVOferta { get; set; }
        public string? PerfilRo { get; set; }
    }
}