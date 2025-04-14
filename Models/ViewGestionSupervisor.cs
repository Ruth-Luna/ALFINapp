namespace ALFINapp.API.Models
{
    public class ViewGestionSupervisor
    {
        public List<ViewClienteSupervisor> DetallesClientes { get; set; } = new List<ViewClienteSupervisor>();
        public string nombreSupervisor { get; set; } = string.Empty;
        public int clientesPendientesSupervisor { get; set; }
        public List<string> DestinoBases { get; set; } = new List<string>();
        public int clientesAsignadosSupervisor { get; set; }
        public int totalClientes { get; set; }
        public ViewUsuario supervisor { get; set; } = new ViewUsuario();
    }
}