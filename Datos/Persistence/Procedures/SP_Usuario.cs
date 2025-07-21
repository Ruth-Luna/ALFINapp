using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class SP_Usuario
    {
        [Column("total_clientes")]
        public int TotalClientes { get; set; }
        [Column("total_clientes_asignados")]
        public int TotalClientesAsignados { get; set; }
        [Column("total_clientes_pendientes")]
        public int TotalClientesPendientes { get; set; }
    }

    //public class GestionConseguirTodasLasAsignacionesPorListas
    //{
    //    public int id_lista { get; set; }
    //    public int id_usuario_supervisor { get; set; }
    //    public string? nombre_lista { get; set; }
    //    public string? dni_supervisor { get; set; }
    //    public string? nombre_supervisor { get; set; }
    //    public DateTime? fecha_creacion_lista { get; set; }
    //    public int total_asignaciones { get; set; }
    //    public int asignaciones_gestionadas { get; set; }
    //    public int asignadas_a_asesores { get; set; }
    //}
}