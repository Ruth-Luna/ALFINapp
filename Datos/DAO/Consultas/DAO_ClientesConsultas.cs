using ALFINapp.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO
{
    public class DAO_ClientesConsultas
    {
        private readonly MDbContext _context;
        public DAO_ClientesConsultas(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message, ViewClienteDetalles? Data)> GetClienteByDniAsync(string dni)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dni) || dni.Length < 8)
                {
                    return (false, "El DNI no puede estar vacío o nulo.", null);
                }

                var hayentradaA365 = await _context.consulta_obtener_cliente
                .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_por_DNI_refactorizado_A365 @DNI",
                    new SqlParameter("@DNI", dni))
                .ToListAsync();

                var entradaA365 = (entradaDB: false, mensaje: string.Empty);

                if (hayentradaA365 != null)
                {
                    var entrada = hayentradaA365.FirstOrDefault();
                    if (entrada == null)
                    {
                        return (false, "El cliente no tiene Detalles en la Base de Datos de A365. Se buscara en la base de datos interna del banco.", null);
                    }
                    var clienteExistenteA365 = new ViewClienteDetalles(entrada);
                    return (true, "El DNI se encuentra registrado en la Base de Datos de A365 durante este mes. Al cliente se le permite ser tipificado", clienteExistenteA365);
                }
                else
                {
                    entradaA365.entradaDB = false;
                    entradaA365.mensaje = "El cliente no tiene Detalles en la Base de Datos de A365. Se buscara en la base de datos interna del banco. ";
                }
                
                var clienteExistenteAlfin = await _context.consulta_obtener_cliente
                    .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_por_DNI_refactorizado_ALFIN @DNI",
                        new SqlParameter("@DNI", dni))
                    .ToListAsync();
                if (clienteExistenteAlfin == null)
                {
                    return (false, entradaA365.mensaje + $"El cliente no tiene Detalles en la Base de Datos del Banco Alfin. Este DNI no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                }
                var entradaAlfin = clienteExistenteAlfin.FirstOrDefault();
                if (entradaAlfin == null)
                {
                    return (false, entradaA365.mensaje + $"El cliente no tiene Detalles en la Base de Datos del Banco Alfin. Este DNI no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                }
                var clienteExistenteDto = new ViewClienteDetalles(entradaAlfin);
                return (true, entradaA365.mensaje + "El DNI se encuentra registrado en la Base de Datos de Alfin durante este mes. Al cliente se le permite ser tipificado", clienteExistenteDto);
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al obtener el cliente: {ex.Message}", null);
            }
        }
    }
}