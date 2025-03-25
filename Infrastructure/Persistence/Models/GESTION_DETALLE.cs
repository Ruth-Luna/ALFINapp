using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class GESTIONDETALLE
    {
        [Key]
        [Column("id_feedback")]
        public int IdFeedback { get; set; }

        [Column("id_asignacion")]
        public int? IdAsignacion { get; set; }

        [Column("cod_canal")]
        public string? CodCanal { get; set; }

        [Column("canal")]
        public string? Canal { get; set; }

        [Required]
        [Column("doc_cliente")]
        public string? DocCliente { get; set; }

        [Required]
        [Column("fecha_envio")]
        public DateTime FechaEnvio { get; set; }

        [Required]
        [Column("fecha_gestion")]
        public DateTime FechaGestion { get; set; }

        [Column("hora_gestion")]
        public TimeSpan? HoraGestion { get; set; }

        [Required]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [Required]
        [Column("origen_telefono")]
        public string? OrigenTelefono { get; set; }

        [Column("cod_campaña")]
        public string? CodCampaña { get; set; }

        [Required]
        [Column("cod_tip")]
        public int CodTip { get; set; }

        [Required]
        [Column("oferta")]
        public decimal Oferta { get; set; }

        [Column("doc_asesor")]
        public string? DocAsesor { get; set; }

        [Column("origen")]
        public string? Origen { get; set; }

        [Column("archivo_origen")]
        public string? ArchivoOrigen { get; set; }

        [Column("fecha_carga")]
        public DateTime? FechaCarga { get; set; }

        [Column("id_derivacion")]
        public int? IdDerivacion { get; set; }

        [Column("id_supervisor")]
        public int? IdSupervisor { get; set; }

        [Column("supervisor")]
        public string? Supervisor { get; set; }
    }
}
