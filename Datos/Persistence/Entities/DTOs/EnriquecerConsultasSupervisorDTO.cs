using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Models.DTOs
{
    public class EnriquecerConsultasSupervisorDTO
    {
        public int IdUsuario { get; set; }
        public string? Dni { get; set; }
        public string? NombresCompletos { get; set; }
        public string? Rol { get; set; }

        public List<DerivacionesInfo> derivaciones { get; set; }
        public List<AsignacionesInfo>? AsignacionesInfo { get; set; }
    }

    public class AsignacionesInfo
    {
        public int IdAsignacion { get; set; }
        public List<string>? Tipificaciones { get; set; }
        public List<int>? idTipificaciones { get; set; }
    }
    public class DerivacionesInfo
    {
        public int IdDerivacion { get; set; }
        public string? DniCliente { get; set; }
        public DateTime? FechaDerivacion { get; set; }
    }
}