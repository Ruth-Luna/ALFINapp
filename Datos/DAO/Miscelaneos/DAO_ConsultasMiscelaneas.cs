using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Miscelaneos
{
    public class DAO_ConsultasMiscelaneas
    {
        private readonly MDbContext _context;
        public DAO_ConsultasMiscelaneas(MDbContext context)
        {
            _context = context;
        }
        public async Task<Usuario?> getuser(int idUsuario)
        {
            try
            {
                var usuario = await _context
                    .usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
                if (usuario != null)
                {
                    return usuario;
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
        public async Task<(bool IsSuccess, string Message, List<string?> Destinos)> GetDestinos(int idUsuario)
        {
            try
            {
                var destinos = await _context.clientes_asignados
                    .Where(c =>
                        c.IdUsuarioS == idUsuario
                        && c.Destino != null
                        && c.FechaAsignacionSup.HasValue
                        && c.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                        && c.FechaAsignacionSup.Value.Month == DateTime.Now.Month)
                    .AsNoTracking()
                    .Select(c => c.Destino)
                    .Distinct()
                    .ToListAsync();

                if (destinos == null || !destinos.Any())
                {
                    return (false, "No se encontraron destinos para el usuario especificado.", new List<string?>());
                }
                return (true, "Destinos obtenidos correctamente.", destinos);
            }
            catch (System.Exception)
            {
                return (false, "Ha ocurrido un error al obtener los destinos.", new List<string?>());
            }
        }
        public async Task<(bool IsSuccess, string Message, List<string?> Listas)> GetListas(int idUsuario)
        {
            try
            {
                var listas_id = await _context.clientes_asignados
                    .Where(c =>
                        c.IdUsuarioS == idUsuario
                        && c.IdLista != null
                        && c.FechaAsignacionSup.HasValue
                        && c.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                        && c.FechaAsignacionSup.Value.Month == DateTime.Now.Month)
                    .AsNoTracking()
                    .Select(c => c.IdLista)
                    .Distinct()
                    .ToListAsync();
                if (listas_id == null || !listas_id.Any())
                {
                    return (false, "No se encontraron listas para el usuario especificado.", new List<string?>());
                }
                var listas = await _context.listas_asignacion
                    .Where(l => listas_id.Contains(l.IdLista))
                    .Select(l => l.NombreLista)
                    .Distinct()
                    .ToListAsync();
                if (listas == null || !listas.Any())
                {
                    return (false, "No se encontraron listas con los IDs especificados.", new List<string?>());
                }
                return (true, "Listas obtenidas correctamente.", listas);
            }
            catch (System.Exception)
            {
                return (false, "Ha ocurrido un error al obtener las listas.", new List<string?>());
            }
        }
    }
}