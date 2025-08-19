using ALFINapp.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Reportes
{
    public class DAO_ReportesSupervisor
    {
        private readonly DA_Usuario dA_Usuario = new DA_Usuario();
        private readonly MDbContext _context;
        public DAO_ReportesSupervisor(MDbContext context)
        {
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
                reportesSupervisor.supervisor = new ViewUsuario(supervisor);
                var asesores = dA_Usuario.ListarAsesores(idUsuario);
                reportesSupervisor.asesores = asesores.Select(x => new ViewUsuario(x)).ToList();

                reportesSupervisor.totalGestionado = supervisorReportes.totalGestionado;
                reportesSupervisor.totalAsignaciones = supervisorReportes?.totalAsignaciones;
                reportesSupervisor.totalSinGestionar = supervisorReportes?.totalSinGestionar;
                reportesSupervisor.totalDerivado = supervisorReportes?.totalDerivado;
                reportesSupervisor.totalDesembolsado = supervisorReportes?.totalDesembolsado;

                var asesoresDni = asesores.Select(a => a.Dni).ToList();

                var createDerivacionesFecha = await _context.GESTION_DETALLE
                    .Where(x => x.CodTip == 2
                            && asesoresDni.Contains(x.DocAsesor)
                            && x.FechaGestion.Month == mes
                            && x.FechaGestion.Year == anio)
                    .GroupBy(x => x.FechaGestion.Date)
                    .Select(g => new
                    {
                        Fecha = g.Key,
                        Contador = g.Count()
                    })
                    .OrderBy(g => g.Fecha)
                    .ToListAsync();

                var derivacionesFecha = createDerivacionesFecha
                    .Select(g => new DerivacionesFecha
                    {
                        Fecha = g.Fecha.ToString("d/M/yy"),
                        Contador = g.Contador
                    })
                    .ToList();

                reportesSupervisor.derivacionesFecha = derivacionesFecha;

#pragma warning disable CS8629 // Nullable value type may be null.
                var createDesembolsosFecha = await _context.desembolsos
                    .Where(x => asesoresDni.Contains(x.DocAsesor)
                            && x.FechaDesembolsos.HasValue
                            && x.FechaDesembolsos.Value.Month == mes
                            && x.FechaDesembolsos.Value.Year == anio)
                    .GroupBy(x => x.FechaDesembolsos.Value.Date)
                    .Select(g => new
                    {
                        Fecha = g.Key,
                        Contador = g.Count()
                    })
                    .OrderBy(g => g.Fecha)
                    .ToListAsync();
#pragma warning restore CS8629 // Nullable value type may be null.
                var desembolsosFecha = createDesembolsosFecha
                    .Select(g => new DerivacionesFecha
                    {
                        Fecha = g.Fecha.ToString("d/M/yy"),
                        Contador = g.Contador
                    })
                    .ToList();

                reportesSupervisor.desembolsosFecha = desembolsosFecha;

                var reporteTipificaciones = await GetReportesAsesorDelSupervisor(idUsuario, anio, mes);
                if (reporteTipificaciones == null || reporteTipificaciones.Count == 0)
                {
                    reporteTipificaciones = new List<ViewTipificacionesAsesor>();
                }
                reportesSupervisor.tipificacionesAsesores = reporteTipificaciones;

                return (true, "Reportes de supervisor obtenidos", reportesSupervisor);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<ViewReportesSupervisor> GetReportesEspecificoSupervisor(
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
                    return new ViewReportesSupervisor();
                }
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                if (anio != null && mes != null)
                {
                    year = anio.Value;
                    month = mes.Value;
                }

                var getAsignaciones = _context
                    .clientes_asignados
                    .AsNoTracking()
                    .Where(x => x.IdUsuarioS == idUsuario
                        && x.FechaAsignacionSup.HasValue
                        && x.FechaAsignacionSup.Value.Year == year
                        && x.FechaAsignacionSup.Value.Month == month)
                    .Count();

                var getAsesores = await _context
                    .usuarios
                    .Where(x => x.IDUSUARIOSUP == idUsuario)
                    .ToListAsync();

                var getGestionDetalles = _context
                    .GESTION_DETALLE
                    .Where(x => x.DocAsesor == usuario.Dni
                        && x.FechaGestion.Year == year
                        && x.FechaGestion.Month == month)
                    .AsNoTracking()
                    .Count();

                var dniAsesores = getAsesores.Select(x => x.Dni).ToHashSet();
                var getDerivaciones = await _context
                    .derivaciones_asesores
                    .Where(x => dniAsesores.Contains(x.DniAsesor)
                        && x.FechaDerivacion.Year == year
                        && x.FechaDerivacion.Month == month)
                    .AsNoTracking()
                    .CountAsync();

                var getDesembolsos = await _context
                    .desembolsos
                    .Where(x => x.DocAsesor == usuario.Dni
                        && x.FechaDesembolsos.HasValue
                        && x.FechaDesembolsos.Value.Year == year
                        && x.FechaDesembolsos.Value.Month == month)
                    .AsNoTracking()
                    .CountAsync();

                
                var detallesReporte = new ViewReportesSupervisor();
                detallesReporte.totalGestionado = getGestionDetalles;
                
                detallesReporte.totalDerivado = getDerivaciones;
                detallesReporte.totalDesembolsado = getDesembolsos;
                detallesReporte.totalAsignaciones = getAsignaciones;

                detallesReporte.totalSinGestionar = getAsignaciones - getGestionDetalles >= 0
                    ? getAsignaciones - getGestionDetalles
                    : 0;
                return detallesReporte;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ViewReportesSupervisor();
            }
        }

        public async Task<List<ViewTipificacionesAsesor>> GetReportesAsesorDelSupervisor(
            int idUsuarioS,
            int? anio = null,
            int? mes = null)
        {
            try
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                if (anio != null && mes != null)
                {
                    year = anio.Value;
                    month = mes.Value;
                }

                var parameters = new[]
                {
                    new SqlParameter("@idSupervisor", idUsuarioS),
                    new SqlParameter("@mes", mes ?? (object)DBNull.Value),
                    new SqlParameter("@anio", anio ?? (object)DBNull.Value)
                };
                
                var result = await _context.reports_supervisor_asesores_del_supervisor.FromSqlRaw(
                    "EXEC SP_REPORTES_SUPERVISOR_ASESORES_DEL_SUPERVISOR @idSupervisor, @mes, @anio",
                    parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (result == null || result.Count == 0)
                {
                    Console.WriteLine("No se encontraron resultados");
                    return new List<ViewTipificacionesAsesor>();
                }

                var tipificacionPorAsesor = result
                    .Select(x => new ViewTipificacionesAsesor(x))
                    .ToList();

                return tipificacionPorAsesor;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ViewTipificacionesAsesor>();
            }
        } 
    }
}