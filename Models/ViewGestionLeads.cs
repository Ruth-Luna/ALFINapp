using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;

namespace ALFINapp.API.Models
{
    public class ViewGestionLeads
    {
        public List<Cliente>? ClientesA365 { get; set; }
        public Vendedor? Vendedor { get; set; }
        public List<Cliente>? ClientesAlfin { get; set; }
        public int clientesPendientes { get; set; }
        public int clientesTipificados { get; set; }
        public int clientesTotal { get; set; }
    }
}