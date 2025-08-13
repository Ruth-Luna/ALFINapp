using Microsoft.EntityFrameworkCore;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Application.DTOs;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryDerivaciones : IRepositoryDerivaciones
    {
        MDbContext _context;
        public RepositoryDerivaciones(MDbContext context)
        {
            _context = context;
        }

        public async Task<DetallesDerivacionesAsesoresDTO?> getDerivacion(int idDer)
        {
            try
            {
                var derivacion = await _context
                    .derivaciones_asesores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdDerivacion == idDer);
                if (derivacion != null)
                {
                    return new DetallesDerivacionesAsesoresDTO(derivacion);
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}