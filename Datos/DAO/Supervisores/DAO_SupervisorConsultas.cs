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
    }
}