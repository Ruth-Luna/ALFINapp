using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Entities;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.API.Models
{
    public class ViewInicioVendedor
    {
        public List<Cliente>? ClientesA365 { get; set; }
        public Usuario? Vendedor { get; set; }
        public List<Cliente>? ClientesAlfin { get; set; }
        public int clientesPendientes { get; set; }
        public int clientesTipificados { get; set; }
        public int clientesTotal { get; set; }
    }
}