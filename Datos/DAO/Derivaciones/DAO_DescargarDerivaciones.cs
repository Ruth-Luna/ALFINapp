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
            DateTime? fecha_final = null,
            int? id_usuario = null)
        {
            try
            {
                if (filtro != null && campo == null)
                {
                    return (false, "Si selecciona un filtro, debe llenar un campo.", new ViewDescargas());
                }
                if (campo != null && filtro == null)
                {
                    return (false, "Si selecciona un campo, debe llenar un filtro.", new ViewDescargas());
                }
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = null; // Si no hay filtro, se asigna null
                }
                if (string.IsNullOrEmpty(campo))
                {
                    campo = null; // Si no hay campo, se asigna null
                }
                if (fecha_inicio.HasValue && fecha_final.HasValue && fecha_inicio > fecha_final)
                {
                    return (false, "La fecha de inicio no puede ser mayor que la fecha final.", new ViewDescargas());
                }
                if (id_usuario.HasValue && id_usuario <= 0)
                {
                    return (false, "El ID de usuario debe ser un número positivo.", new ViewDescargas());
                }
                if (!fecha_inicio.HasValue)
                {
                    fecha_inicio = null; // Si no hay fecha de inicio, se asigna null
                }
                if (!fecha_final.HasValue)
                {
                    fecha_final = null; // Si no hay fecha final, se asigna null
                }
                var parameters = new[]
                {
                    new SqlParameter("@filtro", filtro ?? (object)DBNull.Value),
                    new SqlParameter("@campo", campo ?? (object)DBNull.Value),
                    new SqlParameter("@fecha_inicio", fecha_inicio.HasValue ? fecha_inicio.Value : (object)DBNull.Value),
                    new SqlParameter("@fecha_final", fecha_final.HasValue ? fecha_final.Value : (object)DBNull.Value),
                    new SqlParameter("@id_usuario", id_usuario.HasValue ? id_usuario.Value : (object)DBNull.Value)
                };
                
                var derivaciones = await _context.derivacion_conseguir_o_descargar_asignacion_con_derivaciones_de_sup
                    .FromSqlRaw("EXEC SP_derivacion_conseguir_o_descargar_asignacion_con_derivaciones_de_sup @filtro, @campo, @fecha_inicio, @fecha_final, @id_usuario",
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