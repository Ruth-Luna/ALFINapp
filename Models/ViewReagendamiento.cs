namespace ALFINapp.API.Models
{
    public class ViewClienteReagendado
    {
        public int IdDerivacion { get; set; }
        public string? Dni { get; set; }
        public string? NombresCompletos { get; set; }
        public decimal? OfertaMax { get; set; }
        public string? AgenciaAsignada { get; set; }
        public string? Telefono { get; set; }
        public DateTime? FechaVisitaPrevia { get; set; }
    }
}