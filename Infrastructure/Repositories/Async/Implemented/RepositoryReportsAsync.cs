using ALFINapp.Application.DTOs;
using ALFINapp.Infrastructure.Persistence.Procedures;
using ALFINapp.Infrastructure.Repositories.Async.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories.Async.Implemented
{
    public class RepositoryReportsAsync : IRepositoryReportsAsync
    {
        private readonly IDbContextFactory<MDbContext> _dbContextFactory;

        public RepositoryReportsAsync(IDbContextFactory<MDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<(
            DetallesReportesLineaGestionVsDerivacionDTO linea,
            DetallesReportesGpieDTO pie,
            DetallesReportesBarDTO bar,
            DetallesReportesTablasDTO tabla,
            DetallesReportesGpieDTO pie2,
            DetallesReportesEtiquetasDTO etiquetas)> GetReportesAsync(int idUsuario)
        {
            try
            {
                var context1 = await _dbContextFactory.CreateDbContextAsync();
                var context2 = await _dbContextFactory.CreateDbContextAsync();
                var context3 = await _dbContextFactory.CreateDbContextAsync();
                var context4 = await _dbContextFactory.CreateDbContextAsync();
                var context5 = await _dbContextFactory.CreateDbContextAsync();
                var context6 = await _dbContextFactory.CreateDbContextAsync();
                var context7 = await _dbContextFactory.CreateDbContextAsync();

                var param = new SqlParameter("@id_usuario", idUsuario);

                var parameters = new SqlParameter[]
                    {
                    new SqlParameter("@id_usuario", idUsuario),
                    new SqlParameter("@doc_busqueda", DBNull.Value),
                    };
                var taskLinea = context1.reports_g_lineas_gestion_vs_derivacion_diaria
                    .FromSqlRaw("EXEC SP_REPORTES_GLINEAS_GESTION_VS_DERIVACION_DIARIA @id_usuario", param)
                    .AsNoTracking()
                    .ToListAsync();

                var taskPie1 = context2.reports_g_pie_derivados_desembolsados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_PORCENTAJE_GESTIONADO_DERIVADO_DESEMBOLSADO @id_usuario", param)
                    .AsNoTracking()
                    .ToListAsync();

                var taskPie2 = context3.reports_g_pie_gestion_asignados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_PORCENTAJE_GESTIONADOS_SOBRE_ASIGNADOS @id_usuario", param)
                    .AsNoTracking()
                    .ToListAsync();

                var taskBar1 = context4.reports_bar_top_5_derivaciones
                    .FromSqlRaw("EXEC SP_REPORTES_BAR_TOP_5_DERIVACIONES @id_usuario", new SqlParameter("@id_usuario", idUsuario))
                    .AsNoTracking()
                    .ToListAsync();

                var taskTabla = context5.reports_tabla_gestionado_derivado_desembolsado_importe
                    .FromSqlRaw("EXEC SP_REPORTES_TABLA_GESTIONADO_DERIVADO_DESEMBOLSADO_IMPORTE")
                    .AsNoTracking()
                    .ToListAsync();

                var taskPie3 = context6.reports_pie_contactabilidad_cliente
                    .FromSqlRaw("EXEC SP_REPORTES_PIE_CONTACTABILIDAD_CLIENTE @id_usuario", new SqlParameter("@id_usuario", idUsuario))
                    .AsNoTracking()
                    .ToListAsync();

                var taskEtiquetas = context7.reports_desembolsos_n_monto
                    .FromSqlRaw("EXEC SP_Reportes_desembolsos_n_y_monto @id_usuario, @doc_busqueda", parameters)
                    .AsNoTracking()
                    .ToListAsync();

                await Task.WhenAll(
                    taskLinea,
                    taskPie1,
                    taskPie2,
                    taskBar1,
                    taskTabla,
                    taskPie3,
                    taskEtiquetas);

                var lineaData = taskLinea.Result;
                var pieData1 = taskPie1.Result;
                var pieData2 = taskPie2.Result;
                var barData1 = taskBar1.Result;
                var tablaData = taskTabla.Result;
                var pieData3 = taskPie3.Result;
                var etiquetasData = taskEtiquetas.Result;

                if (lineaData == null || !lineaData.Any())
                {
                    Console.WriteLine("No se encontraron datos para SP_REPORTES_GLINEAS_GESTION_VS_DERIVACION_DIARIA.");
                    lineaData = new List<ReportsGLineasGestionVsDerivacionDiaria>();
                }

                if (pieData1 == null || !pieData1.Any() || pieData2 == null || !pieData2.Any())
                {
                    Console.WriteLine("No se encontraron datos para uno de los pies.");
                    pieData1 = new List<ReportsGPiePorcentajeGestionadoDerivadoDesembolsado>();
                    pieData2 = new List<ReportsGPiePorcentajeGestionadosSobreAsignados>();
                }

                if (barData1 == null || !barData1.Any())
                {
                    Console.WriteLine("No se encontraron datos para SP_REPORTES_BAR_TOP_5_DERIVACIONES.");
                    barData1 = new List<ReportsBarTop5Derivaciones>();
                }

                if (tablaData == null || !tablaData.Any())
                {
                    Console.WriteLine("No se encontraron datos para SP_REPORTES_TABLA_GESTIONADO_DERIVADO_DESEMBOLSADO_IMPORTE.");
                    tablaData = new List<ReportsTablaGestionadoDerivadoDesembolsadoImporte>();
                }

                if (pieData3 == null || !pieData3.Any())
                {
                    Console.WriteLine("No se encontraron datos para SP_REPORTES_PIE_CONTACTABILIDAD_CLIENTE.");
                    pieData3 = new List<ReportsPieContactabilidadCliente>();
                }

                if (etiquetasData == null || !etiquetasData.Any() || etiquetasData.FirstOrDefault() == null)
                {
                    Console.WriteLine("No se encontraron datos para SP_Reportes_desembolsos_n_y_monto.");
                    etiquetasData = new List<ReportsDesembolsosNMonto>();
                }

                var etiqueta = etiquetasData.FirstOrDefault();

                if (etiqueta == null)
                {
                    Console.WriteLine("No se encontraron datos para la etiqueta.");
                    etiqueta = new ReportsDesembolsosNMonto();
                }

                var dtoLinea = new DetallesReportesLineaGestionVsDerivacionDTO(lineaData);
                var dtoPie = new DetallesReportesGpieDTO(pieData2.FirstOrDefault(), pieData1.FirstOrDefault());
                var dtoBar = new DetallesReportesBarDTO(barData1);
                var dtoTabla = new DetallesReportesTablasDTO(tablaData);
                var dtoPie2 = new DetallesReportesGpieDTO(pieData3);
                var dtoEtiquetas = new DetallesReportesEtiquetasDTO(etiqueta);
                return (dtoLinea, dtoPie, dtoBar, dtoTabla, dtoPie2, dtoEtiquetas);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error en RepositoryReportsAsync: {ex.Message} . ");
                return (new DetallesReportesLineaGestionVsDerivacionDTO(), new DetallesReportesGpieDTO(), new DetallesReportesBarDTO(), new DetallesReportesTablasDTO(), new DetallesReportesGpieDTO(), new DetallesReportesEtiquetasDTO());
            }
        }
    }
}
