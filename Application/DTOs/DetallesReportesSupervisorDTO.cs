using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesSupervisorDTO
    {
        public List<ClientesAsignado> ClientesAsignados { get; set; } = new List<ClientesAsignado>();
        public List<Usuario> Asesores { get; set; } = new List<Usuario>();
        public List<DerivacionesAsesores> DerivacionesSupervisor { get; set; } = new List<DerivacionesAsesores>();
        public List<Desembolsos> Desembolsos { get; set; } = new List<Desembolsos>();
        public List<DetallesReportesAsesorDTO> ReportesXAsesor { get; set; } = new List<DetallesReportesAsesorDTO>();
        public List<ClientesTipificado> HistorialTipificaciones { get; set; } = new List<ClientesTipificado>();
    }
}