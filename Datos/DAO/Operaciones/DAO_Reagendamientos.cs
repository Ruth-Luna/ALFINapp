using ALFINapp.Models;
using Microsoft.Build.Framework;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ALFINapp.Datos.DAO.Operaciones
{
    public class DAO_Reagendamientos
    {
        private readonly MDbContext _context;
        public DAO_Reagendamientos(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool issuccess, string message, List<ViewReagendamientos> data)> GetAllReagendamientos(int usuarioId, int rolUsuario)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@id_usuario", usuarioId),
                    new SqlParameter("@month", DateTime.Now.Month),
                    new SqlParameter("@year", DateTime.Now.Year)
                };
                var data = await _context.reagendamientos_get_reagendamientos
                    .FromSqlRaw("EXECUTE dbo.SP_REAGENDAMIENTOS_GET_REAGENDAMIENTOS_VIEW @id_usuario = {0}, @month = {1}, @year = {2}"
                        , parameters)
                    .ToListAsync();

                if (data == null || !data.Any())
                {
                    return (true, "No se encontraron reagendamientos.", new List<ViewReagendamientos>());
                }
                var dataview = data.Select(x => new ViewReagendamientos(x)).ToList();
                return (true, "Reagendamientos obtenidos con éxito.", dataview);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener los reagendamientos: {ex.Message}", new List<ViewReagendamientos>());
            }
        }
    }
}