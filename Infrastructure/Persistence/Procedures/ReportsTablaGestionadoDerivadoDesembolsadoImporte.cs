using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class ReportsTablaGestionadoDerivadoDesembolsadoImporte
    {
        public string? dni_asesor { get; set; }
        public string? asesor { get; set; }
        public string? supervisor { get; set; }
        public int asignados { get; set; } = 0;
        public int gestionado { get; set; } = 0;
        public int derivado { get; set; } = 0;
        public int desembolsado { get; set; } = 0;
        public decimal Importe_Desembolsado { get; set; } = 0.0m;
    }
}