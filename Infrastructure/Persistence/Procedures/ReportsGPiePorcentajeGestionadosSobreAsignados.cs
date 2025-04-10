using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class ReportsGPiePorcentajeGestionadosSobreAsignados
    {
        public string PERIODO { get; set; } = string.Empty;
        public int TOTAL_ASIGNADOS { get; set; }
        public int TOTAL_GESTIONADOS { get; set; }
        public decimal PORCENTAJE_GESTIONADOS { get; set; }
        public decimal PORCENTAJE_NO_GESTIONADOS { get; set; }
    }
}