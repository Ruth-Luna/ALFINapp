using ALFINapp.Domain.Entities;

namespace ALFINapp.API.Models
{
    public class ViewGestionLeads
    {
        public List<ViewCliente> ClientesA365 { get; set; } = new List<ViewCliente>();
        public ViewUsuario Vendedor { get; set; } = new ViewUsuario();
        public ViewUsuario Supervisor { get; set; } = new ViewUsuario();
        public List<ViewCliente> ClientesAlfin { get; set; } = new List<ViewCliente>();
        public int clientesPendientes { get; set; }
        public int clientesTipificados { get; set; }
        public int clientesTotal { get; set; }
        public int PaginaActual { get; set; } = 1;
        public string filtro { get; set; } = "";
        public string searchfield { get; set; } = "";
        public string order { get; set; } = "";
        public bool orderAsc { get; set; } = true;
        public List<string> destinoBases { get; set; } = new List<string>();
        public List<string> listasAsignacion { get; set; } = new List<string>();
    }
}