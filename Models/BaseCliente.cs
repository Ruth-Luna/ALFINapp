using System.ComponentModel.DataAnnotations;

namespace ALFINapp.Models
{
    public class BaseCliente
    {
        [Key]
        public int? id_base { get; set; }
        public string? dni { get; set; }
        public string? x_appaterno { get; set; }
        public string? x_apmaterno { get; set; }
        public string? x_nombre { get; set; }
        public int? edad { get; set; }
        public string? departamento { get; set; }
        public string? provincia { get; set; }
        public string? distrito { get; set; }
    }
}
