using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesGpieDTO
    {
        public ReportsGPiePorcentajeGestionadosSobreAsignados Reportes { get; set; }
        public ReportsGPiePorcentajeGestionadoDerivadoDesembolsado Reportes2 { get; set; }
        public DetallesReportesGpieDTO(
            ReportsGPiePorcentajeGestionadosSobreAsignados? model, 
            ReportsGPiePorcentajeGestionadoDerivadoDesembolsado? model2)
        {
            Reportes = model ?? new ReportsGPiePorcentajeGestionadosSobreAsignados();
            Reportes2 = model2 ?? new ReportsGPiePorcentajeGestionadoDerivadoDesembolsado();
        }
        public DetallesReportesGpieDTO()
        {
            Reportes = new ReportsGPiePorcentajeGestionadosSobreAsignados();
            Reportes2 = new ReportsGPiePorcentajeGestionadoDerivadoDesembolsado();
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
    }
}