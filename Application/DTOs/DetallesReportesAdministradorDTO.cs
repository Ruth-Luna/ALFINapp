using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesAdministradorDTO
    {
        public DerivacionesAsesoresList? DerivacionesAllInfo { get; set; }

    }
    public class DerivacionesAsesoresList
    {
        public string? DniAsesor { get; set; }
        public string? NombreCompletoAsesor { get; set; }
        public int CantidadDerivaciones { get; set; }
        public int CantidadDerivacionesProcesadas { get; set; }
        public int CantidadDerivacionesDesembolsadas { get; set; }
        public int CantidadDerivacionesPendientes { get; set; }
        public List<DerivacionInformacionDesembolsos> AllDerivaciones = new List<DerivacionInformacionDesembolsos>();
    }
    public class DerivacionInformacionDesembolsos
    {
        public DerivacionesAsesores? DerivacionAsesor { get; set; }
        public bool? FueDesembolsado { get; set; }
        public Desembolsos? Desembolso { get; set; }
    }
}