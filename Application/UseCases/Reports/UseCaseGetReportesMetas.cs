using ALFINapp.Application.Interfaces.Reports;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Reports
{
    public class UseCaseGetReportesMetas : IUseCaseGetReportesMetas
    {
        private readonly IRepositoryReports _repositoryReports;
        public UseCaseGetReportesMetas(IRepositoryReports repositoryReports)
        {
            _repositoryReports = repositoryReports;
        }
        public async Task<(bool IsSuccess, string Message, object Data)> Execute(int idUsuario)
        {
            try
            {
                return (true, "OK", new { });
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new { });
            }
        }
    }
}