using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models
{
    public class FeedGReportes
    {
        [Column("DNI")] 
        public string DNI { get; set; } = null!; // Se usa `null!` para evitar nulos en la clave primaria
        
        [Column("COD_CANAL")]
        public string? COD_CANAL { get; set; }

        [Column("CANAL")]
        public string? CANAL { get; set; }

        [Column("FECHA_ENVIO")]
        public DateTime? FECHA_ENVIO { get; set; }

        [Column("FECHA_GESTION")]
        public DateTime? FECHA_GESTION { get; set; }

        [Column("HORA_GESTION")]
        public TimeSpan? HORA_GESTION { get; set; } // Time en SQL Server se mapea con TimeSpan en C#

        [Column("TELEFONO")]
        public string? TELEFONO { get; set; }

        [Column("ORIGEN_TELEFONO")]
        public string? ORIGEN_TELEFONO { get; set; }

        [Column("COD_CAMPAÑA")]
        public string? COD_CAMPAÑA { get; set; }

        [Column("COD_TIP")]
        public int? COD_TIP { get; set; }

        [Column("OFERTA")]
        public double? OFERTA { get; set; }

        [Column("DNI_ASESOR")]
        public string? DNI_ASESOR { get; set; }

        [Column("FILTRO")]
        public bool FILTRO { get; set; } // `bit` en SQL Server se mapea como `bool` en C#

        [Column("DIA_SUBIDA")]
        public DateTime? DIA_SUBIDA { get; set; }

        [Column("ARCHIVO_ORIGEN")]
        public string? ARCHIVO_ORIGEN { get; set; }
    }
}
