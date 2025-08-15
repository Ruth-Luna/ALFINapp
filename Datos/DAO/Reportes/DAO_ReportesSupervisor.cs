using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Datos.DAO.Miscelaneos;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Reportes
{
    public class DAO_ReportesSupervisor
    {
        private readonly DA_Usuario dA_Usuario = new DA_Usuario();
        private readonly DAO_ConsultasMiscelaneas _dao_ConsultasMiscelaneas;
        private readonly MDbContext _context;
        public DAO_ReportesSupervisor(
            DAO_ConsultasMiscelaneas dao_ConsultasMiscelaneas,
            MDbContext context)
        {
            _dao_ConsultasMiscelaneas = dao_ConsultasMiscelaneas;
            _context = context;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesSupervisor? Data)> getAllReportes(
            int idUsuario,
            int? anio = null,
            int? mes = null)
        {
            try
            {
                var supervisor = dA_Usuario.getUsuario(idUsuario);
                if (supervisor == null)
                {
                    return (false, "Supervisor no encontrado", null);
                }
                var supervisorReportes = await GetReportesEspecificoSupervisor(idUsuario, anio, mes);
                var reportesSupervisor = new ViewReportesSupervisor();
                reportesSupervisor.supervisor = new DetallesUsuarioDTO(supervisor).ToView();
                reportesSupervisor.asesores = supervisorReportes?.Asesores.Select(x => new DetallesUsuarioDTO(x).ToView()).ToList();
                reportesSupervisor.totalGestionado = supervisorReportes?.gESTIONDETALLEs.Count();
                reportesSupervisor.totalAsignaciones = supervisorReportes?.ClientesAsignados.Count();
                reportesSupervisor.totalSinGestionar = supervisorReportes?.ClientesAsignados.Count() - supervisorReportes?.gESTIONDETALLEs.Where(x => x.IdAsignacion != null).Count();
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

                var createDesembolsosFechaGestion = supervisorReportes?.Desembolsos
                    .GroupBy(x => x.FechaDesembolsos != null ? x.FechaDesembolsos.Value.ToString("%d/%M/%y") : "")
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
                var tipificacionesDescrp = await _dao_ConsultasMiscelaneas.GetTipificaciones();
                var tipificaciones = supervisorReportes?.gESTIONDETALLEs
                    .GroupBy(x => x.CodTip)
                    .Select(g => new ViewTipificacionesCantidad
                    {
                        IdTipificacion = g.Key,
                        TipoTipificacion = tipificacionesDescrp.Data.FirstOrDefault(t => t.idtip == g.Key).nombretip,
                        Cantidad = g.Count()
                    }).ToList() ?? new List<ViewTipificacionesCantidad>();
                return (true, "Reportes de supervisor obtenidos", reportesSupervisor);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<DetallesReportesSupervisorDTO> GetReportesEspecificoSupervisor(
            int idUsuario
            , int? anio = null
            , int? mes = null)
        {
            try
            {
                var usuario = await _context.usuarios
                    .Where(x => x.IdUsuario == idUsuario)
                    .FirstOrDefaultAsync();
                if (usuario == null)
                {
                    Console.WriteLine("Usuario no encontrado");
                    return new DetallesReportesSupervisorDTO();
                }
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                if (anio != null && mes != null)
                {
                    year = anio.Value;
                    month = mes.Value;
                }
                
                var getAsignaciones = await _context.clientes_asignados
                    .AsNoTracking()
                    .Where(x => x.IdUsuarioS == idUsuario
                        && x.FechaAsignacionSup.HasValue
                        && x.FechaAsignacionSup.Value.Year == year
                        && x.FechaAsignacionSup.Value.Month == month)
                    .Select(x => new ClientesAsignado { IdCliente = x.IdCliente, IdUsuarioV = x.IdUsuarioV })
                    .ToListAsync();

                var getAsesores = await _context
                    .usuarios
                    .Where(x => x.IDUSUARIOSUP == idUsuario)
                    .ToListAsync();
                
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@DniSupervisor", usuario.Dni),
                    new SqlParameter("@mes", month),
                    new SqlParameter("@anio", year)
                };

                var getGestionDetalles = await _context.reports_supervisor_gestion_fecha.FromSqlRaw(
                    "EXEC SP_Reportes_Supervisor_gestion @DniSupervisor, @mes, @anio",
                    parameters)
                    .AsNoTracking()
                    .ToListAsync();

                getGestionDetalles = getGestionDetalles
                    .GroupBy(x => x.DocCliente)
                    .Select(g => g.GroupBy(y => y.DocAsesor)
                        .Select(y => y.OrderByDescending(z => z.CodTip == 2)
                            .ThenByDescending(z => z.FechaGestion)
                            .First())
                        .OrderByDescending(z => z.FechaGestion)
                        .First())
                    .OrderBy(x => x.DocAsesor)
                    .ToList();

                var dniAsesores = getAsesores.Select(x => x.Dni).ToHashSet();
                var getDerivaciones = await _context
                    .derivaciones_asesores
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Supervisor_derivacion @DniSupervisor, @mes, @anio",
                        parameters)
                    .ToListAsync();

                var getDesembolsos = await _context
                    .desembolsos
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Supervisor_desembolso @DniSupervisor, @mes, @anio",
                        parameters)
                    .AsNoTracking()
                    .ToListAsync();

                var detallesReporte = new DetallesReportesSupervisorDTO(getAsignaciones, getAsesores, getDerivaciones, getDesembolsos, new List<DetallesReportesAsesorDTO>(), getGestionDetalles);
                return detallesReporte;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesSupervisorDTO();
            }
        }
    }
}