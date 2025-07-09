using Microsoft.EntityFrameworkCore;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Infrastructure.Services
{
    /// <summary>
    /// Service class that provides database operations for tipificaciones (classifications/categorizations) in the ALFINapp system.
    /// </summary>
    public class DBServicesTipificaciones
    {
        private readonly MDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBServicesTipificaciones"/> class.
        /// </summary>
        /// <param name="context">The database context used for database operations.</param>
        public DBServicesTipificaciones(MDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Retrieves all tipificaciones (classifications) from the database.
        /// </summary>
        /// <returns>
        /// A tuple containing:
        /// - Success status indicating if the operation was successful
        /// - Message describing the result or error
        /// - List of Tipificaciones objects if successful, otherwise null
        /// </returns>
        public async Task<(bool IsSuccess, string Message, List<Tipificaciones>? Data)> ObtenerTipificaciones()
        {
            try
            {
                var tipificaciones = await _context.tipificaciones.ToListAsync();
                if (tipificaciones == null)
                {
                    return (false, "No se pudo encontrar tipificaciones en la base de datos", null);
                }
                return (true, "Se han encontrado las tipificaciones en la base de datos", tipificaciones);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        // Other DB services can be added here
    }
}