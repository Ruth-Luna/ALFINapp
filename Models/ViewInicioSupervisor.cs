using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;

namespace ALFINapp.API.Models
{
    public class ViewInicioSupervisor
    {
        public Supervisor Supervisor { get; set; } = new Supervisor();
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