using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Reports;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Reports
{
    public class UseCaseGetReportesSupervisor : IUseCaseGetReportesSupervisor
    {
        private readonly IRepositoryReports _repositoryReports;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        private readonly IRepositoryTipificaciones _repositoryTipificaciones;
        public UseCaseGetReportesSupervisor(
            IRepositoryReports repositoryReports,
            IRepositoryUsuarios repositoryUsuarios,
            IRepositoryTipificaciones repositoryTipificaciones)
        {
            _repositoryReports = repositoryReports;
            _repositoryUsuarios = repositoryUsuarios;
            _repositoryTipificaciones = repositoryTipificaciones;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesSupervisor? Data)> Execute(
            int idUsuario,
            int? anio = null,
            int? mes = null)
        {
            try
            {
                var supervisor = await _repositoryUsuarios.GetUser(idUsuario);
                if (supervisor == null)
                {
                    return (false, "Supervisor no encontrado", null);
                }
                var supervisorReportes = await _repositoryReports.GetReportesEspecificoSupervisor(idUsuario, anio, mes);
                var reportesSupervisor = new ViewReportesSupervisor();
                reportesSupervisor.supervisor = new DetallesUsuarioDTO(supervisor).ToView();
                reportesSupervisor.asesores = supervisorReportes?.Asesores.Select(x => new DetallesUsuarioDTO(x).ToView()).ToList();
                reportesSupervisor.totalGestionado = supervisorReportes?.gESTIONDETALLEs.Count();
                reportesSupervisor.totalAsignaciones = supervisorReportes?.ClientesAsignados.Count();
                reportesSupervisor.totalSinGestionar = supervisorReportes?.ClientesAsignados.Count() - supervisorReportes?.gESTIONDETALLEs.Where( x => x.IdAsignacion != null ).Count();
                reportesSupervisor.totalDerivado = supervisorReportes?.gESTIONDETALLEs.Count(x => x.CodTip == 2);
                reportesSupervisor.totalDesembolsado = supervisorReportes?.Desembolsos.Count();
                var createDerivacionesFecha = supervisorReportes?.gESTIONDETALLEs
                    .Where(x => x.CodTip == 2)
                    .GroupBy(x => x.FechaGestion.ToString("%d/%M/%y"))
                    .Select(x => new DerivacionesFecha 
                    { 
                        Fecha = x.Key, 
                        Contador = x.Count() 
                    })
                    .OrderBy(x => DateTime.TryParseExact(x.Fecha, "d/M/yy", null, System.Globalization.DateTimeStyles.None, out var parsedDate) ? parsedDate : DateTime.MinValue)
                    .ToList() ?? new List<DerivacionesFecha>();
                reportesSupervisor.derivacionesFecha = createDerivacionesFecha;
                var getIdDer = supervisorReportes?.DerivacionesSupervisor
                    .Select(x => x.IdDerivacion)
                    .ToHashSet();
                var createDesembolsosFechaGestion = supervisorReportes?.Desembolsos
                    .GroupBy(x => x.FechaDesembolsos!=null?x.FechaDesembolsos.Value.ToString("%d/%M/%y"):"")
                    .Select(x => new DerivacionesFecha { Fecha = x.Key, Contador = x.Count() })
                    .OrderBy(x => DateTime.TryParseExact(x.Fecha, "d/M/yy", null, System.Globalization.DateTimeStyles.None, out var parsedDate) ? parsedDate : DateTime.MinValue)
                    .ToList() ?? new List<DerivacionesFecha>();

                reportesSupervisor.desembolsosFecha = createDesembolsosFechaGestion;
                var reporteTipificaciones = supervisorReportes?.Asesores?
                    .Where(x => x.Estado == "ACTIVO")
                    .Select(asesor => new ViewTipificacionesAsesor
                    {
                        DniAsesor = asesor.Dni,
                        NombreAsesor = asesor.NombresCompletos,
                        totalSinGestionar = supervisorReportes.ClientesAsignados.Count(x => x.IdUsuarioV == asesor.IdUsuario) 
                                        - supervisorReportes.gESTIONDETALLEs.Count(x => x.DocCliente == asesor.Dni),
                        totalGestionado = supervisorReportes.gESTIONDETALLEs.Count(x => x.DocAsesor == asesor.Dni),
                        totalDesembolsos = supervisorReportes.Desembolsos.Count(x => x.DocAsesor == asesor.Dni),
                        totalDerivaciones = supervisorReportes.gESTIONDETALLEs.Count(x => x.DocAsesor == asesor.Dni && x.CodTip == 2),
                    })
                    .ToList() ?? new List<ViewTipificacionesAsesor>();
                reportesSupervisor.tipificacionesAsesores = reporteTipificaciones;
                var tipificacionesDescrp = await _repositoryTipificaciones.GetTipificacionesDescripcion();
                var tipificaciones = supervisorReportes?.gESTIONDETALLEs
                    .GroupBy(x => x.CodTip)
                    .Select(g => new ViewTipificacionesCantidad
                    {
                        IdTipificacion = g.Key,
                        TipoTipificacion = tipificacionesDescrp.FirstOrDefault(t => t.IdTipificacion == g.Key)?.DescripcionTipificacion,
                        Cantidad = g.Count()
                    }).ToList() ?? new List<ViewTipificacionesCantidad>();
                return (true, "Reportes de supervisor obtenidos", reportesSupervisor);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}