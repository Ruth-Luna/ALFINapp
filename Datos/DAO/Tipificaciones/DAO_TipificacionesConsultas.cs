using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Tipificaciones
{
    public class DAO_TipificacionesConsultas
    {
        private readonly MDbContext _context;
        public DAO_TipificacionesConsultas(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message, object? lista)> GetClienteA365(
            int id_cliente,
            int id_usuario_v)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdBase", id_cliente),
                    new SqlParameter("@IdUsuarioV", id_usuario_v)
                };
                var detalleClienteConsulta = await _context.detalle_cliente_a365_tipificar_dto
                    .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_para_tipificar_A365 @IdBase, @IdUsuarioV",
                        parameters)
                    .ToListAsync();

                return (true, "Clientes asignados obtenidos correctamente", detalleClienteConsulta );
            }
            catch (System.Exception)
            {
                return (false, "Error al obtener los clientes asignados", null);
            }
        }
        public async Task<(bool IsSuccess, string Message, object? lista)> GetClienteAlfin(
            int id_cliente,
            int id_usuario_v)
        {
            try
            {
                var detalleClienteConsulta = await _context.detalle_cliente_a365_tipificar_dto
                    .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_para_tipificar_ALFIN @IdBase, @IdUsuarioV",
                        new SqlParameter("@IdBase", id_cliente),
                        new SqlParameter("@IdUsuarioV", id_usuario_v))
                    .ToListAsync();
                return (true, "Clientes asignados obtenidos correctamente", "en proceso");
            }
            catch (System.Exception)
            {
                return (false, "Error al obtener los clientes asignados", null);
            }
        }
    }
}