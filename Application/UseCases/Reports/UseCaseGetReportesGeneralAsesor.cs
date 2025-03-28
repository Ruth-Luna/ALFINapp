using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Reports;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.UseCases.Reports
{
    public class UseCaseGetReportesGeneralAsesor : IUseCaseGetReportesGeneralAsesor
    {
        private readonly IRepositoryReports _repositoryReports;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        public UseCaseGetReportesGeneralAsesor(
            IRepositoryReports repositoryReports,
            IRepositoryUsuarios repositoryUsuarios)
        {
            _repositoryReports = repositoryReports;
            _repositoryUsuarios = repositoryUsuarios;
        }
        public async Task<(bool success, string message, ViewReportesGeneral? data)> Execute(int idUsuario)
        {
            try
            {
                var reportes = await _repositoryReports.GetReportesGralAsesor(idUsuario);
                if (reportes == null)
                {
                    return (false, "No se encontraron reportes", null);
                }
                var asesor = await _repositoryUsuarios.GetUser(idUsuario);
                if (asesor == null)
                {
                    return (false, "No se encontró el asesor", null);
                }
                var reportesGeneral = new ViewReportesGeneral();
                var asesorView = new DetallesUsuarioDTO (asesor).ToView();
                reportesGeneral.Asesores = new List<ViewUsuario> { asesorView };
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
                    }).ToList();
                reportesGeneral.NumDesembolsosXFecha = reportes.Desembolsos
                    .GroupBy(x => x.FechaDesembolsos?.ToString("yyyy-MM-dd") ?? "Unknown")
                    .Select(g => new DerivacionesFecha
                    {
                        Fecha = g.Key,
                        Contador = g.Count()
                    }).ToList();
                return (true, "Reportes obtenidos correctamente", reportesGeneral);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}