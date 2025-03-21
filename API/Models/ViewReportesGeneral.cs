using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.API.Models
{
    public class ViewReportesGeneral
    {
        public List<ViewUsuarioInformesClientes>? Usuarios { get; set; }
        public List<ViewSupervisorInformesAsesores>? Supervisores { get; set; }
        public List<ViewReporteDerivaciones>? Derivaciones { get; set; }
        public int? TotalClientes { get; set; }
        public int? TotalClientesTrabajados { get; set; }
        public int? TotalDerivaciones { get; set; }
        public int? TotalDerivacionesDesembolsadas { get; set; }
        public int? TotalDerivacionesNoDesembolsadas { get; set; }
        public int? TotalDerivacionesPendientes { get; set; }
        public int? TotalDerivacionesRechazadas { get; set; }
        public int? TotalDerivacionesAprobadas { get; set; }
        public int? TotalDerivacionesProcesadas { get; set; }
        public int? TotalDerivacionesNoProcesadas { get; set; }
        public int? TotalDerivacionesEnvioEmailAutomatico { get; set; }
        public int? TotalDerivacionesEnvioForm { get; set; }
        public List<DerivacionesFecha>? NumDerivacionesXFecha { get; set; }
        public List<DerivacionesFecha>? NumDesembolsosXFecha { get; set; }
    }
    public class DerivacionesFecha
    {
        public string? Fecha { get; set; }
        public int Contador { get; set; }
    }
    public class DesembolsosAsesor
    {
        public string? Usuario { get; set; }
        public int Contador { get; set; }
    }
    
}