using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Reports;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Reports
{
    public class UseCaseGetReportesGeneralSupervisor : IUseCaseGetReportesGeneralSupervisor
    {
        private readonly IRepositoryReports _repositoryReports;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        public UseCaseGetReportesGeneralSupervisor(
            IRepositoryReports repositoryReports,
            IRepositoryUsuarios repositoryUsuarios)
        {
            _repositoryReports = repositoryReports;
            _repositoryUsuarios = repositoryUsuarios;
        }
        public async Task<(bool success, string message, ViewReportesGeneral? data)> Execute(int idSupervisor)
        {
            try
            {
                var reportes = await _repositoryReports.GetReportesGralSupervisor(idSupervisor);
                if (reportes == null)
                {
                    return (false, "No se encontraron reportes", null);
                }
                var asesores = await _repositoryUsuarios.GetAllAsesoresBySupervisor(idSupervisor);
                
                var reportesGeneral = new ViewReportesGeneral();
                var asesoresView = asesores.Select(asesor => asesor.ToView()).ToList();
                reportesGeneral.Asesores = asesoresView;
                reportesGeneral.TotalDerivaciones = reportes.DerivacionesGral.Count;
                reportesGeneral.TotalDerivacionesDesembolsadas = reportes.Desembolsos.Count;
                reportesGeneral.TotalDerivacionesNoDesembolsadas = reportes.DerivacionesGral.Count - reportes.Desembolsos.Count;
                reportesGeneral.TotalDerivacionesNoProcesadas = reportes.DerivacionesGral.Where(x => x.FueProcesado == false).Count();
                reportesGeneral.TotalDerivacionesEnvioEmailAutomatico = reportes.DerivacionesGral.Where(x => x.FueEnviadoEmail == true).Count();
                reportesGeneral.TotalDerivacionesEnvioForm = reportes.DerivacionesGral.Where(x => x.FueProcesado == true).Count();

                reportesGeneral.NumDerivacionesXFecha = reportes.DerivacionesGral
                    .GroupBy(x => x.FechaDerivacion.ToString("yyyy-MM-dd"))
                    .Select(g => new DerivacionesFecha
                    {
                        Fecha = g.Key,
                        Contador = g.Count()
                    })
                    .OrderByDescending(x => x.Fecha)
                    .ToList();
                reportesGeneral.NumDesembolsosXFecha = reportes.Desembolsos
                    .GroupBy(x => x.FechaDesembolsos?.ToString("yyyy-MM-dd") ?? "Unknown")
                    .Select(g => new DerivacionesFecha
                    {
                        Fecha = g.Key,
                        Contador = g.Count()
                    })
                    .OrderByDescending(x => x.Fecha)
                    .ToList();
                return (true, "Reportes obtenidos correctamente", reportesGeneral);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}