using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class ListasAsignacion
    {
        [Key]
        [Column("id_lista")]
        public int IdLista { get; set; }
        [Column("nombre_lista")]
        public string? NombreLista { get; set; }
        [Column("id_usuario_sup")]
        public int IdUsuarioSup { get; set; }
        [Column("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }
        [Column("estado")]
        public bool Estado { get; set; } = true;
    }
}