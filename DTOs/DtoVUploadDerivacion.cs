namespace ALFINapp.DTOs
{
    public class DtoVUploadDerivacion
    {
        public string agencia_comercial { get; set; } = string.Empty;
        public DateTime fecha_visita { get; set; } = DateTime.Now;
        public string telefono { get; set; } = string.Empty;
        public int id_base { get; set; } = 0;
        public int id_asignacion { get; set; } = 0;
        public int? id_usuario { get; set; } = null;
        public int type { get; set; } = 0;
        public string? nombres_completos { get; set; } = string.Empty;
    }
}