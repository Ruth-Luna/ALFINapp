namespace ALFINapp.DTOs
{
    public class DtoVUploadFiles
    {
        public string? fileName { get; set; }
        public string? fileType { get; set; }
        public string? fileContent { get; set; } // Base64 encoded content or varbinary string for bd
        public int idDerivacion { get; set; } = 0; // Optional, used for derivations
        public int type { get; set; } = 0; // 0: Derivacion, 1: Evidencia
    }
}