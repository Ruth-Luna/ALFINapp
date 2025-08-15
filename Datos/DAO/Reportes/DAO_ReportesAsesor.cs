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
                var detallesUsuarioDTO = new DetallesUsuarioDTO(usuario);
                viewReportes.asesor = detallesUsuarioDTO.ToView();
                viewReportes.totalDerivaciones = reportes.DerivacionesDelAsesor
                    .Count();
                viewReportes.totalDesembolsos = reportes.Desembolsos.Count();
                viewReportes.totalAsignado = reportes.ClientesAsignados.Count();
                viewReportes.totalGestionado = reportes.gESTIONDETALLEs.Count();
                viewReportes.totalSinGestionar = reportes.ClientesAsignados.Count - reportes.gESTIONDETALLEs
                    .Where(x => x.IdAsignacion != null)
                    .Count();
                var createDerivacionesFecha = reportes
                    .gESTIONDETALLEs
                    .Where(x => x.CodTip == 2)
                    .GroupBy(x => x.FechaGestion.ToString("%d/%M/%y"))
                    .Select(x => new DerivacionesFecha { Fecha = x.Key, Contador = x.Count() })
                    .OrderBy(x => DateTime.TryParseExact(x.Fecha, "d/M/yy", null, System.Globalization.DateTimeStyles.None, out var parsedDate) ? parsedDate : DateTime.MinValue)
                    .ToList();
                viewReportes.derivacionesFecha = createDerivacionesFecha;
                var createDesembolsosFecha = reportes
                    .Desembolsos
                    .GroupBy(x => x.FechaDesembolsos != null ? x.FechaDesembolsos.Value.ToString("%d/%M/%y") : "")
                    .Select(x => new DerivacionesFecha { Fecha = x.Key, Contador = x.Count() })
                    .OrderBy(x => DateTime.TryParseExact(x.Fecha, "d/M/yy", null, System.Globalization.DateTimeStyles.None, out var parsedDate) ? parsedDate : DateTime.MinValue)
                    .ToList();
                viewReportes.desembolsosFecha = createDesembolsosFecha;
                viewReportes.gestionDetalles = reportes.gESTIONDETALLEs.Select(x => new DetallesGestionDetalleDTO(x).toView()).ToList();
                var TipificacionesGestion = new List<ViewTipificacionesGestion>();
                var TipificacionesDescripcion = await _dao_ConsultasMiscelaneas.GetTipificaciones();
                var dicTipificaciones = TipificacionesDescripcion.Data
                    .ToDictionary(y => y.idtip, y => y.nombretip);
                var agruparTipificaciones = viewReportes
                    .gestionDetalles
                    .GroupBy(x => x.CodTip)
                    .Select(g => new ViewTipificacionesGestion
                    {
                        IdTipificacion = g.Key,
                        DescripcionTipificaciones = dicTipificaciones.TryGetValue(g.Key, out var descripcion) ? descripcion : "",
                        ContadorTipificaciones = g.Count()
                    })
                    .ToList();
                viewReportes.tipificacionesGestion = agruparTipificaciones;
                return (true, "Reportes obtenidos correctamente", viewReportes);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<DetallesReportesAsesorDTO> GetReportesAsesor(
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
                    return new DetallesReportesAsesorDTO();
                }
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@DniAsesor", getUsuario.Dni),
                    new SqlParameter("@mes", mes ?? (object)DBNull.Value),
                    new SqlParameter("@anio", anio ?? (object)DBNull.Value)
                };

                var getAllAsignaciones = await _context.clientes_asignados
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Asesor_asignaciones @DniAsesor, @mes, @anio",
                        parameters)
                    .ToListAsync();

                var getAllIds = getAllAsignaciones.Select(x => x.IdAsignacion).ToHashSet();
                var getAllDerivaciones = await _context.derivaciones_asesores
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Asesor_derivacion @DniAsesor, @mes, @anio",
                        parameters)
                    .AsNoTracking()
                    .ToListAsync();

                var getAllGestionDetalle = await _context.GESTION_DETALLE
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Asesor_gestion @DniAsesor, @mes, @anio",
                        parameters)
                    .AsNoTracking()
                    .ToListAsync();

                getAllGestionDetalle = getAllGestionDetalle.GroupBy(g => g.DocCliente)
                    .Select(g => g.OrderByDescending(d => d.CodTip == 2)
                        .ThenByDescending(d => d.FechaGestion)
                        .First())
                    .ToList();

                var getAllDesembolsos = await _context.desembolsos
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Asesor_desembolsos @DniAsesor, @mes, @anio",
                        parameters)
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
    }
}