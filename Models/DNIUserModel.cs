using System.ComponentModel.DataAnnotations;

namespace ALFINapp.Models;

public class DNIUserModel
{
    [Key]
    public int? id_usuario { get; set; }

    [Required]
    public string? dni { get; set; }

    public string? nombre { get; set; }

    public string? rol { get; set; }

    public string? departamento { get; set; }

    public string? provincia { get; set; } // Corresponde a 'provincia' en la tabla

    public string? distrito { get; set; } // Corresponde a 'distrito' en la tabla

    public string? telefono { get; set; } // Corresponde a 'telefono' en la tabla

    public DateTime? fecha_registro { get; set; } // Corresponde a 'fecha_registro' en la tabla

    public string? estado { get; set; }
}