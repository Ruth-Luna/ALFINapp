using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Reports;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Repositories.Async.Interfaces;

namespace ALFINapp.Application.UseCases.Reports
{
    public class UseCaseGetReportes : IUseCaseGetReportes
    {
        private readonly IRepositoryReports _repositoryReports;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        private readonly IRepositoryReportsAsync _repositoryReportsAsync;
        public UseCaseGetReportes(
            IRepositoryReports repositoryReports,
            IRepositoryUsuarios repositoryUsuarios,
            IRepositoryReportsAsync repositoryReportsAsync)
        {
            _repositoryReports = repositoryReports;
            _repositoryUsuarios = repositoryUsuarios;
            _repositoryReportsAsync = repositoryReportsAsync;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesGeneral? Data)> Execute(
            int idUsuario,
            int? anio = null,
            int? mes = null)
        {
            try
            {
                var user = await _repositoryUsuarios.GetUser(idUsuario);
                if (user == null)
                {
                    return (false, "Usuario no encontrado", null);
                }
                var reporteGeneral = new ViewReportesGeneral();
                if (user.IdRol == 1 || user.IdRol == 4 || user.IdRol == 2)
                {
                    var usuarios = await _repositoryUsuarios.GetAllAsesores();
                    var usuariosViews = new List<ViewUsuario>();

                    foreach (var item in usuarios)
                    {
                        var usuarioView = new ViewUsuario();
                        usuarioView = item.ToView();
                        usuariosViews.Add(usuarioView);
                    }
                    reporteGeneral.Asesores = usuariosViews;

                    if (user.IdRol == 1 || user.IdRol == 4)
                    {
                        var supervisores = await _repositoryUsuarios.GetAllSupervisores();
                        var supervisoresViews = new List<ViewUsuario>();

                        foreach (var item in supervisores)
                        {
                            var supervisorView = new ViewUsuario();
                            supervisorView = item.ToView();
                            supervisoresViews.Add(supervisorView);
                        }
                        reporteGeneral.Supervisores = supervisoresViews;
                    }
                    // GRAFICAS DE REPORTES Generales
                    /*
                    var lineasGestionDerivacion = await _repositoryReports.LineaGestionVsDerivacionDiaria(idUsuario);
                    var pieGestionAsignados = await _repositoryReports.GetReportesGpieGeneral(idUsuario);
                    var barTop5Asesores = await _repositoryReports.GetReportesBarTop5General(idUsuario);
                    var tablaGestionado = await _repositoryReports.GetReportesTablaGestionDerivadoDesembolsoImporte();
                    var pieContactabilidad = await _repositoryReports.GetReportesPieContactabilidadCliente(idUsuario);
                    var etiquetaDesembolsoMonto = await _repositoryReports.GetReportesEtiquetasDesembolsosNImportes(idUsuario);
                    */
                    var reportesAsync = await _repositoryReportsAsync.GetReportesAsync(idUsuario, anio, mes);
                    var reportesEtiquetas = await _repositoryReports.GetReportesEtiquetasMetas(anio,mes);
                    reporteGeneral.lineaGestionVsDerivacion = reportesAsync.linea.toViewLineaGestionVsDerivacion();
                    reporteGeneral.ProgresoGeneral = reportesAsync.pie.toViewPie();
                    reporteGeneral.top5asesores = reportesAsync.bar.toViewListReporteBarGeneral();
                    reporteGeneral.reporteTablaGeneral = reportesAsync.tabla.toViewTabla();
                    reporteGeneral.pieContactabilidad = reportesAsync.pie2.toViewPieLista();
                    reporteGeneral.etiquetas = new List<ViewEtiquetas>();
                    reporteGeneral.etiquetas.AddRange(reportesAsync.etiquetas.toViewEtiquetas());
                    reporteGeneral.etiquetas.AddRange(reportesEtiquetas.toViewEtiquetas());
                    if (mes != null && anio != null)
                    {
                        reporteGeneral.filtro_por_fechas = true;
                        var fechafiltro = new FechaDelFiltro();
                        fechafiltro.mes = mes;
                        fechafiltro.anio = anio;
                        reporteGeneral.fecha_filtro = fechafiltro;
                    }
                    return (true, "Reportes obtenidos correctamente", reporteGeneral);
                }
                else if (user.IdRol == 3)
                {
                    var reportesAsync = await _repositoryReportsAsync.GetReportesAsync(idUsuario, anio, mes);
                    var reportesEtiquetas = await _repositoryReports.GetReportesEtiquetasMetas(anio,mes);
                    reporteGeneral.lineaGestionVsDerivacion = reportesAsync.linea.toViewLineaGestionVsDerivacion();
                    reporteGeneral.ProgresoGeneral = reportesAsync.pie.toViewPie();
                    reporteGeneral.pieContactabilidad = reportesAsync.pie2.toViewPieLista();
                    reporteGeneral.etiquetas = new List<ViewEtiquetas>();
                    reporteGeneral.etiquetas.AddRange(reportesAsync.etiquetas.toViewEtiquetas());
                    reporteGeneral.etiquetas.AddRange(reportesEtiquetas.toViewEtiquetas());
                    return (true, "Reportes obtenidos correctamente", reporteGeneral);
                }
                else
                {
                    return (false, "Rol no permitido", null);
                }
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}