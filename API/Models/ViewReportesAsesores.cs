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
        public int numDerivaciones { get; set; }
        public int numDesembolsos { get; set; }
        public int numClientesAsignados { get; set; }
        public int numClientesTipificados { get; set; }
        public int numClientesNoTipificados { get; set; }
        public List<DerivacionesFecha> derivacionesFecha { get; set; } = new List<DerivacionesFecha>();
    }
}