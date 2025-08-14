using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Derivaciones
{
    public class DAO_Derivaciones
    {
        private readonly MDbContext _context;
        public DAO_Derivaciones(MDbContext context)
        {
            _context = context;
        }
        public async Task<DerivacionesAsesores?> GetDerivacionAsync(int idDer)
        {
            try
            {
                var derivacion = await _context
                    .derivaciones_asesores
                    .FromSqlRaw("EXEC SP_DERIVACION_GET_DERIVACION @id_derivacion", new SqlParameter("@id_derivacion", idDer))
                    .ToListAsync();

                if (derivacion != null && derivacion.Count > 0)
                {
                    return derivacion.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}