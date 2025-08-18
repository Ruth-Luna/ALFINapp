using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Datos.DAO.Miscelaneos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Reportes
{
    public class DAO_ReportesAsesor
    {
        private DA_Usuario dA_Usuario = new DA_Usuario();
        private readonly MDbContext _context;
        private readonly DAO_ConsultasMiscelaneas _dao_ConsultasMiscelaneas;
        public DAO_ReportesAsesor(
            MDbContext context,
            DAO_ConsultasMiscelaneas dao_ConsultasMiscelaneas)
        {
            _context = context;
            _dao_ConsultasMiscelaneas = dao_ConsultasMiscelaneas;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesAsesores? Data)> getAllReportes(
            int idUsuario,
            int anio,
            int mes)
        {
            try
            {
                var usuario = dA_Usuario.getUsuario(idUsuario);
                if (usuario == null)
                {
                    return (false, "Usuario no encontrado", null);
                }
                var reportes = await GetReportesAsesor(idUsuario, anio, mes);
                var viewReportes = new ViewReportesAsesores();
                viewReportes.asesor = new ViewUsuario(usuario);
                viewReportes.totalDerivaciones = reportes.totalDerivaciones;
                viewReportes.totalDesembolsos = reportes.totalDesembolsos;
                viewReportes.totalAsignado = reportes.totalAsignado;
                viewReportes.totalGestionado = reportes.totalGestionado;
                viewReportes.totalSinGestionar = reportes.totalSinGestionar;

                var createDerivacionesFecha = await _context.GESTION_DETALLE
                    .Where(x => x.CodTip == 2
                            && x.DocAsesor == usuario.Dni
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

                viewReportes.derivacionesFecha = derivacionesFecha;

#pragma warning disable CS8629 // Nullable value type may be null.
                var createDesembolsosFecha = await _context.desembolsos
                    .Where(x => x.DocAsesor == usuario.Dni
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

                viewReportes.desembolsosFecha = desembolsosFecha;

                var TipificacionesGestion = new List<ViewTipificacionesGestion>();

                var agruparTipificaciones = await _context.reports_asesor_tipificaciones_top
                    .FromSqlRaw("EXEC SP_REPORTES_ASESOR_TIPIFICACIONES_TOP @Dni = {0}, @Mes = {1}, @Anio = {2}", 
                        usuario.Dni, mes, anio)
                    .ToListAsync();

                viewReportes.tipificacionesGestion = agruparTipificaciones
                    .Select(x => new ViewTipificacionesGestion(x))
                    .ToList();
                return (true, "Reportes obtenidos correctamente", viewReportes);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<ViewReportesAsesores> GetReportesAsesor(
            int idUsuario,
            int? anio = null,
            int? mes = null)
        {
            try
            {
                var getUsuario = await _context.usuarios
                    .Where(x => x.IdUsuario == idUsuario)
                    .FirstOrDefaultAsync();
                if (getUsuario == null)
                {
                    Console.WriteLine("Usuario no encontrado");
                    return new ViewReportesAsesores();
                }
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@DniAsesor", getUsuario.Dni),
                    new SqlParameter("@mes", mes ?? (object)DBNull.Value),
                    new SqlParameter("@anio", anio ?? (object)DBNull.Value)
                };

                var asignaciones = await _context.numeros_enteros_dto
                    .FromSqlRaw(
                        "EXEC SP_REPORTES_ASIGNACIONES_ASESOR @DniAsesor, @mes, @anio",
                        parameters)
                    .ToListAsync();

                var asignacionContador = asignaciones.FirstOrDefault()?.NumeroEntero ?? 0;
                if (asignacionContador == 0)
                {
                    Console.WriteLine("No se encontraron asignaciones");
                }

                var getallids = await _context.clientes_asignados
                    .Where(x => x.IdUsuarioV == idUsuario
                        && x.FechaAsignacionSup.HasValue
                        && x.FechaAsignacionSup.Value.Month == mes
                        && x.FechaAsignacionSup.Value.Year == anio)
                    .Select(x => x.IdAsignacion)
                    .ToHashSetAsync();

                var derivaciones = await _context.numeros_enteros_dto
                    .FromSqlRaw(
                        "EXEC SP_REPORTES_DERIVACIONES_ASESOR @DniAsesor, @mes, @anio",
                        parameters)
                    .AsNoTracking()
                    .ToListAsync();
                var derivacionContador = derivaciones.FirstOrDefault()?.NumeroEntero ?? 0;
                if (derivacionContador == 0)
                {
                    Console.WriteLine("No se encontraron derivaciones");
                }

                var gestiones = await _context.numeros_enteros_dto
                    .FromSqlRaw(
                        "EXEC SP_REPORTES_GESTION_ASESOR @DniAsesor, @mes, @anio",
                        parameters)
                    .AsNoTracking()
                    .ToListAsync();

                var gestionContador = gestiones.FirstOrDefault()?.NumeroEntero ?? 0;
                if (gestionContador == 0)
                {
                    Console.WriteLine("No se encontraron gestiones");
                }

                var desembolsos = await _context.numeros_enteros_dto
                    .FromSqlRaw(
                        "EXEC SP_REPORTES_DESEMBOLSOS_ASESOR @DniAsesor, @mes, @anio",
                        parameters)
                    .AsNoTracking()
                    .ToListAsync();

                var desembolsosContador = desembolsos.FirstOrDefault()?.NumeroEntero ?? 0;
                if (desembolsosContador == 0)
                {
                    Console.WriteLine("No se encontraron desembolsos");
                }

                var detallesReporte = new ViewReportesAsesores();
                detallesReporte.totalAsignado = asignacionContador;
                detallesReporte.totalDerivaciones = derivacionContador;
                detallesReporte.totalGestionado = gestionContador;
                detallesReporte.totalDesembolsos = desembolsosContador;
                return detallesReporte;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ViewReportesAsesores();
            }
        }
    }
}