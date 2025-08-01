namespace ALFINapp.API.Models
{
    public class ViewClienteReagendado
    {
        public int IdDerivacion { get; set; } = 0;
        public string? Dni { get; set; } = String.Empty;
        public string? NombresCompletos { get; set; } = String.Empty;
        public decimal? OfertaMax { get; set; } = 0;
        public string? AgenciaAsignada { get; set; } = String.Empty;
        public string? Telefono { get; set; } = String.Empty;
        public DateTime? FechaVisitaPrevia { get; set; } = DateTime.MinValue;
    }
}