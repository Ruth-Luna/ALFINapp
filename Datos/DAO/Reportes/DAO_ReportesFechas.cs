using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Reportes
{
    public class DAO_ReportesFechas
    {
        private readonly MDbContext _context;
        public DAO_ReportesFechas(
            MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesFecha Data)> getReportByDate(
            string fecha,
            int idUsuario,
            int rol,
            int? mes = null,
            int? año = null)
        {
            try
            {
                if (string.IsNullOrEmpty(fecha))
                {
                    return (false, "La fecha no puede estar vacía", new ViewReportesFecha());
                }
                if (año == null || mes == null)
                {
                    var getReportesPie = await GetReportesGpieGeneralFecha(DateOnly.Parse(fecha), idUsuario);
                    if (getReportesPie == null)
                    {
                        return (false, "No se encontraron reportes para la fecha seleccionada", new ViewReportesFecha());
                    }
                    var getReportesFechas = new ViewReportesFecha();
                    getReportesFechas.ProgresoGeneral = getReportesPie;
                    return (true, "ok", getReportesFechas);
                }
                if (año != null || mes != null)
                {
#pragma warning disable CS8629 // Nullable value type may be null.
                    var getReportesPie = await GetReportesGpieGeneralFechaMeses(idUsuario, mes.Value, año.Value);
#pragma warning restore CS8629 // Nullable value type may be null.
                    if (getReportesPie == null)
                    {
                        return (false, "No se encontraron reportes para la fecha seleccionada", new ViewReportesFecha());
                    }
                    var getReportesTabla = await GetReportesTablaGeneralFechaMeses(idUsuario, mes.Value, año.Value);
                    if (getReportesTabla == null)
                    {
                        return (false, "No se encontraron reportes para la fecha seleccionada", new ViewReportesFecha());
                    }
                    var getReportesFechas = new ViewReportesFecha();
                    getReportesFechas.ProgresoGeneral = getReportesPie;
                    getReportesFechas.reporteTablaPorMeses = getReportesTabla;
                    return (true, "ok", getReportesFechas);
                }
                else
                {
                    return (false, "No se encontraron reportes para la fecha seleccionada", new ViewReportesFecha());
                }
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewReportesFecha());
            }
        }
        public async Task<ViewReportePieGeneral> GetReportesGpieGeneralFecha(DateOnly fecha, int idUsuario)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@FechaBusqueda", fecha),
                    new SqlParameter("@IdUsuario", idUsuario)
                };
                var getDataDer = await _context.reports_g_pie_derivados_desembolsados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_POR_FECHAS_GESTION_DERIVACION_DESEMBOLSO @FechaBusqueda, @IdUsuario",
                        parameters)
                    .ToListAsync();
                var getDataGes = await _context.reports_g_pie_gestion_asignados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_POR_FECHAS_GESTIONADOS_SOBRE_ASIGNADOS @FechaBusqueda, @IdUsuario",
                        parameters)
                    .ToListAsync();
                if (getDataDer == null || getDataDer.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta. Se pasara una lista vacia");
                    getDataDer = new List<ReportsGPiePorcentajeGestionadoDerivadoDesembolsado>();
                }
                if (getDataGes == null || getDataGes.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    getDataGes = new List<ReportsGPiePorcentajeGestionadosSobreAsignados>();
                }
                var convertDto = new ViewReportePieGeneral(getDataGes.FirstOrDefault(), getDataDer.FirstOrDefault());
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ViewReportePieGeneral();
            }
        }
        public async Task<ViewReportePieGeneral> GetReportesGpieGeneralFechaMeses(int idUsuario, int mes, int año)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@mes", mes),
                    new SqlParameter("@anio", año),
                    new SqlParameter("@IdUsuario", idUsuario)
                };
                var getData = await _context.reports_g_pie_gestion_asignados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_POR_FECHAS_GESTIONADOS_SOBRE_ASIGNADOS_POR_MES @mes, @anio, @IdUsuario",
                        parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new ViewReportePieGeneral();
                }
                var convertDto = new ViewReportePieGeneral(getData.FirstOrDefault() ?? new ReportsGPiePorcentajeGestionadosSobreAsignados(),
                    new ReportsGPiePorcentajeGestionadoDerivadoDesembolsado());
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ViewReportePieGeneral();
            }
        }
        public async Task<List<ViewReporteTablaMeses>> GetReportesTablaGeneralFechaMeses(int idUsuario, int mes, int año)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@mes", mes),
                    new SqlParameter("@año", año),
                    new SqlParameter("@IdUsuario", idUsuario),
                };
                var getData = await _context.reports_g_pie_derivados_desembolsados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_POR_FECHAS_GESTION_DERIVACION_DESEMBOLSO_POR_MES @mes, @año, @IdUsuario",
                        parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new List<ViewReporteTablaMeses>();
                }
                return getData.Select(item => new ViewReporteTablaMeses(item)).ToList();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error al obtener los datos de etiquetas de metas: " + ex.Message);
                return new List<ViewReporteTablaMeses>();
            }
        }
    }
}