using System.ComponentModel.DataAnnotations.Schema;
namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class SupervisorGetNumberOfLeads
    {
        [Column("total_clientes")]
        public int TotalClientes { get; set; }
        [Column("total_clientes_asignados")]
        public int TotalClientesAsignados { get; set; }
        [Column("total_clientes_pendientes")]
        public int TotalClientesPendientes { get; set; }
    }

    public class GestionConseguirTodasLasAsignacionesPorListas
    {
        public int id_lista { get; set; }
        public int id_usuario_supervisor { get; set; }
        public string? nombre_lista { get; set; }
        public string? dni_supervisor { get; set; }
        public string? nombre_supervisor { get; set; }
        public DateTime? fecha_creacion_lista { get; set; }
        public int total_asignaciones { get; set; }
        public int asignaciones_gestionadas { get; set; }
        public int asignadas_a_asesores { get; set; }
    }
    public class GestionConseguirODescargarAsignacionDeLeadsDeSup
    {
        public int? id_asignacion { get; set; }
        public DateTime? fecha_asignacion_sup { get; set; }
        public int? id_usuarioV { get; set; }
        public string? nombre_lista { get; set; }
        public DateTime? fecha_creacion { get; set; }
        public string? telefono_1 { get; set; }
        public string? telefono_2 { get; set; }
        public string? telefono_3 { get; set; }
        public string? telefono_4 { get; set; }
        public string? telefono_5 { get; set; }
        public string? email_1 { get; set; }
        public string? email_2 { get; set; }
        public string? nombre_completo { get; set; }
        public decimal? oferta_max { get; set; }
        public string? tipo_base { get; set; }
        public string? ultima_tipificacion_general { get; set; }
    }
}