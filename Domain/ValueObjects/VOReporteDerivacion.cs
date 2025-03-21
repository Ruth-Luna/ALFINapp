using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;

namespace ALFINapp.Domain.ValueObjects
{
    public class VOReporteDerivacion
    {
        public List<VOReporteDerivacionDetalle>? ReporteDerivacionDetalle { get; set; }
        public int? TotalDerivaciones { get; set; }
        public int? TotalDerivacionesProcesadas { get; set; }
        public int? TotalDerivacionesNoProcesadas { get; set; }
        public int? TotalDerivacionesPendientes { get; set; }
        public int? TotalDerivacionesDesembolsadas { get; set; }
        public int? TotalDerivacionesNoDesembolsadas { get; set; }
    }

    public class VOReporteDerivacionDetalle
    {
        public Derivacion? Derivacion { get; set; }
        public GestionDetalle? GestionDetalle { get; set; }
        public Desembolso? Desembolso { get; set; }
    }
}