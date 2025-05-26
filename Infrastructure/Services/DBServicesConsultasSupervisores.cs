using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Services
{
    /// <summary>
    /// Service class that provides database operations for supervisor-related queries in the ALFINapp system.
    /// Handles retrieval of advisor data, client assignments, and supervisor performance metrics.
    /// </summary>
    public class DBServicesConsultasSupervisores
    {
        private readonly MDbContext _context;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DBServicesConsultasSupervisores"/> class.
        /// </summary>
        /// <param name="context">The database context used for database operations.</param>
        public DBServicesConsultasSupervisores(MDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Retrieves all advisors (asesores) assigned to a specific supervisor.
        /// </summary>
        /// <param name="IdSupervisor">ID of the supervisor to get assigned advisors for.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the operation was successful
        /// - Message: Descriptive message about the result
        /// - Data: List of Usuario objects representing the assigned advisors if successful, otherwise null
        /// </returns>
        /// <remarks>
        /// Uses the stored procedure SP_asesores_conseguir_asesores_asignados_no_contador to retrieve advisors,
        /// excluding those with counter roles.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string Message, List<Usuario>? Data)> GetAsesorsFromSupervisor(int IdSupervisor)
        {
            try
            {
                var asesores = await _context.usuarios.FromSqlRaw("EXEC SP_asesores_conseguir_asesores_asignados_no_contador @IdSupervisor = {0}", IdSupervisor).ToListAsync();

                if (asesores.Count == 0)
                {
                    return (false, "No hay asesores registrados para este supervisor", null);
                }
                return (true, "Los asesores registrados al supervisor han sido correctamente enviados", asesores);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ocurrió un error al obtener los asesores: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Retrieves the number of clients assigned to a specific advisor and supervisor.
        /// </summary>
        /// <param name="AsesorBusqueda">The advisor for which the number of clients will be retrieved.</param>
        /// <param name="IdSupervisor">The supervisor's ID to filter the advisor.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the operation was successful
        /// - Message: Descriptive message about the result
        /// - Data: VendedorConClientesDTO object containing client assignment data if successful, otherwise null
        /// </returns>
        /// <remarks>
        /// Uses the stored procedure SP_CONSEGUIR_NUM_CLIENTES_POR_ASESOR to get client count data.
        /// Maps results to a DTO that includes advisor information and activation status.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<(bool IsSuccess, string Message, VendedorConClientesDTO? Data)> GetNumberTipificacionesPlotedOnDTO(Usuario AsesorBusqueda, int IdSupervisor)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                var numeroClientes = _context.numeros_enteros_dto
                                                .FromSqlRaw("EXEC SP_CONSEGUIR_NUM_CLIENTES_POR_ASESOR @AsesorId = {0}, @SupervisorId = {1}", /// This stored Procedure is used to get the number of clients assigned to an asesor.
                                                AsesorBusqueda.IdUsuario, IdSupervisor)
                                                .AsEnumerable()
                                                .FirstOrDefault();
                if (numeroClientes == null)
                {
                    return (false, "El id del asesor es incorrecto.", null);
                }
                VendedorConClientesDTO vendedorClientesDTO = new VendedorConClientesDTO
                {
                    NombresCompletos = AsesorBusqueda.NombresCompletos,
                    IdUsuario = AsesorBusqueda.IdUsuario,
                    NumeroClientes = numeroClientes.NumeroEntero,
                    estaActivado = AsesorBusqueda.Estado == "ACTIVO" ? true : false
                };
                return (true, $"La Consulta se produjo con exito", vendedorClientesDTO);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ocurrió un error al obtener los asesores: {ex.Message}", null);
            }
        }
        
        /// <summary>
        /// Retrieves detailed information about all advisors assigned to a supervisor including client assignment metrics.
        /// </summary>
        /// <param name="idSupervisorActual">ID of the supervisor to query advisors for.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the operation was successful
        /// - Message: Descriptive message about the result
        /// - Data: List of UsuarioAsesorDTO objects with advisor details and client metrics if successful, otherwise null
        /// </returns>
        /// <remarks>
        /// Performs a complex LINQ query that:
        /// - Gets all users with role ID 3 (advisors) assigned to the specified supervisor
        /// - Left joins with client assignments table
        /// - Groups and aggregates data to calculate:
        ///   - Total assigned clients for current month/year
        ///   - Clients being worked on (those with tipificacion)
        ///   - Clients not being worked on
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string Message, List<UsuarioAsesorDTO>? Data)> ConsultaAsesoresDelSupervisor(int idSupervisorActual)
        {
            try
            {
                var asesores = await (from u in _context.usuarios
                                      where u.IdRol == 3 && u.IDUSUARIOSUP == idSupervisorActual
                                      join ca in _context.clientes_asignados on u.IdUsuario equals ca.IdUsuarioV into caGroup
                                      from ca in caGroup.DefaultIfEmpty()  // Realizamos un left join
                                      group new { u, ca }
                                      by new
                                      {
                                          u.IdUsuario,
                                          u.NombresCompletos,
                                          u.Dni,
                                          u.Telefono,
                                          u.Departamento,
                                          u.Provincia,
                                          u.Distrito,
                                          u.Estado,
                                          u.Rol
                                      } into grouped
                                      select new UsuarioAsesorDTO
                                      {
                                          IdUsuario = grouped.Key.IdUsuario,
                                          Dni = grouped.Key.Dni,
                                          NombresCompletos = grouped.Key.NombresCompletos,
                                          Telefono = grouped.Key.Telefono,
                                          Departamento = grouped.Key.Departamento,
                                          Provincia = grouped.Key.Provincia,
                                          Distrito = grouped.Key.Distrito,
                                          Estado = grouped.Key.Estado,
                                          Rol = grouped.Key.Rol,
                                          TotalClientesAsignados = grouped.Count(g => g.ca != null
                                                                                  && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                  && g.ca.IdUsuarioS == idSupervisorActual
                                                                                  && g.ca.FechaAsignacionVendedor != null
                                                                                  && g.ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                                  && g.ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month), // Clientes asignados
                                          ClientesTrabajando = grouped.Count(g => g.ca != null
                                                                                  && g.ca.TipificacionMayorPeso != null
                                                                                  && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                  && g.ca.IdUsuarioS == idSupervisorActual
                                                                                  && g.ca.FechaAsignacionVendedor != null
                                                                                  && g.ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                                  && g.ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month), // Clientes trabajados
                                          ClientesSinTrabajar = grouped.Count(g => g.ca != null
                                                                                  && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                  && g.ca.IdUsuarioS == idSupervisorActual)
                                                                                   - grouped.Count(g => g.ca != null
                                                                                  && g.ca.TipificacionMayorPeso != null
                                                                                  && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                  && g.ca.IdUsuarioS == idSupervisorActual)
                                      }).ToListAsync();
                return (true, $"La Consulta se produjo con exito", asesores);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ocurrió un error al obtener los asesores: {ex.Message}", null);
            }
        }
        
        /// <summary>
        /// Retrieves all client leads assigned to a supervisor, optionally filtered by destination.
        /// </summary>
        /// <param name="idSupervisorActual">ID of the supervisor to query leads for.</param>
        /// <param name="destino">Optional destination filter. If null, returns all leads.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the operation was successful
        /// - Message: Descriptive message about the result
        /// - Data: List of ClientesAsignado objects representing assigned leads if successful, otherwise null
        /// </returns>
        /// <remarks>
        /// Filters clients based on multiple criteria:
        /// - Assigned to the specified supervisor
        /// - Not disbursed (ClienteDesembolso != true)
        /// - Not withdrawn (ClienteRetirado != true)
        /// - Assigned in the current year and month
        /// - Matching the specified destination (if provided)
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string Message, List<ClientesAsignado>? Data)> ConsultaLeadsDelSupervisorDestino(int idSupervisorActual, string destino)
        {
            try
            {
                if (destino == null)
                {
                    var supervisorDataNoDestino = await (from ca in _context.clientes_asignados
                                                         where ca.IdUsuarioS == idSupervisorActual
                                                            && ca.ClienteDesembolso != true
                                                            && ca.ClienteRetirado != true
                                                            && ca.FechaAsignacionSup.HasValue
                                                            && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                                            && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                                         select ca).ToListAsync();
                    if (supervisorDataNoDestino.Count == 0)
                    {
                        return (true, "El presente Usuario Supervisor no tiene clientes Asignados", null);
                    }
                    return (true, "La Consulta se produjo con exito", supervisorDataNoDestino);
                }

                var supervisorData = await (from ca in _context.clientes_asignados
                                            where ca.IdUsuarioS == idSupervisorActual
                                               && ca.ClienteDesembolso != true
                                               && ca.ClienteRetirado != true
                                               && ca.FechaAsignacionSup.HasValue
                                               && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                               && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                               && ca.Destino == destino
                                            select ca).ToListAsync();
                if (supervisorData.Count == 0)
                {
                    return (true, "El presente Usuario Supervisor no tiene clientes Asignados", null);
                }
                return (true, "La Consulta se produjo con exito", supervisorData);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ocurrió un error al obtener los asesores: {ex.Message}", null);
            }
        }
    }
}