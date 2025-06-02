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
        public async Task<(bool, string)> ReferirCliente(Cliente cliente)
        {
            try
            {
                if (cliente == null)
                {
                    return (false, "Cliente no puede ser nulo");
                }
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Dni", cliente.Dni ?? (object)DBNull.Value),
                    new SqlParameter("@XAppaterno", cliente.XAppaterno ?? (object)DBNull.Value),
                    new SqlParameter("@XApmaterno", cliente.XApmaterno ?? (object)DBNull.Value),
                    new SqlParameter("@XNombre", cliente.XNombre ?? (object)DBNull.Value),
                    new SqlParameter("@Telefono", cliente.Telefono ?? (object)DBNull.Value),
                    new SqlParameter("@Correo", cliente.Correo ?? (object)DBNull.Value),
                    new SqlParameter("@Cci", cliente.Cci ?? (object)DBNull.Value),
                    new SqlParameter("@Ubigeo", cliente.Ubigeo ?? (object)DBNull.Value),
                    new SqlParameter("@Banco", cliente.Banco ?? (object)DBNull.Value),
                    new SqlParameter("@OfertaMax", cliente.OfertaMax ?? (object)DBNull.Value),
                    new SqlParameter("@Campaña", cliente.Campaña ?? (object)DBNull.Value),
                    new SqlParameter("@FechaVisita", cliente.FechaVisita ?? (object)DBNull.Value),
                    new SqlParameter("@FechaSubido", DateTime.Now), // Fecha actual
                    new SqlParameter("@Cuota", cliente.Cuota ?? (object)DBNull.Value),
                    new SqlParameter("@Oferta12m", cliente.Oferta12m ?? (object)DBNull.Value),
                    new SqlParameter("@Tasa12m", cliente.Tasa12m ?? (object)DBNull.Value),
                    new SqlParameter("@Tasa18m", cliente.Tasa18m ?? (object)DBNull.Value),
                    new SqlParameter("@Cuota18m", cliente.Cuota18m ?? (object)DBNull.Value),
                    new SqlParameter("@Oferta24m", cliente.Oferta24m ?? (object)DBNull.Value),
                    new SqlParameter("@Tasa24m", cliente.Tasa24m ?? (object)DBNull.Value),
                    new SqlParameter("@Cuota24m", cliente.Cuota24m ?? (object)DBNull.Value),
                    new SqlParameter("@Oferta36m", cliente.Oferta36m ?? (object)DBNull.Value),
                    new SqlParameter("@Tasa36m", cliente.Tasa36m ?? (object)DBNull.Value),
                    new SqlParameter("@Cuota36m", cliente.Cuota36m ?? (object)DBNull.Value),
                    new SqlParameter("@FuenteBase", cliente.FuenteBase ?? (object)DBNull.Value),
                };

                if (cliente.FuenteBase == "DBA365")
                {
                    var result = await _context
                        .Database
                        .ExecuteSqlRawAsync("EXEC dbo.sp_referir_cliente @Dni, @XAppaterno, @XApmaterno, @XNombre, @Telefono, @Correo, @Cci, @Ubigeo, @Banco, @OfertaMax, @Campaña, @FechaVisita, @FechaSubido, @Cuota, @Oferta12m, @Tasa12m, @Tasa18m, @Cuota18m, @Oferta24m, @Tasa24m, @Cuota24m, @Oferta36m, @Tasa36m, @Cuota36m, @FuenteBase", parameters);
                }
                else if (cliente.FuenteBase == "DBALFIN")
                {

                }
                else
                {
                    return (false, "Fuente de referencia no válida");
                }

                return (true, "Cliente referido correctamente");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al referir cliente: {ex.Message}");
                // Aquí podrías registrar el error en un log o manejarlo de otra manera.
                // Por ahora, simplemente retornamos un mensaje de error.
                return (false, $"Error al referir cliente: {ex.Message}");
            }
        }
    }
}