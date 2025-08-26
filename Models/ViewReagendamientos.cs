using ALFINapp.Datos.Persistence.Entities;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Models
{
    public class ViewReagendamientosMain
    {
        // public List<Usuario> asesores { get; set; } = new List<Usuario>();
        // public List<Usuario> supervisores { get; set; } = new List<Usuario>();
        public int rolUsuario { get; set; } = 0;
        public string dniUsuario { get; set; } = string.Empty;
        public List<ViewReagendamientos> reagendamientos { get; set; } = new List<ViewReagendamientos>();
    }
    public class ViewReagendamientos
    {
        public int IdDerivacion { get; set; } = 0;
        public int IdAgendamientosRe { get; set; } = 0;
        public decimal Oferta { get; set; } = 0;
        public DateTime? FechaVisita { get; set; } = null;
        public string? Telefono { get; set; } = String.Empty;
        public string? Agencia { get; set; } = String.Empty;
        public DateTime? FechaAgendamiento { get; set; } = null;
        public DateTime? FechaDerivacion { get; set; } = null;
        public string? DniAsesor { get; set; } = String.Empty;
        public string? DniCliente { get; set; } = String.Empty;
        public string? NombreCliente { get; set; } = String.Empty;
        public bool PuedeSerReagendado { get; set; } = false;
        public string? NombreAsesor { get; set; } = String.Empty;
        public string? EstadoReagendamiento { get; set; } = String.Empty;
        public DateTime? FechaDerivacionOriginal { get; set; } = null;
        public string? DocSupervisor { get; set; } = String.Empty;
        public int NumeroReagendamiento { get; set; } = 0;
        public ViewReagendamientos() { }
        public ViewReagendamientos(ReagendamientosGetReagendamientos entity)
        {
            IdDerivacion = entity.IdDerivacion ?? 0;
            IdAgendamientosRe = entity.IdAgendamientosRe ?? 0;
            Oferta = entity.Oferta;
            FechaVisita = entity.FechaVisita;
            Telefono = entity.Telefono;
            Agencia = entity.Agencia;
            FechaAgendamiento = entity.FechaAgendamiento;
            FechaDerivacion = entity.FechaDerivacion;
            DniAsesor = entity.DniAsesor;
            DniCliente = entity.DniCliente;
            PuedeSerReagendado = entity.PuedeSerReagendado;
            NombreAsesor = entity.NombreAsesor;
            EstadoReagendamiento = entity.EstadoDerivacion;
            FechaDerivacionOriginal = entity.FechaDerivacionOriginal;
            DocSupervisor = entity.DocSupervisor;
            NumeroReagendamiento = entity.NumeroReagendamiento != null ? (int)entity.NumeroReagendamiento : 0;
            NombreCliente = entity.NombreCliente;
        }
    }
}