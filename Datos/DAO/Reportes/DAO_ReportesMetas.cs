using ALFINapp.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Reportes
{
    public class DAO_ReportesMetas
    {
        private readonly MDbContext _context;
        public DAO_ReportesMetas(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesMetas Data)> getReportsByGoal(int idUsuario)
        {
            try
            {
                var metas = await GetReportesMetas(idUsuario);
                var reportesGeneral = await GetReportesByDate(idUsuario);
                if (metas == null)
                {
                    return (false, "No se encontraron metas para el usuario.", new ViewReportesMetas());
                }
                var reporteMetas = new ViewReportesMetas();
                reporteMetas.metas = metas;
                reporteMetas.totalGestiones = reporteMetas.metas.Sum(x => x.totalGestion);
                reporteMetas.totalImporte = reporteMetas.metas.Sum(x => x.totalImporte);
                reporteMetas.totalDerivaciones = reporteMetas.metas.Sum(x => x.totalDerivaciones);
                reporteMetas.pieFechas = reportesGeneral;
                reporteMetas.pieFechas.PERIODO = DateTime.Now.ToString("dd/MM/yyyy");
                reporteMetas.pieFechas.estado = "Reporte General de Metas";
                return (true, "OK", reporteMetas);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return (false, ex.Message, new ViewReportesMetas());
            }
        }
        public async Task<List<ViewMetas>> GetReportesMetas(int idUsuario)
        {
            try
            {
                var getData = await _context.reports_tablas_metas
                    .FromSqlRaw("EXEC SP_Reportes_TABLAS_Metas @id_usuario",
                        new SqlParameter("@id_usuario", idUsuario))
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new List<ViewMetas>();
                }
                return getData.Select(item => new ViewMetas(item)).ToList();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ViewMetas>();
            }
        }
        public async Task<ViewReportePieGeneral> GetReportesByDate(int idUsuario, DateTime? fecha = null)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@id_usuario", idUsuario),
                    new SqlParameter("@fecha", fecha ?? (object)DBNull.Value)
                };
                var getData = await _context.reports_general_datos_actuales
                    .FromSqlRaw("EXEC sp_Reporte_general_datos_actuales_por_id_usuario_fecha @id_usuario, @fecha",
                        parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new ViewReportePieGeneral();
                }
                var convertDto = new ViewReportePieGeneral(getData);
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ViewReportePieGeneral();
            }
        }
    }
}