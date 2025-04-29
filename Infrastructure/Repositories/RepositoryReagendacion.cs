using ALFINapp.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryReagendacion : IRepositoryReagendacion
    {
        private readonly MDbContext _context;
        public RepositoryReagendacion(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool success, string message)> checkDisReagendacion(int IdDerivacion, DateTime FechaReagendamiento)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdDerivacion", IdDerivacion),
                    new SqlParameter("@FechaDerivacion", FechaReagendamiento)
                };
                var check = await _context.resultado_verificacion
                    .FromSqlRaw("exec sp_reagendamiento_verificar_disponibilidad_para_reagendamiento_derivacion @IdDerivacion, @FechaDerivacion", parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (check.Count == 0)
                {
                    return (false, "No se ha encontrado la derivación.");
                }
                var result = check.FirstOrDefault();
                if (result == null)
                {
                    return (false, "No se ha encontrado la derivación.");
                }
                if (result.Resultado == 1)
                {
                    return (false, result.Mensaje);
                }
                else
                {
                    return (true, result.Mensaje);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return (false, "Ha ocurrido un error en su red, o en la Base de Datos.");
            }
        }
    }
}