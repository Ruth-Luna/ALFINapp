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
        public async Task<(bool IsSuccess, string Message, ViewReportesFecha Data)> Execute(
            string fecha, 
            int idUsuario, 
            int rol,
            int? mes = null,
            int? año = null)
        {
            try
            {
                if (string.IsNullOrEmpty(fecha))
                {
                    return (false, "La fecha no puede estar vacía", new ViewReportesFecha());
                }
                if (año == null || mes == null)
                {
                    var getReportesPie = await _repositoryReports.GetReportesGpieGeneralFecha(DateOnly.Parse(fecha), idUsuario);
                    if (getReportesPie == null)
                    {
                        return (false, "No se encontraron reportes para la fecha seleccionada", new ViewReportesFecha());
                    }
                    var getReportesFechas = new ViewReportesFecha();
                    getReportesFechas.ProgresoGeneral = getReportesPie.toViewPie();
                    return (true, "ok", getReportesFechas);
                }
                if (año != null || mes != null)
                {
                    var getReportesPie = await _repositoryReports.GetReportesGpieGeneralFechaMeses(idUsuario, mes.Value, año.Value);
                    if (getReportesPie == null)
                    {
                        return (false, "No se encontraron reportes para la fecha seleccionada", new ViewReportesFecha());
                    }
                    var getReportesTabla = await _repositoryReports.GetReportesTablaGeneralFechaMeses(idUsuario, mes.Value, año.Value);
                    if (getReportesTabla == null)
                    {
                        return (false, "No se encontraron reportes para la fecha seleccionada", new ViewReportesFecha());
                    }
                    var getReportesFechas = new ViewReportesFecha();
                    getReportesFechas.ProgresoGeneral = getReportesPie.toViewPie();
                    getReportesFechas.reporteTablaPorMeses = getReportesTabla.toViewTablaMeses();
                    return (true, "ok", getReportesFechas);
                }
                else
                {
                    return (false, "No se encontraron reportes para la fecha seleccionada", new ViewReportesFecha());
                }
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewReportesFecha());
            }
        }
    }
}