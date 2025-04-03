using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.API.Models
{
    public class ViewAsignacionSupervisor
    {
        public List<ViewAsignacionAsesor> Asesores { get; set; } = new List<ViewAsignacionAsesor>();
        public int TotalClientes { get; set; } = 0;
        public int TotalClientesAsignados { get; set; } = 0;
        public int TotalClientesPendientes { get; set; } = 0;
        public List<string> Destinos { get; set; } = new List<string>();
    }
    public class ViewAsignacionAsesor
    {
        public string? NombresCompletos { get; set; }
        public int? IdUsuario { get; set; }
        public int? NumeroClientes { get; set; }
        public int? NumeroClientesGestionados { get; set; }
        public int? NumeroClientesPendientes { get; set; }
        public bool? estaActivado { get; set; }
    }
}