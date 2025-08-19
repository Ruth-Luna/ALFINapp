using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.API.Models
{
    public class ViewReportesMetas
    {
        public List<ViewMetas> metas { get; set; } = new List<ViewMetas>();
        public int totalGestiones { get; set; } = 0;
        public decimal totalImporte { get; set; } = 0;
        public int totalDerivaciones { get; set; } = 0;
        public ViewReportePieGeneral pieFechas { get; set; } = new ViewReportePieGeneral();
    }
    public class ViewMetas
    {
        public string dni { get; set; } = string.Empty;
        public string nombresCompletos { get; set; } = string.Empty;
        public int totalDerivaciones { get; set; } = 0;
        public decimal totalImporte { get; set; } = 0;
        public int totalGestion { get; set; } = 0;
        public decimal porcentajeGestiones { get; set; } = 0;
        public decimal porcentajeImporte { get; set; } = 0;
        public decimal porcentajeDerivaciones { get; set; } = 0;
        public int metasGestiones { get; set; } = 0;
        public decimal metasImporte { get; set; } = 0;
        public decimal metasDerivaciones { get; set; } = 0;
        public ViewMetas() { }
        public ViewMetas(ReportsTablasMetas model)
        {
            dni = model.dni ?? string.Empty;
            nombresCompletos = model.nombre_completo ?? string.Empty;
            totalDerivaciones = model.total_derivaciones ?? 0;
            totalImporte = model.total_importe ?? 0;
            totalGestion = model.total_gestion ?? 0;
            porcentajeGestiones = model.porcentaje_gestiones ?? 0;
            porcentajeImporte = model.porcentaje_importe ?? 0;
            porcentajeDerivaciones = model.porcentaje_derivaciones ?? 0;
            metasGestiones = model.metas_gestiones ?? 0;
            metasImporte = model.metas_importe ?? 0;
            metasDerivaciones = model.metas_derivaciones ?? 0;
        }
    }
}