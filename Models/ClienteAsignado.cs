using System.ComponentModel.DataAnnotations;

namespace ALFINapp.Models;
public class ClienteAsignado
{
    [Key]
    public int? id_asignacion { get; set; }
    public int? id_cliente { get; set; } // Relación con la tabla BaseCliente
    public int? id_usuario { get; set; } // Relación con el usuario
    public DateTime? fecha_asignacion { get; set; }
    public string? origen { get; set; }
}
