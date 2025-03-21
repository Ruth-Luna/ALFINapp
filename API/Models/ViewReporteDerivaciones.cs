using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;

namespace ALFINapp.API.Models
{
    public class ViewReporteDerivaciones
    {
        public Derivacion derivacion { get; set; } = new Derivacion();
        public GestionDetalle gestionDetalle { get; set; } = new GestionDetalle();
        public Desembolso desembolsos { get; set; } = new Desembolso();
    }
}