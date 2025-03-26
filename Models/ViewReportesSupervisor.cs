using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.API.Models
{
    public class ViewReportesSupervisor
    {
        public ViewUsuario? supervisor { get; set; }
        public List<ViewUsuario>? asesores { get; set; }
        public List<ViewReporteDerivaciones>? derivaciones { get; set; }
        public int? totalDerivaciones { get; set; }
        public int? totalDerivacionesDesembolsadas { get; set; }
        public int? totalDerivacionesNoDesembolsadas { get; set; }
        public int? totalAsignaciones { get; set; }
        public int? totalAsignacionesProcesadas { get; set; }
        public List<ViewTipificacionesAsesor>? tipificacionesAsesores { get; set; }
        public List<ViewTipificacionesCantidad>? tipificacionesCantidad { get; set; }
    }
    public class ViewTipificacionesAsesor
    {
        public string? DniAsesor { get; set; }
        public string? NombreAsesor { get; set; }
        public int? totalAsignaciones { get; set; }
        public int? totalTipificados { get; set; }
        public int? totalDesembolsos { get; set; }
        public int? totalDerivaciones { get; set; }
        public int? totalDerivacionesProcesadas { get; set; }
        public int? totalDerivacionesPendientes { get; set; }
    }
    public class ViewTipificacionesCantidad
    {
        public string? TipoTipificacion { get; set; }
        public int? Cantidad { get; set; }
    }
}