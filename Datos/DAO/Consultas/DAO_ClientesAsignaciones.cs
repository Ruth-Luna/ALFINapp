using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO
{
    public class DAO_ClientesAsignaciones
    {
        private readonly MDbContext _context;
        public DAO_ClientesAsignaciones(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool success, string message)> AsignarClienteManual(string dniCliente, string baseTipo, int idVendedor)
        {
            try
            {
                var param = new SqlParameter[]
                {
                    new SqlParameter("@DNIBusqueda", dniCliente),
                    new SqlParameter("@IdUsuarioV", idVendedor),
                    new SqlParameter("@BaseTipo", baseTipo)
                };
                var result = await _context
                    .Database
                    .ExecuteSqlRawAsync("EXEC sp_Asignacion_cliente_manual @DNIBusqueda, @IdUsuarioV, @BaseTipo", param);
                if (result < 0)
                {
                    return (true, "Asignación manual exitosa");
                }
                else
                {
                    return (false, "No se pudo guardar la asignación manual del cliente.");
                }
            }
            catch (System.Exception ex)
            {
                return (false, $"Error en la asignación manual de cliente: {ex.Message}");
            }
        }

    }
}