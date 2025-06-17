namespace ALFINapp.Domain.Entities
{
    public class Asignacion
    {
        public string DniCliente { get; set; } = string.Empty;
        public string ClienteNombre { get; set; } = string.Empty;
        public string OfertaMax { get; set; } = string.Empty;
        public string TipoBase { get; set; } = string.Empty;
        public string SupervisorNombre { get; set; } = string.Empty;
        public string NombreLista { get; set; } = string.Empty;
        public string FuenteBase { get; set; } = string.Empty;
        public string? DniAsesor { get; set; } = null;
        public string? AsesorNombre { get; set; } = null;
        public DateTime FechaAsignacionSup { get; set; } = DateTime.MinValue;
        public DateTime? FechaUltimaGestion { get; set; } = null;
        public DateTime? FechaCreacion { get; set; } = null;
        public string? TipUltimaGestion { get; set; } = null;
        public int? IdAsignacion { get; set; } = null;
        public int? IdUsuarioV { get; set; } = null;
        public string Telefono1 { get; set; } = string.Empty;
        public string Telefono2 { get; set; } = string.Empty;
        public string Telefono3 { get; set; } = string.Empty;
        public string Telefono4 { get; set; } = string.Empty;
        public string Telefono5 { get; set; } = string.Empty;
        public string Email1 { get; set; } = string.Empty;
        public string Email2 { get; set; } = string.Empty;
        public string UltimaTipificacionGeneral { get; set; } = string.Empty;
    }
}