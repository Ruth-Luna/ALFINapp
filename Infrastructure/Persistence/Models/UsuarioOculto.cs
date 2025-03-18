using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class AsesoresOcultos
    {
        [Column("DNI_VICIDIAL")]
        public string? DniVicidial { get; set; }

        [Column("NOMBRE_REAL_ASESOR")]
        public string? NombreRealAsesor { get; set; }

        [Column("DNI_AL_BANCO")]
        public string? DniAlBanco { get; set; }

        [Column("NOMBRE_CAMBIO")]
        public string? NombreCambio { get; set; }
        [Key]
        [Column("id_asesor_oculto")]
        public int IdAsesorOculto { get; set; }
    }
}