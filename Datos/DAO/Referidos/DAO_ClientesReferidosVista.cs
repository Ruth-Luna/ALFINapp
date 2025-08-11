using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Referidos
{
    public class DAO_ClientesReferidosVista
    {
        private readonly MDbContext _context;

        public DAO_ClientesReferidosVista(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message, List<ViewReferidos> Data)> GetClientesReferidos()
        {
            try
            {
                var result = await _context.clientes_referidos.FromSqlRaw("EXEC dbo.SP_REFERIDOS_GET_ALL_REFERIDOS")
                    .ToListAsync();

                if (result == null || result.Count == 0)
                {
                    return (false, "No se encontraron clientes referidos.", new List<ViewReferidos>());
                }
                return (true, "Clientes referidos obtenidos correctamente.",
                        result.Select(r => new ViewReferidos(r)).ToList());
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al obtener clientes referidos: {ex.Message}");
                return (false, $"Error al obtener clientes referidos: {ex.Message}", new List<ViewReferidos>());
            }
        }
        public async Task<(bool IsSuccess, string Message, ClientesReferidos Data)> GetClienteReferidoPorId(int IdReferido)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@IdReferido", IdReferido)
                };
                var result = await _context.clientes_referidos
                    .FromSqlRaw("EXEC dbo.SP_REFERIDOS_GET_REFERIDO_BY_ID @IdReferido", parameters)
                    .ToListAsync();
                if (result == null || result.Count == 0)
                {
                    return (false, "No se encontró el referido con el ID proporcionado.", new ClientesReferidos());
                }
                var referido = result.FirstOrDefault();
                if (referido == null)
                {
                    return (false, "No se encontró el referido con el ID proporcionado.", new ClientesReferidos());
                }
                return (true, "Se encontro el siguiente referido", referido);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ClientesReferidos());
            }
        }
    }
}