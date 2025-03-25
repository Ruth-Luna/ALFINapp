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
        public UseCaseGetReportesAsesor(
            IRepositoryReports repositoryReports,
            IRepositoryUsuarios repositoryUsuarios)
        {
            _repositoryReports = repositoryReports;
            _repositoryUsuarios = repositoryUsuarios;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesAsesores? Data)> Execute(int idUsuario)
        {
            try
            {
                var usuario = await _repositoryUsuarios.GetUser(idUsuario);
                if (usuario == null)
                {
                    return (false, "Usuario no encontrado", null);
                }
                var reportes = await _repositoryReports.GetReportesAsesor(idUsuario);
                var viewReportes = new ViewReportesAsesores();
                var detallesUsuarioDTO = new DetallesUsuarioDTO(usuario);
                viewReportes.asesor = detallesUsuarioDTO.ToView();
                viewReportes.numDerivaciones = reportes.DerivacionesDelAsesor.Count;
                viewReportes.numDesembolsos = reportes.DerivacionesDelAsesor.Count;
                viewReportes.numClientesAsignados = reportes.ClientesAsignados.Count;
                viewReportes.numClientesTipificados = reportes.UltimaTipificacionXAsignacion.Count;
                viewReportes.numClientesNoTipificados = reportes.ClientesAsignados.Count - reportes.UltimaTipificacionXAsignacion.Count;
                var createDerivacionesFecha = reportes
                    .DerivacionesDelAsesor
                    .GroupBy(x => x.FechaDerivacion.ToString("%d/%M/%y"))
                    .Select(x => new DerivacionesFecha {Fecha = x.Key, Contador = x.Count()})
                    .ToList();
                viewReportes.derivacionesFecha = createDerivacionesFecha;
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