using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryMiscellaneous : IRepositoryMiscellaneous
    {
        private readonly MDbContext _context;
        public RepositoryMiscellaneous(MDbContext context)
        {
            _context = context;
        }
        public async Task<List<Agencia>> GetUAgenciasConNumeros()
        {
            try
            {
                var agenciasDisponibles = await _context.agencias_disponibles_dto
                .FromSqlRaw("EXEC sp_U_agencias_con_numeros")
                .ToListAsync();

                if (agenciasDisponibles == null || !agenciasDisponibles.Any())
                {
                    return (new List<Agencia>());
                }

                var agencias = agenciasDisponibles.Select(a => new Agencia
                {
                    sucursal_comercial = a.sucursal_comercial,
                    agencia_comercial = a.agencia_comercial
                }).ToList();
                return (agencias);
            }
            catch (System.Exception ex)
            {
                // Log the exception (ex) as needed
                // For now, we return an empty list
                Console.WriteLine($"Error fetching agencies: {ex.Message}");
                return (new List<Agencia>());
            }
        }
    }
}