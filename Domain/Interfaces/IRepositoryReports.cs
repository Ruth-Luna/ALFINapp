
using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryReports
    {
<<<<<<< HEAD
        public Task<DetallesReportesAsesorDTO> GetReportesAsesor(int idUsuario);
        public Task<DetallesReportesSupervisorDTO> GetReportesEspecificoSupervisor(int idUsuario);
=======
        public Task<DetallesReportesAsesorDTO> GetReportesAsesor(
            int idUsuario,
            int? anio = null,
            int? mes = null);
        public Task<DetallesReportesSupervisorDTO> GetReportesEspecificoSupervisor(
            int idUsuario
            , int? anio = null
            , int? mes = null);
>>>>>>> d0736346e8dd5e7829bc5c0baea4bfb205da3e8d
        public Task<DetallesReportesLineaGestionVsDerivacionDTO> LineaGestionVsDerivacionDiaria (int idUsuario);
        public Task<DetallesReportesGpieDTO> GetReportesGpieGeneral(int idUsuario);
        public Task<DetallesReportesGpieDTO> GetReportesGpieGeneralFecha(DateOnly fecha, int idUsuario);
        public Task<DetallesReportesGpieDTO> GetReportesPieContactabilidadCliente(int idUsuario);
        public Task<DetallesReportesEtiquetasDTO> GetReportesEtiquetasDesembolsosNImportes(int idUsuario);
        public Task<DetallesReportesTablasDTO> GetReportesMetas(int idUsuario);
        public Task<DetallesReportesGpieDTO> GetReportesByDate( int idUsuario, DateTime? fecha = null);
        public Task<DetallesReportesGpieDTO> GetReportesByActualMonth(int idUsuario);
<<<<<<< HEAD
        public Task<DetallesReportesEtiquetasDTO> GetReportesEtiquetasMetas(int idUsuario);
=======
        public Task<DetallesReportesEtiquetasDTO> GetReportesEtiquetasMetas(int? anio, int? mes);
        public Task<DetallesReportesTablasDTO> GetReportesTablaGeneralFechaMeses(int idUsuario, int mes, int año);
        public Task<DetallesReportesGpieDTO> GetReportesGpieGeneralFechaMeses(int idUsuario, int mes, int año);
>>>>>>> d0736346e8dd5e7829bc5c0baea4bfb205da3e8d
    }
}