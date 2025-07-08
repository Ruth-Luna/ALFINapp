using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class ReportsGLineasGestionVsDerivacionDiaria
    {
        public DateOnly FECHA { get; set; }
        public int GESTIONES { get; set; }
        public int DERIVACIONES { get; set; }
    }
}