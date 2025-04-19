using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;

namespace ALFINapp.API.Models
{
    public class ViewGestionLeads
    {
        public List<Cliente> ClientesA365 { get; set; } = new List<Cliente>();
        public Vendedor Vendedor { get; set; } = new Vendedor();
        public Supervisor Supervisor { get; set; } = new Supervisor();
        public List<Cliente> ClientesAlfin { get; set; } = new List<Cliente>();
        public int clientesPendientes { get; set; }
        public int clientesTipificados { get; set; }
        public int clientesTotal { get; set; }
        public int PaginaActual { get; set; } = 1;
        public string filtro { get; set; } = "";
        public string searchfield { get; set; } = "";
        public string order { get; set; } = "";
        public bool orderAsc { get; set; } = true;
        public List<string> destinoBases { get; set; } = new List<string>();
    }
}