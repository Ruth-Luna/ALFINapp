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
        public async Task<DetallesReportesDerivacionesDTO?> GetReportesGralSupervisor(int idSupervisor)
        {
            try
            {
                var getAsesores = await _context
                    .usuarios
                    .Where(x => x.IDUSUARIOSUP == idSupervisor)
                    .ToListAsync();
                var dniAsesores = getAsesores.Select(x => x.Dni).ToHashSet();
                var getDerivacionesGral = await _context.derivaciones_asesores
                    .Where(x => dniAsesores.Contains(x.DniAsesor)
                        && x.FechaDerivacion.Year == DateTime.Now.Year
                        && x.FechaDerivacion.Month == DateTime.Now.Month)
                    .ToListAsync();
                var getGestionDetalles = await _context.GESTION_DETALLE
                    .Where(x => dniAsesores.Contains(x.DocAsesor)
                        && x.FechaGestion.Year == DateTime.Now.Year
                        && x.FechaGestion.Month == DateTime.Now.Month)
                    .GroupBy(x => x.DocCliente)
                    .Select(g => g.OrderByDescending(x => x.IdFeedback).First())
                    .ToListAsync();
                var getDesembolsos = await _context.desembolsos
                    .Where(x => dniAsesores.Contains(x.DocAsesor)
                        && x.FechaDesembolsos.HasValue
                        && x.FechaDesembolsos.Value.Year == DateTime.Now.Year
                        && x.FechaDesembolsos.Value.Month == DateTime.Now.Month
                        && x.Sucursal != null)
                    .ToListAsync();
                var detallesReporte = new DetallesReportesDerivacionesDTO();
                detallesReporte.DerivacionesGral = getDerivacionesGral;
                detallesReporte.GestionDetalles = getGestionDetalles;
                detallesReporte.Desembolsos = getDesembolsos;
                return detallesReporte;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
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

        public async Task<DetallesReportesDerivacionesDTO> GetReportesGralAsesor(int idAsesor)
        {
            try
            {
                var getAsesor = await _context.usuarios
                    .Where(x => x.IdUsuario == idAsesor)
                    .FirstOrDefaultAsync();
                if (getAsesor == null)
                {
                    Console.WriteLine("Asesor no encontrado");
                    return new DetallesReportesDerivacionesDTO();
                }
                var getDerivacionesGral = await _context.derivaciones_asesores
                    .Where(x => x.DniAsesor == getAsesor.Dni
                        && x.FechaDerivacion.Year == DateTime.Now.Year
                        && x.FechaDerivacion.Month == DateTime.Now.Month)
                    .ToListAsync();
                var getGestionDetalles = await _context.GESTION_DETALLE
                    .Where(x => x.DocAsesor == getAsesor.Dni
                        && x.FechaGestion.Year == DateTime.Now.Year
                        && x.FechaGestion.Month == DateTime.Now.Month)
                    .GroupBy(x => x.DocCliente)
                    .Select(g => g.OrderByDescending(x => x.IdFeedback).First())
                    .ToListAsync();
                var getDesembolsos = await _context.desembolsos
                    .Where(x => x.DocAsesor == getAsesor.Dni
                        && x.FechaDesembolsos.HasValue
                        && x.FechaDesembolsos.Value.Year == DateTime.Now.Year
                        && x.FechaDesembolsos.Value.Month == DateTime.Now.Month
                        && x.Sucursal != null)
                    .ToListAsync();
                var detallesReporte = new DetallesReportesDerivacionesDTO();
                detallesReporte.DerivacionesGral = getDerivacionesGral;
                detallesReporte.GestionDetalles = getGestionDetalles;
                detallesReporte.Desembolsos = getDesembolsos;
                return detallesReporte;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesDerivacionesDTO();
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

        public async Task<DetallesReportesGpieDTO> GetReportesGpieGestionadosVsAsignados()
        {
            try
            {
                var getData = await _context.reports_g_pie_gestion_asignados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_PORCENTAJE_GESTIONADOS_SOBRE_ASIGNADOS")
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesGpieDTO();
                }
                var convertDto = new DetallesReportesGpieDTO(getData.FirstOrDefault(), null);
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesGpieDTO();
            }
        }

        public async Task<DetallesReportesGpieDTO> GetReportesGpieDerivadosVsDesembolsados()
        {
            try
            {
                var getData = await _context.reports_g_pie_derivados_desembolsados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_PORCENTAJE_GESTIONADO_DERIVADO_DESEMBOLSADO")
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesGpieDTO();
                }
                var convertDto = new DetallesReportesGpieDTO(null, getData.FirstOrDefault());
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesGpieDTO();
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
                    new SqlParameter("@Fecha", fecha),
                    new SqlParameter("@IdUsuario", idUsuario)
                };
                var getDataDer = await _context.reports_g_pie_derivados_desembolsados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_POR_FECHAS_GESTION_DERIVACION_DESEMBOLSO @Fecha @IdUsuario",
                        parameters)
                    .ToListAsync();
                var getDataGes = await _context.reports_g_pie_gestion_asignados
                    .FromSqlRaw("EXEC SP_REPORTES_GPIE_POR_FECHAS_GESTIONADOS_SOBRE_ASIGNADOS @Fecha @IdUsuario",
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

        public async Task<DetallesReportesBarDTO> GetReportesBarTop5General(int idUsuario)
        {
            try
            {
                var getData = await _context.reports_bar_top_5_derivaciones
                    .FromSqlRaw("EXEC SP_REPORTES_BAR_TOP_5_DERIVACIONES @id_usuario", new SqlParameter("@id_usuario", idUsuario))
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesBarDTO();
                }
                var convertDto = new DetallesReportesBarDTO(getData);
                return convertDto;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesBarDTO();
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

        public async Task<DetallesReportesTablasDTO> GetReportesTablaGestionDerivadoDesembolsoImporte()
        {
            try
            {
                var getData = await _context.reports_tabla_gestionado_derivado_desembolsado_importe
                    .FromSqlRaw("EXEC SP_REPORTES_TABLA_GESTIONADO_DERIVADO_DESEMBOLSADO_IMPORTE")
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesTablasDTO();
                }
                var convertDto = new DetallesReportesTablasDTO(getData);
                return convertDto;
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error al obtener los datos de la tabla de gestión, derivación y desembolso por importe.");
                return new DetallesReportesTablasDTO();
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

        public async Task<DetallesReportesTablasDTO> GetReportesMetas(DateOnly fecha, int idUsuario)
        {
            try
            {
                var getData = await _context.reports_bar_top_5_derivaciones
                    .FromSqlRaw("EXEC SP_REPORTES_TABLA_METAS @fecha, @id_usuario",
                        new SqlParameter("@fecha", fecha),
                        new SqlParameter("@id_usuario", idUsuario))
                    .AsNoTracking()
                    .ToListAsync();
                if (getData == null || getData.Count == 0)
                {
                    Console.WriteLine("No se encontraron datos para la consulta.");
                    return new DetallesReportesTablasDTO();
                }
                return new DetallesReportesTablasDTO();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesTablasDTO();
            }
        }
    }
}