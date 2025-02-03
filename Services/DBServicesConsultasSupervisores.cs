using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.EntityFrameworkCore; // Assuming 'Usuario' is defined in the Models namespace

namespace ALFINapp.Services
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
        public async Task<(bool IsSuccess, string Message, List<Usuario>? Data)> GetAsesorsFromSupervisor(int? IdSupervisor)
        {
            try
            {
                var asesores = await _context.usuarios
                                    .Where(u => u.IDUSUARIOSUP == IdSupervisor)
                                    .ToListAsync();
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

        /// <summary>
        /// Obtiene las bases de clientes asignadas a un supervisor específico en el mes actual
        /// </summary>
        /// <param name="SupervisorId">ID del supervisor del cual se desean obtener las bases asignadas</param>
        /// <returns>
        /// Una tupla que contiene:
        /// - IsSuccess: Indica si la operación fue exitosa
        /// - Message: Mensaje descriptivo del resultado de la operación
        /// - Data: Lista de nombres de bases asignadas. Puede ser null si ocurre un error
        /// </returns>
        /// <remarks>
        /// Este método:
        /// - Consulta la tabla clientes_asignados
        /// - Filtra por supervisor y mes/año actual
        /// - Retorna nombres únicos de bases (FuenteBase)
        /// - Los resultados se limitan al mes y año actual de la consulta
        /// </remarks>
        /// <exception cref="Exception">Se lanza cuando ocurre un error en la consulta a la base de datos</exception>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// var result = await GetBasesClientes(supervisorId);
        /// if (result.IsSuccess)
        /// {
        ///     var bases = result.Data;
        ///     // Procesar la lista de bases
        /// }
        /// </code>
        /// </example>
        public async Task<(bool IsSuccess, string Message, List<string>? Data)> GetBasesClientes(int SupervisorId)
        {
            try
            {
                var BasesAsignadas = await (from ca in _context.clientes_asignados
                                            where ca.IdUsuarioS == SupervisorId
                                                && ca.FechaAsignacionSup.HasValue
                                                && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                                && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                            select new { ca.FuenteBase })
                                                .Distinct()
                                                .ToListAsync();

                if (BasesAsignadas == null)
                {
                    return (false, "Ocurrió un error al obtener la base de los Asesores", null);
                }

                var BasesAsignadasMapeadas = new List<string>();
                foreach (var BaseAsignada in BasesAsignadas)
                {
                    BasesAsignadasMapeadas.Add(BaseAsignada.FuenteBase);
                }
                return (true, $"Ocurrió un error al obtener los asesores", BasesAsignadasMapeadas);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ocurrió un error al obtener los asesores: {ex.Message}", null);
            }
        }
        public async Task<(bool IsSuccess, string Message, List<Usuario>? Data)> GetGeneralSupervisores()
        {
            try
            {
                var supervisores = await _context.usuarios
                                    .Where(u => u.Rol == "SUPERVISOR")
                                    .ToListAsync();
                if (supervisores.Count == 0)
                {
                    return (false, "No hay supervisores registrados", null);
                }
                return (true, "Los supervisores han sido correctamente enviados", supervisores);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ocurrió un error al obtener los supervisores: {ex.Message}", null);
            }
        }

        public async Task<(bool IsSuccess, string Message, SupervisorConAsesoresDTO? Data)> GetAssessorsFromSupervisor(Usuario SupervisorBusqueda)
        {
            try
            {
                var IdVendedoresAsignados = await (from ca in _context.clientes_asignados
                                                   where ca.IdUsuarioS == SupervisorBusqueda.IdUsuario
                                                           && ca.FechaAsignacionSup.HasValue
                                                           && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                                           && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                                   select ca.IdUsuarioV).Distinct().ToListAsync();

                if (IdVendedoresAsignados.Count() == 0)
                {
                    // El supervisor no tiene asesores asignados
                    return (true, "El supervisor no tiene asesores asignados", null);
                }

                var AsesoresDelSupervisor = await _context.usuarios
                    .Where(u => IdVendedoresAsignados.Contains(u.IdUsuario))
                    .ToListAsync();

                var DataToBeReturned = new SupervisorConAsesoresDTO
                {
                    TotalAsesores = IdVendedoresAsignados.Count(),
                    AsesoresDelSupervisor = AsesoresDelSupervisor
                };
                return (true, $"La Consulta se produjo con exito", DataToBeReturned);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ocurrió un error al obtener los supervisores: {ex.Message}", null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<Usuario>? Data)> GetAllSupervisor()
        {
            try
            {
                var GetAllSupervisor = await _context.usuarios
                                    .Where(u => u.Rol == "SUPERVISOR")
                                    .ToListAsync();
                if (GetAllSupervisor.Count == 0)
                {
                    return (false, "No hay supervisores registrados", null);
                }
                return (true, "La consulta se desarrollo con exito", GetAllSupervisor);
            }
            catch (System.Exception ex)
            {
                return (false, "Ocurrio un error con una consulta: " + ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<Usuario>? Data)> GetAllAssessors()
        {
            try
            {
                var GetAllAssessors = await _context.usuarios
                                   .Where(u => u.Rol == "VENDEDOR")
                                   .ToListAsync();
                if (GetAllAssessors.Count == 0)
                {
                    return (false, "No hay asesores registrados para este supervisor", null);
                }
                return (true, "La consulta se desarrollo con exito", GetAllAssessors);
            }
            catch (System.Exception ex)
            {
                return (false, "Ocurrio un error con una consulta: " + ex.Message, null);
            }
        }
        //... other methods...
    }
}