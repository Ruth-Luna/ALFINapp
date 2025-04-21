using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Reports;
using ALFINapp.Application.Mappers;
using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Domain.ValueObjects;

namespace ALFINapp.Application.UseCases.Reports
{
    public class UseCaseGetReportesAdministrador : IUseCaseGetReportesAdministrador
    {
        private readonly IRepositoryReports _repositoryReports;
        private readonly IRepositoryClientes _repositoryClientes;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        public UseCaseGetReportesAdministrador(
            IRepositoryReports repositoryReports, 
            IRepositoryClientes repositoryClientes,
            IRepositoryUsuarios repositoryUsuarios)
        {
            _repositoryReports = repositoryReports;
            _repositoryClientes = repositoryClientes;
            _repositoryUsuarios = repositoryUsuarios;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesGeneral? Data)> Execute(int idUsuario)
        {
            try
            {
                var reporteGeneral = new ViewReportesGeneral();

                var usuarios = await _repositoryUsuarios.GetAllAsesores();
                var usuariosViews = new List<ViewUsuario>();

                foreach (var item in usuarios)
                {
                    var usuarioView = new ViewUsuario();
                    usuarioView = item.ToView();
                    usuariosViews.Add(usuarioView);
                }
                var supervisores = await _repositoryUsuarios.GetAllSupervisores();
                var supervisoresViews = new List<ViewUsuario>();

                foreach (var item in supervisores)
                {
                    var supervisorView = new ViewUsuario();
                    supervisorView = item.ToView();
                    supervisoresViews.Add(supervisorView);
                }
                reporteGeneral.Supervisores = supervisoresViews;
                reporteGeneral.Asesores = usuariosViews;
                // GRAFICAS DE REPORTES Generales
                var lineasGestionDerivacion = await _repositoryReports.LineaGestionVsDerivacionDiaria(idUsuario);
                var pieGestionAsignados = await _repositoryReports.GetReportesGpieGeneral(idUsuario);
                var barTop5Asesores = await _repositoryReports.GetReportesBarTop5General(idUsuario);
                var tablaGestionado = await _repositoryReports.GetReportesTablaGestionDerivadoDesembolsoImporte();
                var pieContactabilidad = await _repositoryReports.GetReportesPieContactabilidadCliente(idUsuario);
                var etiquetaDesembolsoMonto = await _repositoryReports.GetReportesEtiquetasDesembolsosNImportes(idUsuario);
                reporteGeneral.lineaGestionVsDerivacion = lineasGestionDerivacion.toViewLineaGestionVsDerivacion();
                reporteGeneral.ProgresoGeneral = pieGestionAsignados.toViewPie();
                reporteGeneral.top5asesores = barTop5Asesores.toViewListReporteBarGeneral();
                reporteGeneral.reporteTablaGeneral = tablaGestionado.toViewTabla();
                reporteGeneral.pieContactabilidad = pieContactabilidad.toViewPieLista();
                reporteGeneral.etiquetas = etiquetaDesembolsoMonto.toViewEtiquetas();
                
                return (true, "Reportes obtenidos correctamente", reporteGeneral);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}