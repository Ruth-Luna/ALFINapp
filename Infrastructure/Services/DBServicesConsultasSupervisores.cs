using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore; // Assuming 'Usuario' is defined in the Models namespace

namespace ALFINapp.Infrastructure.Services
{

    public class DBServicesConsultasSupervisores
    {
        private readonly MDbContext _context;
        public DBServicesConsultasSupervisores(MDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene la lista de asesores asignados a un supervisor específico
        /// </summary>
        /// <param name="IdSupervisor">ID del supervisor del cual se desean obtener los asesores asignados. Puede ser null</param>
        /// <returns>
        /// Una tupla que contiene:
        /// - IsSuccess: Indica si la operación fue exitosa
        /// - Message: Mensaje descriptivo del resultado de la operación
        /// - Data: Lista de usuarios (asesores) asignados al supervisor. Puede ser null si ocurre un error
        /// </returns>
        /// <remarks>
        /// Este método consulta la base de datos para obtener todos los usuarios cuyo IDUSUARIOSUP
        /// coincida con el ID del supervisor proporcionado.
        /// </remarks>
        /// <exception cref="Exception">Se lanza cuando ocurre un error en la consulta a la base de datos</exception>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// var result = await GetAsesorsFromSupervisor(1);
        /// if (result.IsSuccess)
        /// {
        ///     var asesores = result.Data;
        ///     // Procesar la lista de asesores
        /// }
        /// </code>
        /// </example>
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
        /// This method retrieves the number of clients assigned to a specific asesor and supervisor.
        /// It uses a stored procedure to get the data and maps it to a DTO object.
        /// </summary>
        /// <param name="AsesorBusqueda">The asesor for which the number of clients will be retrieved.</param>
        /// <param name="IdSupervisor">The supervisor's id to filter the asesor.</param>
        /// <returns>
        /// A tuple containing a boolean indicating success or failure, a message describing the outcome,
        /// and a nullable VendedorConClientesDTO object containing the data.
        /// </returns>
        public async Task<(bool IsSuccess, string Message, VendedorConClientesDTO? Data)> GetNumberTipificacionesPlotedOnDTO(Usuario AsesorBusqueda, int IdSupervisor)
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