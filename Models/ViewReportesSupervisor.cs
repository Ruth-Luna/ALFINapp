namespace ALFINapp.API.Models
{
    public class ViewReportesSupervisor
    {
        public ViewUsuario? supervisor { get; set; }
        public List<ViewUsuario>? asesores { get; set; }
        public int? totalGestionado { get; set; }
        public int? totalSinGestionar { get; set; }
        public int? totalDerivado { get; set; }
        public int? totalDesembolsado { get; set; }
        public int? totalAsignaciones { get; set; }
        public List<ViewTipificacionesAsesor>? tipificacionesAsesores { get; set; }
        public List<ViewTipificacionesCantidad>? tipificacionesCantidad { get; set; }
        public List<DerivacionesFecha>? derivacionesFecha { get; set; }
        public List<DerivacionesFecha>? desembolsosFecha { get; set; }
    }
    public class ViewTipificacionesAsesor
    {
        public string? DniAsesor { get; set; }
        public string? NombreAsesor { get; set; }
        public int? totalSinGestionar { get; set; }
        public int? totalGestionado { get; set; }
        public int? totalDesembolsos { get; set; }
        public int? totalDerivaciones { get; set; }
        public int? totalDerivacionesProcesadas { get; set; }
        public int? totalDerivacionesPendientes { get; set; }
    }
    public class ViewTipificacionesCantidad
    {
        public string? TipoTipificacion { get; set; }
        public int? IdTipificacion { get; set; }
        public int? Cantidad { get; set; }
    }
}