using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryReports : IRepositoryReports
    {
        MDbContext _context;
        public RepositoryReports(MDbContext context)
        {
            _context = context;
        }
        public async Task<DetallesReportesAsesorDTO> GetReportesAsesor(int idUsuario)
        {
            try
            {
                var getUsuario = await _context.usuarios
                    .Where(x => x.IdUsuario == idUsuario)
                    .FirstOrDefaultAsync();
                if (getUsuario == null)
                {
                    Console.WriteLine("Usuario no encontrado");
                    return new DetallesReportesAsesorDTO();
                }
                var getAllAsignaciones = await _context.clientes_asignados
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Asesor_asignaciones @DniAsesor",
                        new SqlParameter("@DniAsesor", getUsuario.Dni))
                    .ToListAsync();

                var getAllIds = getAllAsignaciones.Select(x => x.IdAsignacion).ToHashSet();
                var getAllDerivaciones = await _context.derivaciones_asesores
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Asesor_derivacion @DniAsesor",
                        new SqlParameter("@DniAsesor", getUsuario.Dni))
                    .AsNoTracking()
                    .ToListAsync();

                var getAllGestionDetalle = await _context.GESTION_DETALLE
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Asesor_gestion @DniAsesor",
                        new SqlParameter("@DniAsesor", getUsuario.Dni))
                    .AsNoTracking()
                    .ToListAsync();

                getAllGestionDetalle = getAllGestionDetalle.GroupBy(g => g.DocCliente)
                    .Select(g => g.OrderByDescending(d => d.CodTip == 2)
                        .ThenByDescending(d => d.FechaGestion)
                        .First())
                    .ToList();

                var getAllDesembolsos = await _context.desembolsos
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Asesor_desembolsos @DniAsesor",
                        new SqlParameter("@DniAsesor", getUsuario.Dni))
                    .AsNoTracking()
                    .ToListAsync();

                var detallesReporte = new DetallesReportesAsesorDTO();
                detallesReporte.Usuario = getUsuario;
                detallesReporte.ClientesAsignados = getAllAsignaciones;
                detallesReporte.DerivacionesDelAsesor = getAllDerivaciones;
                detallesReporte.gESTIONDETALLEs = getAllGestionDetalle;
                detallesReporte.Desembolsos = getAllDesembolsos;
                return detallesReporte;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesAsesorDTO();
            }
        }
        public async Task<DetallesReportesSupervisorDTO> GetReportesEspecificoSupervisor(int idUsuario)
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

                var getGestionDetalles = await _context.GESTION_DETALLE.FromSqlRaw(
                    "EXEC SP_Reportes_Supervisor_gestion @DniSupervisor",
                    new SqlParameter("@DniSupervisor", usuario.Dni))
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
                        "EXEC SP_Reportes_Supervisor_derivacion @DniSupervisor",
                        new SqlParameter("@DniSupervisor", usuario.Dni))
                    .ToListAsync();

                var getDesembolsos = await _context
                    .desembolsos
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Supervisor_desembolso @DniSupervisor",
                        new SqlParameter("@DniSupervisor", usuario.Dni))
                    .AsNoTracking()
                    .ToListAsync();

                var detallesReporte = new DetallesReportesSupervisorDTO();
                detallesReporte.Asesores = getAsesores;
                detallesReporte.ClientesAsignados = getAsignaciones;
                detallesReporte.DerivacionesSupervisor = getDerivaciones;
                detallesReporte.Desembolsos = getDesembolsos;
                detallesReporte.gESTIONDETALLEs = getGestionDetalles;
                return detallesReporte;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesSupervisorDTO();
            }
        }
        public async Task<DetallesReportesLineaGestionVsDerivacionDTO> LineaGestionVsDerivacionDiaria(int idUsuario)
        {
            try
            {
                var getData = await _context.reports_g_lineas_gestion_vs_derivacion_diaria
                    .FromSqlRaw("EXEC SP_REPORTES_GLINEAS_GESTION_VS_DERIVACION_DIARIA @id_usuario", new SqlParameter("@id_usuario", idUsuario))
                    .AsNoTracking()
                    .ToListAsync();

                var convertDto = new DetallesReportesLineaGestionVsDerivacionDTO(getData);
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesLineaGestionVsDerivacionDTO();
                }

                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesLineaGestionVsDerivacionDTO();
            }
        }
        public async Task<DetallesReportesGpieDTO> GetReportesGpieGeneral(int idUsuario)
        {
            try
            {
                var getData = await _context.reports_g_pie_derivados_desembolsados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_PORCENTAJE_GESTIONADO_DERIVADO_DESEMBOLSADO @id_usuario", new SqlParameter("@id_usuario", idUsuario))
                    .AsNoTracking()
                    .ToListAsync();
                var getData2 = await _context.reports_g_pie_gestion_asignados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_PORCENTAJE_GESTIONADOS_SOBRE_ASIGNADOS @id_usuario", new SqlParameter("@id_usuario", idUsuario))
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesGpieDTO();
                }
                if (getData2 == null || getData2.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesGpieDTO();
                }
                var convertDto = new DetallesReportesGpieDTO(getData2.FirstOrDefault(), getData.FirstOrDefault());
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesGpieDTO();
            }
        }

        public async Task<DetallesReportesGpieDTO> GetReportesGpieGeneralFecha(DateOnly fecha, int idUsuario)
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
                var convertDto = new DetallesReportesGpieDTO(getDataGes.FirstOrDefault(), getDataDer.FirstOrDefault());
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesGpieDTO();
            }
        }
        public async Task<DetallesReportesGpieDTO> GetReportesPieContactabilidadCliente(int idUsuario)
        {
            try
            {
                var getData = await _context.reports_pie_contactabilidad_cliente
                    .FromSqlRaw("EXEC SP_REPORTES_PIE_CONTACTABILIDAD_CLIENTE @id_usuario", new SqlParameter("@id_usuario", idUsuario))
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesGpieDTO();
                }
                var convertDto = new DetallesReportesGpieDTO(getData);
                return convertDto;
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error al obtener los datos de contactabilidad del cliente.");
                return new DetallesReportesGpieDTO();
            }
        }
        public async Task<DetallesReportesEtiquetasDTO> GetReportesEtiquetasDesembolsosNImportes(int idUsuario)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@id_usuario", idUsuario),
                    new SqlParameter("@doc_busqueda", DBNull.Value),
                };
                var getData = await _context.reports_desembolsos_n_monto
                    .FromSqlRaw("EXEC SP_Reportes_desembolsos_n_y_monto @id_usuario, @doc_busqueda", parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesEtiquetasDTO();
                }
                var firstData = getData.FirstOrDefault();
                if (firstData == null)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesEtiquetasDTO();
                }
                var convertDto = new DetallesReportesEtiquetasDTO(firstData);
                return convertDto;
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error al obtener los datos de etiquetas de desembolsos por importe.");
                return new DetallesReportesEtiquetasDTO();
            }
        }
        public async Task<DetallesReportesTablasDTO> GetReportesMetas(int idUsuario)
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
                    return new DetallesReportesTablasDTO();
                }
                return new DetallesReportesTablasDTO(getData);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesTablasDTO();
            }
        }

        public async Task<DetallesReportesGpieDTO> GetReportesByDate(int idUsuario, DateTime? fecha = null)
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
                    return new DetallesReportesGpieDTO();
                }
                var convertDto = new DetallesReportesGpieDTO(getData);
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesGpieDTO();
            }
        }

        public async Task<DetallesReportesGpieDTO> GetReportesByActualMonth(int idUsuario)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@id_usuario", idUsuario)
                };
                var getData = await _context.reports_general_datos_actuales
                    .FromSqlRaw("EXEC sp_Reporte_general_datos_actuales_por_id_usuario_actual_month @id_usuario",
                        parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesGpieDTO();
                }
                var convertDto = new DetallesReportesGpieDTO(getData);
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesGpieDTO();
            }
        }

        public async Task<DetallesReportesEtiquetasDTO> GetReportesEtiquetasMetas()
        {
            try
            {
                var getData = await _context.reports_etiqueta_meta_importe
                    .FromSqlRaw("EXEC SP_Reportes_etiqueta_meta_importe")
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesEtiquetasDTO();
                }
                var convertDto = new DetallesReportesEtiquetasDTO(getData);
                return convertDto;
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error al obtener los datos de etiquetas de metas.");
                return new DetallesReportesEtiquetasDTO();
            }
        }

        public async Task<DetallesReportesTablasDTO> GetReportesTablaGeneralFechaMeses(int idUsuario, int mes, int año)
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
                    return new DetallesReportesTablasDTO();
                }
                return new DetallesReportesTablasDTO(getData);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error al obtener los datos de etiquetas de metas: " + ex.Message);
                return new DetallesReportesTablasDTO();
            }
        }
        public async Task<DetallesReportesGpieDTO> GetReportesGpieGeneralFechaMeses(int idUsuario, int mes, int año)
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
                    return new DetallesReportesGpieDTO();
                }
                var convertDto = new DetallesReportesGpieDTO(getData.FirstOrDefault());
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesGpieDTO();
            }
        }
    }
}