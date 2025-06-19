using ALFINapp.Domain.Entities;

namespace ALFINapp.Models
{
    public class ViewCruceAsignaciones
    {
        public List<Supervisor> Supervisores { get; set; } = new List<Supervisor>();
    }
    public class CruceSupervisor
    {
        public string? DniSupervisor { get; set; } = string.Empty;
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
    }
}