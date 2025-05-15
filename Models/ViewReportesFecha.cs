namespace ALFINapp.API.Models
{
    public class ViewReportesFecha
    {
        public ViewReportePieGeneral ProgresoGeneral { get; set; } = new ViewReportePieGeneral();
        public List<ViewReporteTablaMeses>? reporteTablaPorMeses { get; set; } = new List<ViewReporteTablaMeses>();
    }
}