using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Derivaciones
{
    public class DAO_DescargarDerivaciones
    {
        private readonly MDbContext _context;
        public DAO_DescargarDerivaciones(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, string Message, ViewDescargas Data)> GetDerivacionesForDownload(
            string? filtro = null,
            string? campo = null,
            DateTime? fecha_inicio = null,
            DateTime? fecha_final = null)
        {
            try
            {
                var parameters = new []
                {
                    new SqlParameter("@filtro", filtro ?? (object)DBNull.Value),
                    new SqlParameter("@campo", campo ?? (object)DBNull.Value),
                    new SqlParameter("@fecha_inicio", fecha_inicio.HasValue ? fecha_inicio.Value : (object)DBNull.Value),
                    new SqlParameter("@fecha_final", fecha_final.HasValue ? fecha_final.Value : (object)DBNull.Value)
                };
                
                var derivaciones = await _context.derivacion_conseguir_o_descargar_asignacion_con_derivaciones_de_sup
                    .FromSqlRaw("EXEC SP_derivacion_conseguir_o_descargar_asignacion_con_derivaciones_de_sup @Id",
                        parameters)
                    .ToListAsync();

                if (derivaciones == null || !derivaciones.Any())
                {
                    return (false, "No se encontraron derivaciones para el ID proporcionado.", new ViewDescargas());
                }

                var downloadData = new ViewDescargas(derivaciones);
                return (true, "Derivaciones obtenidas correctamente.", downloadData);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener las derivaciones: {ex.Message}", new ViewDescargas());
            }
        }
    }
}