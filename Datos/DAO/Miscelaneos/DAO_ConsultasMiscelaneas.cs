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
        public async Task<(bool IsSuccess, string Message, List<string> Listas)> GetListas(int idUsuario)
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
                    return (false, "No se encontraron listas para el usuario especificado.", new List<string>());
                }
                var listas = await _context.listas_asignacion
                    .Where(l => listas_id.Contains(l.IdLista))
                    .Select(l => l.NombreLista ?? string.Empty)
                    .Distinct()
                    .ToListAsync();
                if (listas == null || !listas.Any())
                {
                    return (false, "No se encontraron listas con los IDs especificados.", new List<string>());
                }
                return (true, "Listas obtenidas correctamente.", listas);
            }
            catch (System.Exception)
            {
                return (false, "Ha ocurrido un error al obtener las listas.", new List<string>());
            }
        }
        public async Task<List<string>> getDestinos(int idUsuarioS)
        {
            try
            {
                var destinos = await _context.clientes_asignados
                    .Where(c => c.IdUsuarioS == idUsuarioS)
                    .Select(c => c.Destino ?? string.Empty)
                    .Distinct()
                    .ToListAsync();

                if (destinos == null || !destinos.Any())
                {
                    return new List<string>();
                }
                return destinos;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al obtener agencias: {ex.Message}");
                return new List<string>();
            }
        }
        public async Task<List<string>> getBases(int idUsuarioS)
        {
            try
            {
                var bases = await _context.clientes_asignados
                    .Where(c => c.IdUsuarioS == idUsuarioS)
                    .Select(c => c.FuenteBase ?? string.Empty)
                    .Distinct()
                    .ToListAsync();

                if (bases == null || !bases.Any())
                {
                    return new List<string>();
                }
                return bases;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al obtener listas: {ex.Message}");
                return new List<string>();
            }
        }
        public async Task<(bool IsSuccess, string Message, List<AgenciasDisponiblesDTO> Agencias)> GetAgencias()
        {
            try
            {
                var agenciasDisponibles = await _context.agencias_disponibles_dto
                .FromSqlRaw("EXEC sp_U_agencias_con_numeros")
                .ToListAsync();

                if (agenciasDisponibles == null || !agenciasDisponibles.Any())
                {
                    return (false, "No se encontraron agencias disponibles", new List<AgenciasDisponiblesDTO>());
                }
                return (true, "Agencias encontradas", agenciasDisponibles);
            }
            catch (System.Exception)
            {
                return (false, "Ha ocurrido un error al obtener las agencias.", new List<AgenciasDisponiblesDTO>());
            }
        }
        public async Task<(bool IsSuccess, string Message, List<(int idtip, string nombretip)> Data)> GetTipificaciones()
        {
            try
            {
                var tipificaciones = await _context.tipificaciones.ToListAsync();
                if (tipificaciones == null)
                {
                    return (false, "No se pudo encontrar tipificaciones en la base de datos", new List<(int idtip, string nombretip)>());
                }
                return (true, "Se han encontrado las tipificaciones en la base de datos",
                    tipificaciones.Select(t => (t.IdTipificacion, t.DescripcionTipificacion)).ToList());
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new List<(int idtip, string nombretip)>());
            }
        }
        public async Task<BaseCliente?> getBase(int idBase)
        {
            try
            {
                var basecliente = await _context.base_clientes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdBase == idBase);
                return basecliente;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        
    }
}