using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ALFINapp.Infrastructure.Persistence.Models;

public class StringDTO
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string? Cadena { get; set; }
}
