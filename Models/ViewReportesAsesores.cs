using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;

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
        public List<ViewGestionDetalle> gestionDetalles { get; set; } = new List<ViewGestionDetalle>();
        public List<ViewTipificacionesGestion> tipificacionesGestion { get; set; } = new List<ViewTipificacionesGestion>();
    }

    public class ViewTipificacionesGestion
    {
        public int IdTipificacion { get; set; }
        public string DescripcionTipificaciones { get; set; } = string.Empty;
        public int ContadorTipificaciones { get; set; }
    }
}