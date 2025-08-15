using ALFINapp.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Reportes
{
    public class DAO_GetReportes
    {
        private DA_Usuario _da_usuario = new DA_Usuario();
        private readonly DAO_GetReportesAsync _dao_getReportesAsync;
        private readonly MDbContext _context;
        public DAO_GetReportes(MDbContext context, DAO_GetReportesAsync dao_getReportesAsync)
        {
            _context = context;
            _dao_getReportesAsync = dao_getReportesAsync;
        }
        public async Task<(bool IsSuccess, string Message, ViewReportesGeneral? Data)> getAllReportes(
            int idUsuario,
            int? anio = null,
            int? mes = null)
        {
            try
            {
                if (idUsuario <= 0)
                {
                    return (false, "Id de usuario no valido", null);
                }
                if (anio == null || mes == null)
                {
                    anio = DateTime.Now.Year;
                    mes = DateTime.Now.Month;
                }
                var user = _da_usuario.getUsuario(idUsuario);
                if (user == null)
                {
                    return (false, "Usuario no encontrado", null);
                }
                var reporteGeneral = new ViewReportesGeneral();
                if (user.IdRol == 1 || user.IdRol == 4 || user.IdRol == 2)
                {
                    var usuarios = _da_usuario.ListarAsesores(idUsuario);
                    var usuariosViews = new List<ViewUsuario>();

                    foreach (var item in usuarios)
                    {
                        var usuarioView = new ViewUsuario(item);
                        usuariosViews.Add(usuarioView);
                    }
                    reporteGeneral.Asesores = usuariosViews;

                    if (user.IdRol == 1 || user.IdRol == 4)
                    {
                        var supervisores = _da_usuario.ListarSupervisores();
                        var supervisoresViews = new List<ViewUsuario>();

                        foreach (var item in supervisores)
                        {
                            var supervisorView = new ViewUsuario(item);
                            supervisoresViews.Add(supervisorView);
                        }
                        reporteGeneral.Supervisores = supervisoresViews;
                    }
                    var reportesAsync = await _dao_getReportesAsync.GetReportesAsync(idUsuario, anio, mes);
                    var reportesEtiquetas = await GetReportesEtiquetasMetas(anio, mes);
                    reporteGeneral.lineaGestionVsDerivacion = reportesAsync.linea;
                    reporteGeneral.ProgresoGeneral = reportesAsync.pie;
                    reporteGeneral.top5asesores = reportesAsync.bar;
                    reporteGeneral.reporteTablaGeneral = reportesAsync.tabla;
                    reporteGeneral.pieContactabilidad = reportesAsync.pie2;
                    reporteGeneral.etiquetas = new List<ViewEtiquetas>();
                    reporteGeneral.etiquetas.AddRange(reportesAsync.etiquetas);
                    reporteGeneral.etiquetas.AddRange(reportesEtiquetas);
                    if (mes != null && anio != null)
                    {
                        reporteGeneral.filtro_por_fechas = true;
                        var fechafiltro = new FechaDelFiltro();
                        fechafiltro.mes = mes;
                        fechafiltro.anio = anio;
                        reporteGeneral.fecha_filtro = fechafiltro;
                    }
                    return (true, "Reportes obtenidos correctamente", reporteGeneral);
                }
                else if (user.IdRol == 3)
                {
                    var reportesAsync = await _dao_getReportesAsync.GetReportesAsync(idUsuario, anio, mes);
                    var reportesEtiquetas = await GetReportesEtiquetasMetas(anio, mes);
                    reporteGeneral.lineaGestionVsDerivacion = reportesAsync.linea;
                    reporteGeneral.ProgresoGeneral = reportesAsync.pie;
                    reporteGeneral.pieContactabilidad = reportesAsync.pie2;
                    reporteGeneral.etiquetas = new List<ViewEtiquetas>();
                    reporteGeneral.etiquetas.AddRange(reportesAsync.etiquetas);
                    reporteGeneral.etiquetas.AddRange(reportesEtiquetas);
                    return (true, "Reportes obtenidos correctamente", reporteGeneral);
                }
                else
                {
                    return (false, "Rol no permitido", null);
                }
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<List<ViewEtiquetas>> GetReportesEtiquetasMetas(
            int? anio = null,
            int? mes = null
        )
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@mes", mes ?? (object)DBNull.Value),
                    new SqlParameter("@anio", anio ?? (object)DBNull.Value)
                };
                var getData = await _context.reports_etiqueta_meta_importe
                    .FromSqlRaw("EXEC SP_Reportes_etiqueta_meta_importe @mes, @anio",
                        parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new List<ViewEtiquetas>();
                }
                var convertDto = new List<ViewEtiquetas>();
                foreach (var item in getData)
                {
                    convertDto.Add(new ViewEtiquetas(item));
                }
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al obtener etiquetas y metas: {ex.Message}");
                return new List<ViewEtiquetas>();
            }
        }
    }
}