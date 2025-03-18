using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Correos
    {
        [Key]
        public int Id { get; set; }

        [Column("tema")]
        public string? Tema { get; set; }

        [Column("origen")]
        public string? Origen { get; set; }

        [Column("Tipo_Persona")]
        public string? TipoPersona { get; set; }

        [Column("correo")]
        public string? Correo { get; set; }

        [Column("contraseña")]
        public string? Contrasena { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }
        [Column("envio")]
        public string? Envio { get; set; }
        [Column("es_correo_propio")]
        public bool EsCorreoPropio { get; set; }
    }
}