using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryTelefonos : IRepositoryTelefonos
    {
        MDbContext _context;
        public RepositoryTelefonos(MDbContext context)
        {
            _context = context;
        }
        public async Task<TelefonosAgregados?> GetTelefono(string telefono, int IdCliente)
        {
            try
            {
                var telfono = await _context
                    .telefonos_agregados
                    .FirstOrDefaultAsync(x => x.Telefono == telefono && x.IdCliente == IdCliente);
                return telfono;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> UpdateTelefono(TelefonosAgregados telefono)
        {
            try
            {
                var telfono = await _context
                    .telefonos_agregados
                    .FirstOrDefaultAsync(x => x.IdTelefono == telefono.IdTelefono);
                if (telfono != null)
                {
                    telfono.Telefono = telefono.Telefono;
                    telfono.UltimaTipificacion = telefono.UltimaTipificacion;
                    telfono.FechaUltimaTipificacion = telefono.FechaUltimaTipificacion;
                    telfono.IdClienteTip = telefono.IdClienteTip;
                    
                    await _context.SaveChangesAsync();
                    return true;
                }
                Console.WriteLine("No se encontró el teléfono");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}