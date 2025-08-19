using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Usuarios
{
    public class DAO_Usuarios
    {
        private readonly MDbContext _context;
        public DAO_Usuarios(MDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> ListarAsesores(int? idUsuario = null)
        {
            try
            {
                if (idUsuario.HasValue)
                {
                    return await _context.usuarios
                        .Where(
                            u => u.IdRol == 3
                            && u.IDUSUARIOSUP == idUsuario.Value
                            && u.Estado == "ACTIVO")
                        .ToListAsync();
                }
                return await _context.usuarios
                    .Where(
                        u => u.IdRol == 3
                        && u.Estado == "ACTIVO")
                    .ToListAsync();
            }
            catch (System.Exception ex)
            {
                throw new Exception("Error al listar asesores", ex);
            }
        }
        public async Task<List<Usuario>> ListarSupervisores()
        {
            try
            {
                return await _context.usuarios
                    .Where(u => u.IdRol == 2 && u.Estado == "ACTIVO")
                    .ToListAsync();
            }
            catch (System.Exception ex)
            {
                throw new Exception("Error al listar supervisores", ex);
            }
        }
    }
}