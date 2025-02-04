using System;

namespace ALFINapp.Models
{
    public class DerivacionesBSDIALDTO
    {
        public string? COD_CANAL { get; set; }
        public string? CANAL { get; set; }
        public DateTime? FECHA_ENVIO { get; set; }
        public DateTime? FECHA_GESTION { get; set; }
        public TimeSpan? HORA_GESTION { get; set; }
        public string? TELEFONO { get; set; }
        public string? ORIGEN_TELEFONO { get; set; }
        public string? COD_CAMPAÑA { get; set; }
        public int? COD_TIP { get; set; }
        public float? OFERTA { get; set; }
        public string? DNI_ASESOR { get; set; }
        public bool? FILTRO { get; set; }
        public DateTime? DIA_SUBIDA { get; set; }
        public string? ARCHIVO_ORIGEN { get; set; }
        public string? Nombres_Completos { get; set; }
        public int? ID_USUARIO_SUP { get; set; }
        public int? ID_USUARIO { get; set; }
        public string? DNI { get; set; }
        public int? ID_ROL { get; set; }
        public string? ROL { get; set; }
    }
}
