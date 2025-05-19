using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Reports;
using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Repositories;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace ALFINapp.Application.UseCases.Reports
{
    public class UseCaseGetReportesAsesor : IUseCaseGetReportesAsesor
    {
        private readonly IRepositoryReports _repositoryReports;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        private readonly IRepositoryTipificaciones _repositoryTipificaciones;
        public UseCaseGetReportesAsesor(
            IRepositoryReports repositoryReports,
            IRepositoryUsuarios repositoryUsuarios,
            IRepositoryTipificaciones repositoryTipificaciones)
        {
            _repositoryReports = repositoryReports;
            _repositoryUsuarios = repositoryUsuarios;
            _repositoryTipificaciones = repositoryTipificaciones;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesAsesores? Data)> Execute(
            int idUsuario,
            int anio,
            int mes)
        {
            try
            {
                var usuario = await _repositoryUsuarios.GetUser(idUsuario);
                if (usuario == null)
                {
                    return (false, "Usuario no encontrado", null);
                }
                var reportes = await _repositoryReports.GetReportesAsesor(idUsuario, anio, mes);
                var viewReportes = new ViewReportesAsesores();
                var detallesUsuarioDTO = new DetallesUsuarioDTO(usuario);
                viewReportes.asesor = detallesUsuarioDTO.ToView();
                viewReportes.totalDerivaciones = reportes.gESTIONDETALLEs
                    .Where(x => x.CodTip == 2)
                    .Count();
                viewReportes.totalDesembolsos = reportes.Desembolsos.Count();
                viewReportes.totalAsignado = reportes.ClientesAsignados.Count();
                viewReportes.totalGestionado = reportes.gESTIONDETALLEs.Count();
                viewReportes.totalSinGestionar = reportes.ClientesAsignados.Count - reportes.gESTIONDETALLEs
                    .Where(x => x.IdAsignacion != null)
                    .Count();
                var createDerivacionesFecha = reportes
                    .gESTIONDETALLEs
                    .Where(x => x.CodTip == 2)
                    .GroupBy(x => x.FechaGestion.ToString("%d/%M/%y"))
                    .Select(x => new DerivacionesFecha { Fecha = x.Key, Contador = x.Count() })
                    .OrderBy(x => DateTime.TryParseExact(x.Fecha, "d/M/yy", null, System.Globalization.DateTimeStyles.None, out var parsedDate) ? parsedDate : DateTime.MinValue)
                    .ToList();
                viewReportes.derivacionesFecha = createDerivacionesFecha;
                var createDesembolsosFecha = reportes
                    .Desembolsos
                    .GroupBy(x => x.FechaDesembolsos!=null?x.FechaDesembolsos.Value.ToString("%d/%M/%y"):"")
                    .Select(x => new DerivacionesFecha { Fecha = x.Key, Contador = x.Count() })
                    .OrderBy(x => DateTime.TryParseExact(x.Fecha, "d/M/yy", null, System.Globalization.DateTimeStyles.None, out var parsedDate) ? parsedDate : DateTime.MinValue)
                    .ToList();
                viewReportes.gestionDetalles = reportes.gESTIONDETALLEs.Select(x => new DetallesGestionDetalleDTO(x).toView()).ToList();
                var TipificacionesGestion = new List<ViewTipificacionesGestion>();
                var TipificacionesDescripcion = await _repositoryTipificaciones.GetTipificacionesDescripcion();
                var dicTipificaciones = TipificacionesDescripcion
                    .ToDictionary(y => y.IdTipificacion, y => y.DescripcionTipificacion);
                var agruparTipificaciones = viewReportes
                    .gestionDetalles
                    .GroupBy(x => x.CodTip)
                    .Select(g => new ViewTipificacionesGestion
                    {
                        IdTipificacion = g.Key,
                        DescripcionTipificaciones = dicTipificaciones.TryGetValue(g.Key, out var descripcion) ? descripcion : "",
                        ContadorTipificaciones = g.Count()
                    })
                    .ToList();
                viewReportes.tipificacionesGestion = agruparTipificaciones;
                return (true, "Reportes obtenidos correctamente", viewReportes);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }

    public interface IReportesRepository
    {
    }
}