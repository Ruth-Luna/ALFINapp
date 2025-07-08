using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
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
        /// Filters client databases based on multiple criteria for assignment purposes.
        /// </summary>
        /// <param name="base_busqueda">Database source to filter by (DBA365 or DBALFIN).</param>
        /// <param name="rango_edad">Age range filter.</param>
        /// <param name="rango_tasas">Rate range filter.</param>
        /// <param name="oferta">Minimum offer amount filter.</param>
        /// <param name="tipo_cliente">Client type filter.</param>
        /// <param name="cliente_estado">Client status filter.</param>
        /// <param name="grupo_tasa">Rate group filter.</param>
        /// <param name="grupo_monto">Amount group filter.</param>
        /// <param name="deudas">Debt flag filter.</param>
        /// <param name="campaña">List of campaign names to filter by.</param>
        /// <param name="usuarios">List of user names to filter by.</param>
        /// <param name="propension">List of propensity values to filter by.</param>
        /// <param name="color">List of color classifications to filter by.</param>
        /// <param name="color_final">List of final color classifications to filter by.</param>
        /// <param name="frescura">List of data freshness indicators to filter by.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the operation was successful
        /// - Message: Descriptive message about the result
        /// - Data: List of AsignacionFiltrarBasesDTO objects matching the criteria if successful, otherwise null
        /// </returns>
        /// <remarks>
        /// Uses the stored procedure sp_Asignacion_FiltrarBases to perform advanced filtering.
        /// Parameters are optional - null values are passed as DBNull to the stored procedure.
        /// List parameters are converted to comma-separated strings before passing to the stored procedure.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string Message, List<AsignacionFiltrarBasesDTO>? Data)> AsignacionFiltrarBases(
            string? base_busqueda,
            string? rango_edad,
            string? rango_tasas,
            decimal? oferta,
            string? tipo_cliente,
            string? cliente_estado,
            string? grupo_tasa,
            string? grupo_monto,
            int? deudas,
            List<string>? campaña,
            List<string>? usuarios,
            List<string>? propension,
            List<string>? color,
            List<string>? color_final,
            List<string>? frescura)
        {
            try
            {
                var asignacionFiltrarBases = await _context.asignacion_filtrar_bases_dto
                    .FromSqlRaw("EXEC sp_Asignacion_FiltrarBases @base = {0}, @campaña = {1}, @oferta = {2}, @usuario = {3}, @propension = {4}, @color = {5}, @color_final = {6}, @rango_edad = {7}, @rango_sueldo = {8}, @rango_tasa = {9}, @rango_oferta = {10}, @tipo_cliente = {11}, @cliente_estado = {12}, @grupo_tasa = {13}, @grupo_monto = {14}, @deudas = {15}, @frescura = {16}",
                        new SqlParameter("@base", base_busqueda ?? (object)DBNull.Value),
                        new SqlParameter("@campaña", campaña != null ? string.Join(",", campaña) : (object)DBNull.Value),
                        new SqlParameter("@oferta", oferta ?? (object)DBNull.Value),
                        new SqlParameter("@usuario", usuarios != null ? string.Join(",", usuarios) : (object)DBNull.Value),
                        new SqlParameter("@propension", propension != null ? string.Join(",", propension) : (object)DBNull.Value),
                        new SqlParameter("@color", color != null ? string.Join(",", color) : (object)DBNull.Value),
                        new SqlParameter("@color_final", color_final != null ? string.Join(",", color_final) : (object)DBNull.Value),
                        new SqlParameter("@rango_edad", rango_edad ?? (object)DBNull.Value),
                        new SqlParameter("@rango_sueldo", (object)DBNull.Value),
                        new SqlParameter("@rango_tasa", rango_tasas ?? (object)DBNull.Value),
                        new SqlParameter("@rango_oferta", (object)DBNull.Value),
                        new SqlParameter("@tipo_cliente", tipo_cliente ?? (object)DBNull.Value),
                        new SqlParameter("@cliente_estado", cliente_estado ?? (object)DBNull.Value),
                        new SqlParameter("@grupo_tasa", grupo_tasa ?? (object)DBNull.Value),
                        new SqlParameter("@grupo_monto", grupo_monto ?? (object)DBNull.Value),
                        new SqlParameter("@deudas", deudas ?? (object)DBNull.Value),
                        new SqlParameter("@frescura", frescura != null ? string.Join(",", frescura) : (object)DBNull.Value)
                    )
                    .ToListAsync();

                if (asignacionFiltrarBases == null)
                {
                    return (false, "La Asignacion al Filtrar Bases no se ha encontrado", null);
                }

                return (true, "Asignacion Filtrar Bases ha devuelto una tabla", asignacionFiltrarBases);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
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
        /// <summary>
        /// Retrieves all users registered in the system regardless of role.
        /// </summary>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the operation was successful
        /// - Message: Descriptive message about the result
        /// - Data: List of Usuario objects representing all system users if successful, otherwise null
        /// </returns>
        /// <remarks>
        /// Uses the stored procedure sp_usuario_conseguir_todos to retrieve all users.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string Message, List<Usuario>? Data)> ConseguirTodosLosUsuarios()
        {
            try
            {
                var TodosLosUsuarios = await _context.usuarios.FromSqlRaw("EXEC sp_usuario_conseguir_todos").ToListAsync();
                if (TodosLosUsuarios == null)
                {
                    return (false, "No se han encontrado usuarios este error fue inesperado", null);
                }
                return (true, "Se han encontrado los siguientes usuarios", TodosLosUsuarios);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}