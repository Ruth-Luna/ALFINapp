using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Supervisores
{
    public class DAO_SupervisorConsultas
    {
        private readonly MDbContext _context;
        public DAO_SupervisorConsultas(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, string Message, List<ClientesAsignado>? Data)> ConsultaLeadsDelSupervisorXDestino(int idSupervisorActual, string destino)
        {
            try
            {
                var parameters = new[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@IdSupervisorActual", idSupervisorActual),
                    new Microsoft.Data.SqlClient.SqlParameter("@Destino", destino ?? (object)DBNull.Value)
                };
                var data = await _context.clientes_asignados.FromSqlRaw("EXEC SP_SUPERVISORES_CONSULTAR_VENTAS_POR_DESTINO @IdSupervisorActual = {0}, @Destino = {1}", parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (data == null || !data.Any())
                {
                    return (false, "No se encontraron clientes asignados al supervisor actual", null);
                }
                return (true, "La Consulta se produjo con exito", data);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ocurrió un error al obtener los datos: {ex.Message}", null);
            }
        }
        public async Task<(bool IsSuccess, string Message, List<ClientesAsignado> Data)> ConsultarAsignaciones(
            int idSupervisor,
            string filter,
            string type_filter)
        {
            try
            {
                var valid_filters = new List<string>
                {
                    "lista",
                    "destino",
                    "fecha",
                    "base"
                };
                if (!valid_filters.Contains(type_filter))
                {
                    return (false, $"El tipo de filtro no es válido. Debe ser uno de los siguientes: {string.Join(", ", valid_filters)}", new List<ClientesAsignado>());
                }
                if (string.IsNullOrEmpty(filter))
                {
                    return (false, "Debe seleccionar un Destino o Lista de la Base.", new List<ClientesAsignado>());
                }
                var filter_lista = 0;
                if (type_filter == "lista")
                {
                    var lista = await _context.listas_asignacion
                        .Where(l => l.NombreLista == filter)
                        .Select(l => l.IdLista)
                        .FirstOrDefaultAsync();
                    if (lista == 0)
                    {
                        return (false, "No se encontró la lista especificada.", new List<ClientesAsignado>());
                    }
                    filter_lista = lista;
                }
                var consulta = await _context.clientes_asignados.Where(
                    c => c.IdUsuarioS == idSupervisor &&
                    (type_filter == "lista" && c.IdLista == filter_lista ||
                     type_filter == "destino" && c.Destino == filter ||
                     type_filter == "fecha" && c.FechaAsignacionSup.Value.ToString("yyyy-MM-dd") == filter ||
                     type_filter == "base" && c.NomBase == filter))
                    .ToListAsync();

                return (true, "Consulta realizada con éxito", consulta);
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}