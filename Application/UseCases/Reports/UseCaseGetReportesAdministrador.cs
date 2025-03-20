using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.Interfaces.Reports;
using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Reports
{
    public class UseCaseGetReportesAdministrador : IUseCaseGetReportesAdministrador
    {
        private readonly IRepositoryReports _repositoryReports;
        public UseCaseGetReportesAdministrador(IRepositoryReports repositoryReports)
        {
            _repositoryReports = repositoryReports;
        }
        public async Task<(bool IsSuccess, string Message, Reporte? Data)> Execute(string? email, int idUsuario)
        {
            try
            {
                var getDerivacionReports = await _repositoryReports.GetReportesDerivacionGral();

                return (true, "Reportes obtenidos correctamente", null);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}