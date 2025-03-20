using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesDerivacionesDTO
    {
        public List<DerivacionesAsesores>? DerivacionesGral { get; set; }
        public List<GESTIONDETALLE>? GestionDetalles { get; set; }
        public List<Desembolsos>? Desembolsos { get; set; }
    }
}