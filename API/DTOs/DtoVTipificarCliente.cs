namespace ALFINapp.API.DTOs
{
    public class DtoVTipificarCliente
    {
        public string? Telefono { get; set; }
        public int TipificacionId { get; set; }
        public DateTime? FechaVisita { get; set; }
        public string? AgenciaAsignada { get; set; }
    }
}
