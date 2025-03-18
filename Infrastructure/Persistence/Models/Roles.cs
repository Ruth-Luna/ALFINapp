using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class Roles
    {
        [Key]
        public int IdRol { get; set; }
        public string? Rol { get; set; }
        public string? Descripcion { get; set; }
        public int? Id_Vista_Inicio { get; set; }
    }
}