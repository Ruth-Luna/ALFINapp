using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesTablasDTO
    {
        public List <ReportsTablaGestionadoDerivadoDesembolsadoImporte> Reportes { get; set; } = new List<ReportsTablaGestionadoDerivadoDesembolsadoImporte>();
        public List <ReportsTablasMetas> Metas { get; set; } = new List<ReportsTablasMetas>();
        public List <ReportsGPiePorcentajeGestionadoDerivadoDesembolsado> FechasReporte { get; set; } = new List<ReportsGPiePorcentajeGestionadoDerivadoDesembolsado>();
        public DetallesReportesTablasDTO(List<ReportsTablaGestionadoDerivadoDesembolsadoImporte> model)
        {
            Reportes = model;
        }
        public DetallesReportesTablasDTO()
        {
            Reportes = new List<ReportsTablaGestionadoDerivadoDesembolsadoImporte>();
        }
        public DetallesReportesTablasDTO(List<ReportsGPiePorcentajeGestionadoDerivadoDesembolsado> model)
        {
            FechasReporte = model;
        }
        public DetallesReportesTablasDTO(List<ReportsTablasMetas> model)
        {
            Metas = model;
        }
        public List<ViewReporteTablaGeneral> toViewTabla ()
        {
            var reportes = new List<ViewReporteTablaGeneral>();
            foreach (var item in Reportes)
            {
                var reporte = new ViewReporteTablaGeneral();
                reporte.dni = item.dni_asesor;
                reporte.nombres_asesor = item.asesor;
                reporte.nombres_supervisor = item.supervisor;
                reporte.contador_asignados = item.asignados;
                reporte.contador_gestionado = item.gestionado;
                reporte.contador_derivado = item.derivado;
                reporte.contador_desembolsado = item.desembolsado;
                reporte.importe_desembolsado = item.Importe_Desembolsado;
                reporte.porcentaje_desembolsos_derivados = item.desembolsado / (item.derivado == 0 ? 1 : item.derivado) * 100;
                reporte.porcentaje_derivados_asignados = item.derivado / (item.asignados == 0 ? 1 : item.asignados) * 100;
                reporte.porcentaje_derivados_gestionados = item.derivado / (item.gestionado == 0 ? 1 : item.gestionado) * 100;
                reporte.porcentaje_gestionados_asignados = item.gestionado / (item.asignados == 0 ? 1 : item.asignados) * 100;
                reportes.Add(reporte);
            }
            return reportes;
        }
        public List<ViewReporteTablaMeses> toViewTablaMeses()
        {
            var reportes = new List<ViewReporteTablaMeses>();
            foreach (var item in FechasReporte)
            {
                var reporte = new ViewReporteTablaMeses();
                reporte.periodo = item.PERIODO;
                reporte.total_asignados = 0;
                reporte.total_gestionados = item.TOTAL_GESTIONADOS;
                reporte.total_desembolsados = item.TOTAL_DESEMBOLSADOS;
                reporte.porcentaje_derivados = item.PORCENTAJE_DERIVADOS;
                reporte.porcentaje_desembolsados = item.PORCENTAJE_DESEMBOLSADOS;
                reporte.porcentaje_no_derivado = item.PORCENTAJE_NO_DERIVADO;
                reportes.Add(reporte);
            }
            return reportes;
        }
        public List<ViewMetas> toViewMetas()
        {
            var metas = new List<ViewMetas>();
            foreach (var item in Metas)
            {
                var meta = new ViewMetas();
                meta.dni = item.dni ?? string.Empty;
                meta.nombresCompletos = item.nombre_completo ?? string.Empty;
                meta.totalDerivaciones = item.total_derivaciones ?? 0;
                meta.totalImporte = item.total_importe ?? 0;
                meta.totalGestion = item.total_gestion ?? 0;
                meta.porcentajeGestiones = item.porcentaje_gestiones ?? 0;
                meta.porcentajeImporte = item.porcentaje_importe ?? 0;
                meta.porcentajeDerivaciones = item.porcentaje_derivaciones ?? 0;
                meta.metasGestiones = item.metas_gestiones ?? 0;
                meta.metasImporte = item.metas_importe ?? 0;
                meta.metasDerivaciones = item.metas_derivaciones ?? 0;
                metas.Add(meta);
            }
            return metas;
        }

        internal int Sum(Func<object, object> value)
        {
            throw new NotImplementedException();
        }
    }
}