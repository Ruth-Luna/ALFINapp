using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
namespace ALFINapp.Infrastructure.Services
{
    /// <summary>
    /// Service class that provides database operations for administrator-level queries in the ALFINapp system.
    /// Handles database filtering, supervisor management, and user retrieval operations.
    /// </summary>
    public class DBServicesConsultasAdministrador
    {
        private readonly MDbContext _context;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DBServicesConsultasAdministrador"/> class.
        /// </summary>
        /// <param name="context">The database context used for database operations.</param>
        public DBServicesConsultasAdministrador(MDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves all users with supervisor role (role ID 2) from the system.
        /// </summary>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the operation was successful
        /// - Message: Descriptive message about the result
        /// - Data: List of Usuario objects representing supervisors if successful, otherwise null
        /// </returns>
        /// <remarks>
        /// Uses a LINQ query to filter users where IdRol equals 2 (supervisor role).
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string Message, List<Usuario>? Data)> ConseguirTodosLosSupervisores()
        {
            try
            {
                var TodosLosSupervisores = await (from u in _context.usuarios
                                                  where u.IdRol == 2
                                                  select u
                                ).ToListAsync();

                if (TodosLosSupervisores == null)
                {
                    return (false, "No se han encontrado supervisores este error fue inesperado", null);
                }

                return (true, "Se han encontrado los siguientes supervisores", TodosLosSupervisores);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}