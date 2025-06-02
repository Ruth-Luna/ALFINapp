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
}