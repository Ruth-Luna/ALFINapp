
using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryReports
    {
        public Task<DetallesReportesAsesorDTO> GetReportesAsesor(int idUsuario);
        public Task<DetallesReportesSupervisorDTO> GetReportesEspecificoSupervisor(int idUsuario);
        public Task<DetallesReportesDerivacionesDTO?> GetReportesGralSupervisor(int idSupervisor);
        public Task<DetallesReportesDerivacionesDTO> GetReportesGralAsesor(int idAsesor);
        public Task<DetallesReportesLineaGestionVsDerivacionDTO> LineaGestionVsDerivacionDiaria (int idUsuario);
        public Task<DetallesReportesGpieDTO> GetReportesGpieGestionadosVsAsignados();
        public Task<DetallesReportesGpieDTO> GetReportesGpieDerivadosVsDesembolsados();
        public Task<DetallesReportesGpieDTO> GetReportesGpieGeneral(int idUsuario);
        public Task<DetallesReportesGpieDTO> GetReportesGpieGeneralFecha(DateOnly fecha);
        public Task<DetallesReportesBarDTO> GetReportesBarTop5General(int idUsuario);
        public Task<DetallesReportesGpieDTO> GetReportesPieContactabilidadCliente(int idUsuario);
    }
}