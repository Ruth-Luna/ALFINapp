using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Services
{
    /// <summary>
    /// Service class that provides database operations for administrator-level assignment management in the ALFINapp system.
    /// Handles client assignment to supervisors and related administrative functions.
    /// </summary>
    public class DBServicesAsignacionesAdministrador
    {
        private readonly MDbContext _context;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DBServicesAsignacionesAdministrador"/> class.
        /// </summary>
        /// <param name="context">The database context used for database operations.</param>
        public DBServicesAsignacionesAdministrador(MDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Assigns a client to a specific supervisor in the system.
        /// </summary>
        /// <param name="idCliente">ID of the client to assign.</param>
        /// <param name="idSupervisor">ID of the supervisor to assign the client to.</param>
        /// <param name="fuenteBase">Source database identifier for the client data.</param>
        /// <param name="destino">Destination or purpose of the assignment.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the assignment operation was successful
        /// - Message: Descriptive message about the result
        /// </returns>
        /// <remarks>
        /// Uses the stored procedure SP_INSERTAR_ASIGNACION to create the assignment record.
        /// All parameters are optional - null values are passed as DBNull to the stored procedure.
        /// Returns success only if the stored procedure affects at least one row.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string Message)> InsertarAsignacionASupervisores(
            int? idCliente, int? idSupervisor, string? fuenteBase, string? destino)
        {
            try
            {
                var insertarAsignacion = await _context.Database
                    .ExecuteSqlRawAsync("EXEC SP_INSERTAR_ASIGNACION @id_cliente, @id_usuarioS, @fuente_base, @destino",
                        new SqlParameter("@id_cliente", idCliente ?? (object)DBNull.Value),
                        new SqlParameter("@id_usuarioS", idSupervisor ?? (object)DBNull.Value),
                        new SqlParameter("@fuente_base", fuenteBase ?? (object)DBNull.Value),
                        new SqlParameter("@destino", destino ?? (object)DBNull.Value));
                if (insertarAsignacion <= 0)
                {
                    return (false, "La Asignacion al Filtrar Bases no se ha insertado");
                }
                return (true, "La Asignacion al Filtrar Bases se ha ejecutado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}