namespace ALFINapp.Models
{

    public class ViewReagendamientos
    {
        public int IdDerivacion { get; set; } = 0;
        public int IdAgendamientosRe { get; set; } = 0;
        public decimal Oferta { get; set; } = 0;
        public DateTime? FechaVisita { get; set; } = null;
        public string? Telefono { get; set; } = string.Empty;
        public string? Agencia { get; set; } = string.Empty;
        public DateTime? FechaAgendamiento { get; set; } = null;
        public DateTime? FechaDerivacion { get; set; } = null;
        public string? DniAsesor { get; set; } = string.Empty;
        public string? DniCliente { get; set; } = string.Empty;
        public string? NombreCliente { get; set; } = string.Empty;
        public bool PuedeSerReagendado { get; set; } = false;
        public string? NombreAsesor { get; set; } = string.Empty;
        public string? EstadoReagendamiento { get; set; } = string.Empty;
        public DateTime? FechaDerivacionOriginal { get; set; } = null;
        public string? DocSupervisor { get; set; } = string.Empty;
        public int NumeroReagendamiento { get; set; } = 0;
        public int TotalReagendamientos { get; set; } = 0;
    }
}