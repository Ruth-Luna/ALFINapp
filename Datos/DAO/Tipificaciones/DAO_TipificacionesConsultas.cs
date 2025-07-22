using ALFINapp.Models;
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

        public async Task<(bool IsSuccess, string Message, ViewClienteDetallado? lista)> GetClienteA365(
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
                    .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_para_tipificar_A365_refactorizado @IdBase, @IdUsuarioV",
                        parameters)
                    .ToListAsync();
                if (detalleClienteConsulta == null || !detalleClienteConsulta.Any())
                {
                    return (false, "No se encontró el cliente con el ID proporcionado", null);
                }
                var datosCliente = detalleClienteConsulta.FirstOrDefault();
                if (datosCliente == null)
                {
                    return (false, "No se encontraron datos del cliente", null);
                }
                var viewCliente = new ViewClienteDetallado(datosCliente);
                viewCliente.llenarTelefonosBd(datosCliente);
                viewCliente.llenarTelefonosManual(detalleClienteConsulta);
                return (true, "Clientes asignados obtenidos correctamente", viewCliente);
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