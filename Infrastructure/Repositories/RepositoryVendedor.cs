using ALFINapp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryVendedor : IRepositoryVendedor
    {
        private readonly MDbContext _context;
        public RepositoryVendedor(MDbContext context)
        {
            _context = context;
        }
        public async Task<Persistence.Models.Usuario?> GetVendedor(int IdUsuario)
        {
            try
            {
                var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.IdUsuario == IdUsuario);
                return usuario;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}