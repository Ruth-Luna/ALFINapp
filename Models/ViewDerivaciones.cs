using ALFINapp.Domain.Entities;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.API.Models
{
    public class ViewDerivacionesVistaGeneral
    {
        public List<ViewUsuario> Asesores { get; set; } = new List<ViewUsuario>();
        public List<ViewUsuario> Supervisores { get; set; } = new List<ViewUsuario>();
        public int RolUsuario { get; set; } = 0;
        public string DniUsuario { get; set; } = string.Empty;
        public List<ViewDerivaciones> Derivaciones { get; set; } = new List<ViewDerivaciones>();
    }
    public class ViewDerivaciones
    {
        public int IdDerivacion { get; set; } = 0;
        public DateTime FechaDerivacion { get; set; } = DateTime.Now;
        public string DniAsesor { get; set; } = string.Empty;
        public string DniCliente { get; set; } = string.Empty;
        public int IdCliente { get; set; } = 0;
        public string NombreCliente { get; set; } = string.Empty;
        public string TelefonoCliente { get; set; } = string.Empty;
        public string NombreAgencia { get; set; } = string.Empty;
        public string NumAgencia { get; set; } = string.Empty;
        public bool FueProcesado { get; set; } = false;
        public DateTime FechaVisita { get; set; } = DateTime.Now;
        public string EstadoDerivacion { get; set; } = string.Empty;
        public string EstadoFormulario { get; set; } = string.Empty;
        public string EstadoCorreo { get; set; } = string.Empty;
        public int IdAsignacion { get; set; } = 0;
        public string ObservacionDerivacion { get; set; } = string.Empty;
        public bool FueEnviadoEmail { get; set; } = false;
        public int IdDesembolso { get; set; } = 0;
        public string DocSupervisor { get; set; } = string.Empty;
        public decimal OfertaMax { get; set; } = 0;
        public string Supervisor { get; set; } = string.Empty;
        public decimal MontoDesembolso { get; set; } = 0;
        public string RealError { get; set; } = string.Empty;
        public bool PuedeSerReagendado { get; set; } = true;
        public DateTime? FechaEvidencia { get; set; } = null;
        public string estadoEvidencia { get; set; } = "No Enviado";
        public bool? HayEvidencia { get; set; } = false;
        public bool? FueDesembolsado { get; set; } = false;
        public DateTime? FechaDesembolsos { get; set; } = null;
        public string DocAsesorDesembolso { get; set; } = string.Empty;
        public string DocSupervisorDesembolso { get; set; } = string.Empty;
        public decimal? MontoDesembolsoFinanciado { get; set; } = null;
        public string acciones { get; set; } = string.Empty;
        public string NombreAsesor { get; set; } = string.Empty;
        public ViewDerivaciones() { }
        public ViewDerivaciones(DerivacionConsultaDerivacionesXAsesorPorDniConReagendacion model)
        {
            IdDerivacion = model.IdDerivacion;
            FechaDerivacion = model.FechaDerivacion;
            DniAsesor = model.DniAsesor ?? string.Empty;
            DniCliente = model.DniCliente ?? string.Empty;
            IdCliente = model.IdCliente ?? 0;
            NombreCliente = model.NombreCliente ?? string.Empty;
            TelefonoCliente = model.TelefonoCliente ?? string.Empty;
            NombreAgencia = model.NombreAgencia ?? string.Empty;
            NumAgencia = model.NumAgencia ?? string.Empty;
            FueProcesado = model.FueProcesado ?? false;
            FechaVisita = model.FechaVisita ?? DateTime.Now;
            EstadoDerivacion = model.EstadoDerivacion ?? string.Empty;
            IdAsignacion = model.IdAsignacion ?? 0;
            ObservacionDerivacion = model.ObservacionDerivacion ?? string.Empty;
            FueEnviadoEmail = model.FueEnviadoEmail ?? false;
            IdDesembolso = model.IdDesembolso ?? 0;
            DocSupervisor = model.DocSupervisor ?? string.Empty;
            OfertaMax = model.OfertaMax ?? 0;
            Supervisor = model.Supervisor ?? string.Empty;
            MontoDesembolso = model.MontoDesembolso ?? 0;
            RealError = model.RealError ?? string.Empty;
            PuedeSerReagendado = model.PuedeSerReagendado ?? true;
            FechaEvidencia = model.FechaEvidencia;
            if (FechaEvidencia.HasValue)
            {
                estadoEvidencia = "Enviado";
            }
            else
            {
                estadoEvidencia = "No Enviado";
            }
            HayEvidencia = model.HayEvidencia ?? false;
        }
    }
}