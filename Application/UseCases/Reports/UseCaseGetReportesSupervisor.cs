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
    public class UseCaseGetReportesSupervisor : IUseCaseGetReportesSupervisor
    {
        private readonly IRepositoryReports _repositoryReports;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        public UseCaseGetReportesSupervisor(
            IRepositoryReports repositoryReports,
            IRepositoryUsuarios repositoryUsuarios)
        {
            _repositoryReports = repositoryReports;
            _repositoryUsuarios = repositoryUsuarios;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesSupervisor? Data)> Execute(int idUsuario)
        {
            try
            {
                var supervisor = await _repositoryUsuarios.GetUser(idUsuario);
                if (supervisor == null)
                {
                    return (false, "Supervisor no encontrado", null);
                }
                var supervisorReportes = await _repositoryReports.GetReportesEspecificoSupervisor(idUsuario);
                var reportesSupervisor = new ViewReportesSupervisor();
                reportesSupervisor.supervisor = new DetallesUsuarioDTO(supervisor).ToView();
                reportesSupervisor.asesores = supervisorReportes?.Asesores.Select(x => new DetallesUsuarioDTO(x).ToView()).ToList();
                reportesSupervisor.totalDerivaciones = supervisorReportes?.DerivacionesSupervisor.Count();
                reportesSupervisor.totalDerivacionesDesembolsadas = supervisorReportes?.Desembolsos.Count();
                reportesSupervisor.totalDerivacionesNoDesembolsadas = supervisorReportes?.DerivacionesSupervisor.Count() - supervisorReportes?.Desembolsos.Count();
                reportesSupervisor.totalAsignaciones = supervisorReportes?.ClientesAsignados.Count();
                reportesSupervisor.totalAsignacionesConVendedor = supervisorReportes?.ClientesAsignados.Count(x => x.IdUsuarioV != null);
                reportesSupervisor.totalAsignacionesProcesadas = supervisorReportes?.ClientesAsignados.Count(x => x.PesoTipificacionMayor != null);
                reportesSupervisor.totalGestionProcesada = supervisorReportes?.gESTIONDETALLEs.Count();
                var reporteTipificaciones = supervisorReportes?.Asesores?
                    .Select(asesor => new ViewTipificacionesAsesor
                    {
                        DniAsesor = asesor.Dni,
                        NombreAsesor = asesor.NombresCompletos,
                        totalAsignaciones = supervisorReportes.ClientesAsignados.Count(x => x.IdUsuarioV == asesor.IdUsuario),
                        totalTipificados = supervisorReportes.gESTIONDETALLEs.Count(x => x.DocAsesor == asesor.Dni),
                        totalDesembolsos = supervisorReportes.Desembolsos.Count(x => x.DocAsesor == asesor.Dni),
                        totalDerivaciones = supervisorReportes.DerivacionesSupervisor.Count(x => x.DniAsesor == asesor.Dni),
                        totalDerivacionesProcesadas = supervisorReportes.DerivacionesSupervisor.Count(x => x.DniAsesor == asesor.Dni && x.FueProcesado == true),
                        totalDerivacionesPendientes = supervisorReportes.DerivacionesSupervisor.Count(x => x.DniAsesor == asesor.Dni && x.FueProcesado == false)
                    })
                    .ToList() ?? new List<ViewTipificacionesAsesor>();
                reportesSupervisor.tipificacionesAsesores = reporteTipificaciones;
                var tipificacionesCantidad = new ViewTipificacionesCantidad();
                var tipificaciones = supervisorReportes?.gESTIONDETALLEs
                    .GroupBy(x => x.CodTip)
                    .Select(g => new ViewTipificacionesCantidad
                    {
                        TipoTipificacion = g.Key,
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