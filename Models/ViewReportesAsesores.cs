using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.API.Models
{
    public class ViewReportesAsesores
    {
        public ViewUsuario asesor { get; set; } = new ViewUsuario();
        public int totalDerivaciones { get; set; }
        public int totalDesembolsos { get; set; }
        public int totalAsignado { get; set; }
        public int totalGestionado { get; set; }
        public int totalSinGestionar { get; set; }
        public List<DerivacionesFecha> derivacionesFecha { get; set; } = new List<DerivacionesFecha>();
        public List<DerivacionesFecha> desembolsosFecha { get; set; } = new List<DerivacionesFecha>();
        public List<ViewTipificacionesGestion> tipificacionesGestion { get; set; } = new List<ViewTipificacionesGestion>();
    }

    public class ViewTipificacionesGestion
    {
        public int IdTipificacion { get; set; }
        public string DescripcionTipificaciones { get; set; } = string.Empty;
        public int ContadorTipificaciones { get; set; }
        public ViewTipificacionesGestion() { }
        public ViewTipificacionesGestion(ReportsAsesorTipificacionesTop model)
        {
            IdTipificacion = model.IdTipificacion;
            DescripcionTipificaciones = model.DescripcionTipificaciones ?? string.Empty;
            ContadorTipificaciones = model.ContadorTipificaciones;
        }
    }
}