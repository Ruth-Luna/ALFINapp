using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models
{
    [Table("Vista_Rutas")]
    public class VistaRutas
    {
        [Key]
        [Column("id_vista")]
        public int IdVista { get; set; }

        [Column("nombre_vista")]
        public string? NombreVista { get; set; }

        [Column("ruta_vista")]
        public string? RutaVista { get; set; }

        [Column("es_principal")]
        public bool? EsPrincipal { get; set; }

        [Column("id_vista_padre")]
        public int? IdVistaPadre { get; set; }

        [Column("bi_logo")]
        public string? BiLogo { get; set; }

        [Column("nombre_sidebar")]
        public string? NombreSidebar { get; set; }
    }
}
