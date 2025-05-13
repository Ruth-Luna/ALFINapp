
using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryReports
    {
        public Task<DetallesReportesAsesorDTO> GetReportesAsesor(int idUsuario);
        public Task<DetallesReportesSupervisorDTO> GetReportesEspecificoSupervisor(int idUsuario);
        public Task<DetallesReportesLineaGestionVsDerivacionDTO> LineaGestionVsDerivacionDiaria (int idUsuario);
        public Task<DetallesReportesGpieDTO> GetReportesGpieGeneral(int idUsuario);
        public Task<DetallesReportesGpieDTO> GetReportesGpieGeneralFecha(DateOnly fecha, int idUsuario);
        public Task<DetallesReportesGpieDTO> GetReportesPieContactabilidadCliente(int idUsuario);
        public Task<DetallesReportesEtiquetasDTO> GetReportesEtiquetasDesembolsosNImportes(int idUsuario);
        public Task<DetallesReportesTablasDTO> GetReportesMetas(int idUsuario);
        public Task<DetallesReportesGpieDTO> GetReportesByDate( int idUsuario, DateTime? fecha = null);
        public Task<DetallesReportesGpieDTO> GetReportesByActualMonth(int idUsuario);
        public Task<DetallesReportesEtiquetasDTO> GetReportesEtiquetasMetas();
    }
}