using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.Interfaces.Reports;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Repositories;
using ALFINapp.API.Models;

namespace ALFINapp.Application.UseCases.Reports
{
    public class UseCaseGetReportesFechas : IUseCaseGetReportesFechas
    {
        private readonly IRepositoryReports _repositoryReports;
        public UseCaseGetReportesFechas(IRepositoryReports repositoryReports)
        {
            _repositoryReports = repositoryReports;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesFecha Data)> Execute(string fecha, int idUsuario, int rol)
        {
            try
            {
                var getReportesPie = await _repositoryReports.GetReportesGpieGeneralFecha(DateOnly.Parse(fecha));
                if (getReportesPie == null)
                {
                    return (false, "No se encontraron reportes para la fecha seleccionada", new ViewReportesFecha());
                }
                var getReportesFechas = new ViewReportesFecha();
                getReportesFechas.ProgresoGeneral = getReportesPie.toViewPie();
                return (true, "ok", getReportesFechas);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewReportesFecha());
            }
        }
    }
}