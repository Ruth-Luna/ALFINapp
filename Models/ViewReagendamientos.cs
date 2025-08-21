using ALFINapp.Datos.Persistence.Entities;

namespace ALFINapp.Models
{
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
        public bool PuedeSerReagendado { get; set; } = false;
        public string? NombreAsesor { get; set; } = String.Empty;
        public string? EstadoDerivacion { get; set; } = String.Empty;
        public DateTime? FechaDerivacionOriginal { get; set; } = null;
        public string? DocSupervisor { get; set; } = String.Empty;
        public int NumeroReagendamiento { get; set; } = 0;
        public ViewReagendamientos() { }
        public ViewReagendamientos(ReagendamientosGetReagendamientos entity)
        {
            IdDerivacion = entity.IdDerivacion;
            IdAgendamientosRe = entity.IdAgendamientosRe;
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
            EstadoDerivacion = entity.EstadoDerivacion;
            FechaDerivacionOriginal = entity.FechaDerivacionOriginal;
            DocSupervisor = entity.DocSupervisor;
            NumeroReagendamiento = entity.NumeroReagendamiento;
        }
    }
}