using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesGpieDTO
    {
        public ReportsGPiePorcentajeGestionadosSobreAsignados Reportes { get; set; }
        public ReportsGPiePorcentajeGestionadoDerivadoDesembolsado Reportes2 { get; set; }
        public List<ReportsPieContactabilidadCliente> ReportesGeneral { get; set; }
        public List<ReportsGeneralDatosActualesPorIdUsuarioFecha> ReportesGralTemplate { get; set; }
        public DetallesReportesGpieDTO(
            ReportsGPiePorcentajeGestionadosSobreAsignados? model, 
            ReportsGPiePorcentajeGestionadoDerivadoDesembolsado? model2)
        {
            Reportes = model ?? new ReportsGPiePorcentajeGestionadosSobreAsignados();
            Reportes2 = model2 ?? new ReportsGPiePorcentajeGestionadoDerivadoDesembolsado();
            ReportesGeneral = new List<ReportsPieContactabilidadCliente>();
            ReportesGralTemplate = new List<ReportsGeneralDatosActualesPorIdUsuarioFecha>();
        }
        public DetallesReportesGpieDTO(List<ReportsPieContactabilidadCliente> model)
        {
            Reportes = new ReportsGPiePorcentajeGestionadosSobreAsignados();
            Reportes2 = new ReportsGPiePorcentajeGestionadoDerivadoDesembolsado();
            ReportesGeneral = model;
            ReportesGralTemplate = new List<ReportsGeneralDatosActualesPorIdUsuarioFecha>();
        }
        public DetallesReportesGpieDTO()
        {
            Reportes = new ReportsGPiePorcentajeGestionadosSobreAsignados();
            Reportes2 = new ReportsGPiePorcentajeGestionadoDerivadoDesembolsado();
            ReportesGeneral = new List<ReportsPieContactabilidadCliente>();
            ReportesGralTemplate = new List<ReportsGeneralDatosActualesPorIdUsuarioFecha>();
        }
        public DetallesReportesGpieDTO(List<ReportsGeneralDatosActualesPorIdUsuarioFecha> model)
        {
            Reportes = new ReportsGPiePorcentajeGestionadosSobreAsignados();
            Reportes2 = new ReportsGPiePorcentajeGestionadoDerivadoDesembolsado();
            ReportesGeneral = new List<ReportsPieContactabilidadCliente>();
            ReportesGralTemplate = model;
        }
        public ViewReportePieGeneral toViewPie()
        {
            return new ViewReportePieGeneral
            {
                PERIODO = Reportes.PERIODO,
                TOTAL_ASIGNADOS = Reportes.TOTAL_ASIGNADOS,
                TOTAL_GESTIONADOS = Reportes.TOTAL_GESTIONADOS,
                TOTAL_DERIVADOS = Reportes2.TOTAL_DERIVADOS,
                TOTAL_DESEMBOLSADOS = Reportes2.TOTAL_DESEMBOLSADOS,
                PORCENTAJE_GESTIONADOS = Reportes.PORCENTAJE_GESTIONADOS,
                PORCENTAJE_NO_GESTIONADOS = Reportes.PORCENTAJE_NO_GESTIONADOS,
                PORCENTAJE_DERIVADOS = Reportes2.PORCENTAJE_DERIVADOS,
                PORCENTAJE_DESEMBOLSADOS = Reportes2.PORCENTAJE_DESEMBOLSADOS,
                PORCENTAJE_NO_DERIVADO = Reportes2.PORCENTAJE_NO_DERIVADO
            };
        }
        public List<ViewReportePieGeneral> toViewPieLista()
        {
            return ReportesGeneral.Select(item => new ViewReportePieGeneral
            {
                estado = item.estado,
                total = item.cantidad,
                porcentaje = item.porcentaje,
            }).ToList();
        }

        public ViewReportePieGeneral toViewPie(
            string busqueda, 
            int metasGestion = 0,
            int metasDerivacion = 0,
            int metasDesembolso = 0,
            decimal metasImporte = 0.0m)
        {
            return new ViewReportePieGeneral
            {
                estado = busqueda,
                PERIODO = DateTime.Now.ToString("dd/MM/yyyy"),
                total = ReportesGralTemplate.Count(),
                TOTAL_ASIGNADOS = ReportesGralTemplate.Count(x => x.tiene_asignacion != null),
                TOTAL_GESTIONADOS = ReportesGralTemplate.Count(x => x.cod_tip != null),
                TOTAL_DERIVADOS = ReportesGralTemplate.Count(x => x.tiene_derivacion != null),
                TOTAL_DESEMBOLSADOS = ReportesGralTemplate.Count(x => x.tiene_desembolso != null),
                total_importes = ReportesGralTemplate.Sum(x => x.MONTO_FINANCIADO??0),
                PORCENTAJE_GESTIONADOS = ReportesGralTemplate.Count(x => x.cod_tip != null) * 100 / ReportesGralTemplate.Count(),
                PORCENTAJE_NO_GESTIONADOS = ReportesGralTemplate.Count(x => x.cod_tip == null) * 100 / ReportesGralTemplate.Count(),
                PORCENTAJE_DERIVADOS = ReportesGralTemplate.Count(x => x.tiene_derivacion != null) * 100 / ReportesGralTemplate.Count(),
                PORCENTAJE_DESEMBOLSADOS = ReportesGralTemplate.Count(x => x.tiene_desembolso != null) * 100 / ReportesGralTemplate.Count(),
                PORCENTAJE_NO_DERIVADO = ReportesGralTemplate.Count(x => x.tiene_derivacion == null) * 100 / ReportesGralTemplate.Count(),
                metasGestiones = metasGestion,
                metasDerivaciones = metasDerivacion,
                metasDesembolsos = metasDesembolso,
                metasImporte = metasImporte,
            };
        }
    }
}