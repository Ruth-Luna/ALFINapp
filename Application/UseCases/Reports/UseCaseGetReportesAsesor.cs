using ALFINapp.Application.Interfaces.Reports;
using ALFINapp.Domain.Entities;
using ALFINapp.Infrastructure.Repositories;

namespace ALFINapp.Application.UseCases.Reports
{
    public class UseCaseGetReportesAsesor : IUseCaseGetReportesAsesor
    {
        public async Task<(bool IsSuccess, string Message, Reporte? Data)> Execute(int idUsuario)
        {
            try
            {
                return (true, "Reportes obtenidos correctamente", null);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}