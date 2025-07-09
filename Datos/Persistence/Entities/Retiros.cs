using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class Retiros
    {
        [Key]
        [Column("id_retiro")]
        public int IdRetiro { get; set; }
        [Column("dni_retiros")]
        public string? DniRetiros { get; set; }
        [Column("fecha_retiro")]
        public DateTime? FechaRetiro { get; set; }
        [Column("motivo_retiro")]
        public string? MotivoRetiro { get; set; }
    }
}