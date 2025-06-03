using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryReferidos : IRepositoryReferidos
    {
        private readonly MDbContext _context;
        public RepositoryReferidos(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message)> EnviarCorreoReferido(string dni)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@dni", dni ?? (object)DBNull.Value)
                };
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_referir_enviar_emails_de_referencia @dni",
                    parameters
                );
                if (result > 0)
                {
                    return (false, "No se pudo enviar el correo de referido, verifique el DNI ingresado.");
                }
                return (true, "Correo de referido enviado correctamente");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al referir cliente: {ex.Message}");
                return (false, $"Error al referir cliente: {ex.Message}");
            }
        }

        public async Task<(bool IsSuccess, string Message)> ReferirCliente(Cliente cliente, Vendedor asesor)
        {
            try
            {
                if (cliente == null)
                {
                    return (false, "Cliente no puede ser nulo");
                }
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@dni", cliente.Dni ?? (object)DBNull.Value),
                    new SqlParameter("@dni_asesor", cliente.DniVendedor ?? (object)DBNull.Value),
                    new SqlParameter("@nombre_completo_asesor", cliente.NombresCompletosV ?? (object)DBNull.Value),
                    new SqlParameter("@traido_de", cliente.FuenteBase ?? (object)DBNull.Value),
                    new SqlParameter("@telefono_cliente", cliente.Telefono ?? (object)DBNull.Value),
                    new SqlParameter("@agencia_referido", cliente.AgenciaComercial ?? (object)DBNull.Value),
                    new SqlParameter("@fecha_visita_agencia", cliente.FechaVisita ?? (object)DBNull.Value),
                    new SqlParameter("@celular_asesor", asesor.Telefono ?? (object)DBNull.Value),
                    new SqlParameter("@correo_asesor", asesor.Correo ?? (object)DBNull.Value),
                    new SqlParameter("@cci_asesor", asesor.Cci ?? (object)DBNull.Value),
                    new SqlParameter("@ubigeo_asesor", asesor.Ubigeo ?? (object)DBNull.Value),
                    new SqlParameter("@departamento_asesor", asesor.Departamento ?? (object)DBNull.Value),
                    new SqlParameter("@banco_asesor", asesor.Banco ?? (object)DBNull.Value),
                };
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_referir_cliente_guardar_referencia @dni, @dni_asesor, @nombre_completo_asesor, @traido_de, @telefono_cliente, @agencia_referido, @fecha_visita_agencia, @celular_asesor, @correo_asesor, @cci_asesor, @ubigeo_asesor, @departamento_asesor, @banco_asesor",
                    parameters
                );
                
                if (result <= 0)
                {
                    return (false, "No se pudo referir el cliente, verifique los datos ingresados.");
                }
                return (true, "Cliente referido correctamente");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al referir cliente: {ex.Message}");
                return (false, $"Error al referir cliente: {ex.Message}");
            }
        }
    }
}