namespace ALFINapp.API.DTOs
{
    public class DtoVTipificarCliente
    {
        public string Telefono { get; set; } = string.Empty;
        public int TipificacionId { get; set; }
        public int? idderivacion { get; set; }
    }
}
