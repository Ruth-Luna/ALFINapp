using Microsoft.EntityFrameworkCore;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Infrastructure.Services
{
    /// <summary>
    /// Service class that provides general database operations for the ALFINapp system.
    /// Handles retrieving various lookup data, user information, and common database operations.
    /// </summary>
    public class DBServicesGeneral
    {
        private readonly MDbContext _context;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DBServicesGeneral"/> class.
        /// </summary>
        /// <param name="context">The database context used for database operations.</param>
        public DBServicesGeneral(MDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves detailed information for a specific user by their ID.
        /// </summary>
        /// <param name="IdUsuario">ID of the user to retrieve information for.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the operation was successful
        /// - Message: Descriptive message about the result
        /// - Data: User information object if found, otherwise null
        /// </returns>
        public async Task<(bool IsSuccess, string Message, Usuario? Data)> GetUserInformation(int IdUsuario)
        {
            try
            {
                var FoundUser = await _context.usuarios
                                    .Where(u => u.IdUsuario == IdUsuario)
                                    .FirstOrDefaultAsync();

                if (FoundUser == null)
                {
                    return (false, "El usuario a buscar no se encuentra registrado", null);
                }

                return (true, "Usuario encontrado correctamente", FoundUser);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        /// <summary>
        /// Updates a user's password in the database.
        /// </summary>
        /// <param name="IdUsuario">ID of the user whose password will be updated.</param>
        /// <param name="password">The new password to set.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the password update was successful
        /// - Message: Descriptive message about the result
        /// </returns>
        public async Task<(bool IsSuccess, string Message)> UpdatePasswordGeneralFunction(int IdUsuario, string password)
        {
            try
            {
                var user = await _context.usuarios.Where(u => u.IdUsuario == IdUsuario)
                                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return (false, "El usuario a modificar no se encuentra registrado");
                }

                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE usuarios SET contraseña = {0} WHERE id_usuario = {1}",
                    password, IdUsuario);

                return (true, "La Modificación se realizo con exito");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}