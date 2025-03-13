using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Models
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
    }
}