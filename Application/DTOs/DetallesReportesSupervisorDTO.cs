using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesSupervisorDTO
    {
        public List<ClientesAsignado> ClientesAsignados { get; set; } = new List<ClientesAsignado>();
        public List<Usuario> Asesores { get; set; } = new List<Usuario>();
        public List<DerivacionesAsesores> DerivacionesSupervisor { get; set; } = new List<DerivacionesAsesores>();
        public List<Desembolsos> Desembolsos { get; set; } = new List<Desembolsos>();
        public List<DetallesReportesAsesorDTO> ReportesXAsesor { get; set; } = new List<DetallesReportesAsesorDTO>();
        public List<GESTIONDETALLE> gESTIONDETALLEs { get; set; } = new List<GESTIONDETALLE>();
        public DetallesReportesSupervisorDTO()
        {
            ClientesAsignados = new List<ClientesAsignado>();
            Asesores = new List<Usuario>();
            DerivacionesSupervisor = new List<DerivacionesAsesores>();
            Desembolsos = new List<Desembolsos>();
            ReportesXAsesor = new List<DetallesReportesAsesorDTO>();
            gESTIONDETALLEs = new List<GESTIONDETALLE>();
        }
        public DetallesReportesSupervisorDTO(List<ClientesAsignado> clientesAsignados, List<Usuario> asesores, List<DerivacionesAsesores> derivacionesSupervisor, List<Desembolsos> desembolsos, List<DetallesReportesAsesorDTO> reportesXAsesor, List<GESTIONDETALLE> gESTIONDETALLEs)
        {
            ClientesAsignados = clientesAsignados;
            Asesores = asesores;
            DerivacionesSupervisor = derivacionesSupervisor;
            Desembolsos = desembolsos;
            ReportesXAsesor = reportesXAsesor;
            this.gESTIONDETALLEs = gESTIONDETALLEs;
        }
        public DetallesReportesSupervisorDTO(
            List<ClientesAsignado> clientesAsignados,
            List<Usuario> asesores,
            List<DerivacionesAsesores> derivacionesSupervisor,
            List<Desembolsos> desembolsos,
            List<DetallesReportesAsesorDTO> reportesXAsesor,
            List<ReportsSupervisorGestionFecha> gESTIONDETALLEs)
        {
            ClientesAsignados = clientesAsignados;
            Asesores = asesores;
            DerivacionesSupervisor = derivacionesSupervisor;
            Desembolsos = desembolsos;
            ReportesXAsesor = reportesXAsesor;
            this.gESTIONDETALLEs = gESTIONDETALLEs.Select(g => new GESTIONDETALLE
            {
                DocCliente = g.DocCliente,
                DocAsesor = g.DocAsesor,
                FechaGestion = g.FechaGestion,
                CodTip = g.CodTip
            }).ToList();
        }
    }
}