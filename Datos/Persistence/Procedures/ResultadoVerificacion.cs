using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class ResultadoVerificacion
    {
        [Column("mensaje")]
        public string Mensaje { get; set; } = string.Empty;
        [Column("resultado")]
        public int Resultado { get; set; }
    }
}