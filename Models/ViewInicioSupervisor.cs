using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.API.Models
{
    public class ViewInicioSupervisor
    {
        public List<ViewClienteSupervisor> DetallesClientes { get; set; } = new List<ViewClienteSupervisor>();
        public string nombreSupervisor { get; set; } = string.Empty;
        public int clientesPendientesSupervisor { get; set; }
        public List<string> DestinoBases { get; set; } = new List<string>();
        public int clientesAsignadosSupervisor { get; set; }
        public int totalClientes { get; set; }
    }

    public class ViewClienteSupervisor
    {
        public int? IdAsignacion { get; set; }
        public int? IdCliente { get; set; }
        public int? idUsuarioV { get; set; }
        public DateTime? FechaAsignacionV { get; set; }
        public string? DniVendedor { get; set; }
        //Propiedades de la tabla base_clientes
        public string? Dni { get; set; }
        public string XAppaterno { get; set; } = "";
        public string XApmaterno { get; set; } = "";
        public string XNombre { get; set; } = "";
        //Propiedades de la tabla usuarios
        public string NombresCompletos { get; set; } = "";
        public string? ApellidoPaterno { get; set; }
        public string? UltimaTipificacion { get; set; } = "";
        public string? TipificacionMasRelevante { get; set; } = "";
    }
}