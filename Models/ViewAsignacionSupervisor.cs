using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Datos.Persistence.Procedures;

namespace ALFINapp.API.Models
{
    public class ViewAsignacionSupervisor
    {
        public List<ViewAsignacionAsesor> Asesores { get; set; } = new List<ViewAsignacionAsesor>();
        public int TotalClientes { get; set; } = 0;
        public int TotalClientesAsignados { get; set; } = 0;
        public int TotalClientesPendientes { get; set; } = 0;
        public List<string> Destinos { get; set; } = new List<string>();
        public List<string> ListasAsignacion { get; set; } = new List<string>();
        public List<string> BasesAsignacion { get; set; } = new List<string>();
    }
    public class ViewAsignacionAsesor
    {
        public string? NombresCompletos { get; set; } = string.Empty;
        public int? IdUsuario { get; set; } = 0;
        public int? NumeroClientes { get; set; } = 0;
        public int? NumeroClientesGestionados { get; set; } = 0;
        public int? NumeroClientesPendientes { get; set; } = 0;
        public bool? estaActivado { get; set; } = false;
        public ViewAsignacionAsesor() { }
        public ViewAsignacionAsesor(SupervisoresGetDetallesAsignacionesPorAsesores model)
        {
            IdUsuario = model.idUsuarioA;
            NombresCompletos = model.nombreUsuarioA;
            NumeroClientes = model.totalClientesAsignados;
            NumeroClientesGestionados = model.totalClientesGestionados;
            NumeroClientesPendientes = model.totalClientesPendientes;
            estaActivado = model.estaActivo == "ACTIVO";
        }
    }
}