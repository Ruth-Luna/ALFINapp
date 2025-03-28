
using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryReports
    {
        public Task<DetallesReportesAdministradorDTO?> GetReportesAdministradorAsesores();
        public Task<DetallesReportesAsesorDTO> GetReportesAsesor(int idUsuario);
        public Task<DetallesReportesDerivacionesDTO?> GetReportesDerivacionGral();
        public Task<DetallesReportesSupervisorDTO> GetReportesEspecificoSupervisor(int idUsuario);
        public Task<DetallesReportesDerivacionesDTO?> GetReportesGralSupervisor(int idSupervisor);
        public Task<DetallesReportesDerivacionesDTO> GetReportesGralAsesor(int idAsesor);
    }
}