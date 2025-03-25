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
        public async Task<(bool IsSuccess, string Message, ViewReportesGeneral? Data)> Execute()
        {
            try
            {
                /*var getAsignaciones = await _repositoryClientes.GetAllAsignaciones();*/

                var getDerivacionReports = await _repositoryReports.GetReportesDerivacionGral();
                if (getDerivacionReports == null)
                {
                    return (true, "No hay informacion de derivaciones de este mes. Aun no se pueden hacer reportes", null);
                }

                var Derivaciones = getDerivacionReports.DerivacionesGral;
                var GestionDetalles = getDerivacionReports.GestionDetalles;
                var Desembolsos = getDerivacionReports.Desembolsos;

                var createReport = ( from dr in Derivaciones
                    join gd in GestionDetalles on dr.DniCliente equals gd.DocCliente into gdGroup
                    from gd in gdGroup.DefaultIfEmpty()
                    join de in Desembolsos on dr.DniCliente equals de.DniDesembolso into deGroup
                    from de in deGroup.DefaultIfEmpty()
                    select new 
                    {
                        dr,
                        gd,
                        de
                    }
                ).ToList();
                createReport = createReport
                    .GroupBy(x => x.dr.DniCliente)
                    .Select(x => x.FirstOrDefault())
                    .ToList();
                var reporteGeneral = new ViewReportesGeneral();
                var reporteDerivaciones = new List<ViewReporteDerivaciones>();
                foreach (var item in createReport)
                {
                    var reporteDerivacion = new ViewReporteDerivaciones();
                    
                    reporteDerivacion.derivacion = MapperDerivaciones.ToEntity(item.dr);
                    reporteDerivacion.desembolsos = MapperDesembolso.ToEntity(item.de);
                    reporteDerivacion.gestionDetalle = MapperGestionDetalle.ToEntity(item.gd);
                    reporteDerivaciones.Add(reporteDerivacion);
                }

                var CreateNumDerivacionesXFecha = reporteDerivaciones
                    .GroupBy(x => x.derivacion.FechaDerivacion.ToString("%d/%M/%y"))
                    .Select(x => new DerivacionesFecha {Fecha = x.Key, Contador = x.Count()})
                    .ToList();
                var CreateNumDesembolsosXFecha = reporteDerivaciones
                    .GroupBy(x => x.desembolsos.FechaDesembolsos!=null
                        ? x.desembolsos.FechaDesembolsos.Value.ToString("%d/%M/%y")
                        : null)
                    .Select(x => new DerivacionesFecha {Fecha = x.Key, Contador = x.Count()})
                    .ToList();
                reporteGeneral.Derivaciones = reporteDerivaciones;
                reporteGeneral.NumDerivacionesXFecha = CreateNumDerivacionesXFecha;
                reporteGeneral.NumDesembolsosXFecha = CreateNumDesembolsosXFecha;
                reporteGeneral.TotalDerivaciones = reporteDerivaciones.Count;
                reporteGeneral.TotalDerivacionesDesembolsadas = reporteDerivaciones.Count(x => x.desembolsos.DniDesembolso != null);
                reporteGeneral.TotalDerivacionesNoDesembolsadas = reporteGeneral.TotalDerivaciones - reporteGeneral.TotalDerivacionesDesembolsadas;
                reporteGeneral.TotalDerivacionesEnvioEmailAutomatico = reporteDerivaciones.Count(x => x.derivacion.FueEnviadoEmail == true);
                reporteGeneral.TotalDerivacionesEnvioForm = reporteDerivaciones.Count(x => x.derivacion.FueProcesado == true);
                reporteGeneral.TotalDerivacionesNoProcesadas = reporteGeneral.TotalDerivaciones - reporteGeneral.TotalDerivacionesEnvioEmailAutomatico;

                var usuarios = await _repositoryUsuarios.GetAllAsesores();
                var usuariosViews = new List<ViewUsuario>();

                foreach (var item in usuarios)
                {
                    var usuarioView = new ViewUsuario();
                    usuarioView = item.ToView();
                    usuariosViews.Add(usuarioView);
                }

                reporteGeneral.Asesores = usuariosViews;
                return (true, "Reportes obtenidos correctamente", reporteGeneral);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}