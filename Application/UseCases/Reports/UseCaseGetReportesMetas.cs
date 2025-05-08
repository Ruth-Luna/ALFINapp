using ALFINapp.API.Models;
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
        public async Task<(bool IsSuccess, string Message, ViewReportesMetas Data)> Execute(int idUsuario)
        {
            try
            {
                var metas = await _repositoryReports.GetReportesMetas(idUsuario);
                if (metas == null)
                {
                    return (false, "No se encontraron metas para el usuario.", new ViewReportesMetas());
                }
                var reporteMetas = new ViewReportesMetas ();
                reporteMetas.metas = metas.toViewMetas();
                reporteMetas.totalGestiones = reporteMetas.metas.Sum(x => x.totalGestion);
                reporteMetas.totalImporte = reporteMetas.metas.Sum(x => x.totalImporte) ;
                reporteMetas.totalDerivaciones = reporteMetas.metas.Sum(x => x.totalDerivaciones);
                return (true, "OK", reporteMetas);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return (false, ex.Message, new ViewReportesMetas());
            }
        }
    }
}