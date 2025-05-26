using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Services
{
    /// <summary>
    /// Service class that provides database operations for managing advisor status in the ALFINapp system.
    /// Handles activation and deactivation of advisor accounts.
    /// </summary>
    public class DBServicesEstadoAsesores
    {
        private readonly MDbContext _context;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DBServicesEstadoAsesores"/> class.
        /// </summary>
        /// <param name="context">The database context used for database operations.</param>
        public DBServicesEstadoAsesores(MDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Activates an advisor in the system.
        /// </summary>
        /// <param name="asesor">The advisor user to activate.</param>
        /// <param name="idAccion">ID of the user performing the activation action.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the activation was successful
        /// - message: Descriptive message about the result
        /// </returns>
        /// <remarks>
        /// This method calls the stored procedure sp_usuario_activar to change the advisor's status.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string message)> ActivarAsesor(Usuario asesor, int idAccion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", asesor.IdUsuario),
                    new SqlParameter("@id_usuario_accion", idAccion)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_activar @IdUsuario, @id_usuario_accion", 
                    parameters);
                return (true, "Asesor activado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deactivates an advisor in the system.
        /// </summary>
        /// <param name="asesor">The advisor user to deactivate.</param>
        /// <param name="idAccion">ID of the user performing the deactivation action.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the deactivation was successful
        /// - message: Descriptive message about the result
        /// </returns>
        /// <remarks>
        /// This method calls the stored procedure sp_usuario_desactivar to change the advisor's status.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string message)> DesactivarAsesor(Usuario asesor, int idAccion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", asesor.IdUsuario),
                    new SqlParameter("@id_usuario_accion", idAccion)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_desactivar @IdUsuario, @id_usuario_accion", 
                    parameters);
                return (true, "Asesor desactivado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}