using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class VistasPorRolDTO
    {
        public int id_vista { get; set; }
        public string? nombre_vista { get; set; }
        public string? ruta_vista { get; set; }
        public bool? es_principal { get; set; }
        public int? id_vista_padre { get; set; }
        public string? Rol { get; set; }
        public string? bi_logo { get; set; }
        public string? nombre_sidebar { get; set; }
        public int? id_rol { get; set; }
    }
}