using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class PermisosRolesVistas
    {
        [Key]
        [Column("id_permiso")]
        public int IdPermiso { get; set; }
        [Column("id_rol")]
        public int IdRol { get; set; }
        [Column("id_vista")]
        public int IdVista { get; set; }
    }
}