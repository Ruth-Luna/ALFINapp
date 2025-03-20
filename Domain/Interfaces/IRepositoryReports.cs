
using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryReports
    {
        public Task<DetallesReportesAdministradorDTO?> GetReportesAdministradorAsesores();
    }
}