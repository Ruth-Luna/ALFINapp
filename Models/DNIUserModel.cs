using System.ComponentModel.DataAnnotations;

namespace ALFINapp.Models;

public class DNIUserModel
{
    [Key] // Marca el campo Id como clave primaria
    public int? id_usuario { get; set; } // Corresponde a 'id_usuario' en la tabla

    [Required] // Marca el campo como obligatorio
    public string? dni { get; set; } // Corresponde a 'dni' en la tabla

    public string? nombre { get; set; } // Corresponde a 'nombre' en la tabla

    public string? rol { get; set; } // Corresponde a 'rol' en la tabla

    public string? departamento { get; set; } // Corresponde a 'departamento' en la tabla

    public string? provincia { get; set; } // Corresponde a 'provincia' en la tabla

    public string? distrito { get; set; } // Corresponde a 'distrito' en la tabla

    public string? telefono { get; set; } // Corresponde a 'telefono' en la tabla

    public DateTime? fecha_registro { get; set; } // Corresponde a 'fecha_registro' en la tabla

    public string? estado { get; set; }
}