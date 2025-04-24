using ALFINapp.Domain.Entities;

namespace ALFINapp.API.Models
{
    public class ViewDerivacionesVistaGeneral
    {
        public List<Vendedor> Asesores { get; set; } = new List<Vendedor>();
        public List<Supervisor> Supervisores { get; set; } = new List<Supervisor>();
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
        public int IdAsignacion { get; set; } = 0;
        public string ObservacionDerivacion { get; set; } = string.Empty;
        public bool FueEnviadoEmail { get; set; } = false;
        public int IdDesembolso { get; set; } = 0;
        public string DocSupervisor { get; set; } = string.Empty;
        public decimal OfertaMax { get; set; } = 0;
        public string Supervisor { get; set; } = string.Empty;
        public decimal MontoDesembolso { get; set; } = 0;
        public string RealError { get; set; } = string.Empty;
    }
}