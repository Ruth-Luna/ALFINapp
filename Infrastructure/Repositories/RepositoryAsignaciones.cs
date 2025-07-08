using System.Data;
using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryAsignaciones : IRepositoryAsignaciones
    {
        private readonly MDbContext _context;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        public RepositoryAsignaciones(
            MDbContext context,
            IRepositoryUsuarios repositoryUsuarios)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositoryUsuarios = repositoryUsuarios ?? throw new ArgumentNullException(nameof(repositoryUsuarios));
        }
        private object ToDbValue(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? DBNull.Value : input;
        }
        public async Task<(bool IsSuccess, string Message, string NombreLista)> CreateListName(string dni_supervisor)
        {
            try
            {
                var getSupervisor = await _repositoryUsuarios.GetUser(dni_supervisor);

                if (getSupervisor.IsSuccess == false || getSupervisor.user == null)
                {
                    return (false, "El supervisor no existe o no se pudo encontrar.", string.Empty);
                }

                var existingList = await _context.listas_asignacion
                    .Where(l => l.IdUsuarioSup == getSupervisor.user.IdUsuario)
                    .Where(l => l.FechaCreacion != null)
                    .OrderByDescending(l => l.FechaCreacion.Value)
                    .FirstOrDefaultAsync();

                var nombreLista = String.Empty;
                if (existingList != null
                    && existingList.FechaCreacion != null
                    && existingList.FechaCreacion.Value.Date == DateTime.Now.Date
                    && existingList.FechaCreacion.Value.Month == DateTime.Now.Month
                    && existingList.FechaCreacion.Value.Year == DateTime.Now.Year)
                {
                    nombreLista = existingList.NombreLista;
                    if (string.IsNullOrWhiteSpace(nombreLista))
                    {
                        nombreLista = $"{dni_supervisor}_{DateTime.Now:yyyyMMdd}_{existingList.IdLista}";
                        existingList.NombreLista = nombreLista;
                        _context.listas_asignacion.Update(existingList);
                        await _context.SaveChangesAsync();
                    }
                    return (true, $"Lista de asignación ya existe, puede usar la lista {nombreLista}.", nombreLista);
                }

                var idParam = new SqlParameter("@id_lista", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                var supParam = new SqlParameter("@id_usuario_sup", SqlDbType.Int)
                {
                    Value = getSupervisor.user.IdUsuario
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.SP_GESTION_ASIGNACION_CREAR_LISTAS_DE_ASIGNACION @id_usuario_sup, @id_lista OUTPUT",
                    supParam, idParam
                );

                int idLista = (int)idParam.Value;


                var listaActualizra = await _context.listas_asignacion
                    .FirstOrDefaultAsync(l => l.IdLista == idLista);
                if (listaActualizra == null)
                {
                    return (false, "No se pudo crear la lista de asignación.", string.Empty);
                }
                nombreLista = $"{dni_supervisor}_{DateTime.Now:yyyyMMdd}_{listaActualizra.IdLista}";
                listaActualizra.NombreLista = nombreLista;
                _context.listas_asignacion.Update(listaActualizra);
                await _context.SaveChangesAsync();
                
                return (true, "Lista de asignación creada correctamente.", nombreLista);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, string.Empty);
            }
        }

        public async Task<(bool IsSuccess, string Message)> CrossAssignments(DetallesAssignmentsMasive asignaciones)
        {
            try
            {
                _context.Database.SetCommandTimeout(800);

                if (asignaciones == null || asignaciones.SupervisoresConClientes.Count == 0)
                {
                    return (false, "No se proporcionaron asignaciones para procesar.");
                }
                // Subir los clientes a una tabla para el procedimiento almacenado
                await _context.Database.ExecuteSqlRawAsync(@"
                    DROP TABLE IF EXISTS dbo.base_aux_cruz;
                ");

                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE dbo.base_aux_cruz (
                        dni_cliente NVARCHAR(20) NOT NULL,
                        dni_supervisor NVARCHAR(20) NULL,
                        celular_1 NVARCHAR(20) NULL,
                        celular_2 NVARCHAR(20) NULL,
                        celular_3 NVARCHAR(20) NULL,
                        celular_4 NVARCHAR(20) NULL,
                        celular_5 NVARCHAR(20) NULL,
                        d_base NVARCHAR(100) NULL
                        );
                    ");

                var dataTable = new DataTable();
                dataTable.Columns.Add("dni_cliente", typeof(string));
                dataTable.Columns.Add("dni_supervisor", typeof(string));
                dataTable.Columns.Add("celular_1", typeof(string));
                dataTable.Columns.Add("celular_2", typeof(string));
                dataTable.Columns.Add("celular_3", typeof(string));
                dataTable.Columns.Add("celular_4", typeof(string));
                dataTable.Columns.Add("celular_5", typeof(string));
                dataTable.Columns.Add("d_base", typeof(string));

                // Llenar el DataTable con los datos de asignaciones
                foreach (var supervisor in asignaciones.SupervisoresConClientes)
                {
                    foreach (var cliente in supervisor.Clientes)
                    {
                        dataTable.Rows.Add(
                            cliente.Dni ?? throw new ArgumentNullException(nameof(cliente.Dni)),
                            supervisor.DniSupervisor ?? throw new ArgumentNullException(nameof(supervisor.DniSupervisor)),
                            ToDbValue(cliente.Telefonos.ElementAtOrDefault(0) ?? string.Empty),
                            ToDbValue(cliente.Telefonos.ElementAtOrDefault(1) ?? string.Empty),
                            ToDbValue(cliente.Telefonos.ElementAtOrDefault(2) ?? string.Empty),
                            ToDbValue(cliente.Telefonos.ElementAtOrDefault(3) ?? string.Empty),
                            ToDbValue(cliente.Telefonos.ElementAtOrDefault(4) ?? string.Empty),
                            cliente.FuenteBase ?? string.Empty
                        );
                    }
                }

                var connection = (SqlConnection)_context.Database.GetDbConnection();
                bool wasClosed = connection.State == ConnectionState.Closed;
                if (wasClosed) connection.Open();

                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "dbo.base_aux_cruz";
                    await bulkCopy.WriteToServerAsync(dataTable);
                }

                if (wasClosed) connection.Close();

                
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.SP_GESTION_ASIGNACION_CRUZAR_DATA_EXTERNA_REFACTORIZADO");
                if (result < 0)
                {
                    return (false, "Error al procesar las asignaciones cruzadas.");
                }
                return (true, "Asignaciones cruzadas procesadas correctamente");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<List<ClienteCruceDTO>> GetCrossed(int page = 1)
        {
            var resultado = new List<ClienteCruceDTO>();

            _context.Database.SetCommandTimeout(600);
            if (page < 1) page = 1;
            var connection = _context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SP_ASIGNACION_CRUCE_DNIS";
                command.Parameters.Add(new SqlParameter("@Page", SqlDbType.Int) { Value = page });
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        resultado.Add(new ClienteCruceDTO
                        {
                            DniCliente = reader["dni"]?.ToString() ?? string.Empty,
                            ClienteNombre = reader["CLIENTE"]?.ToString() ?? string.Empty,
                            Campaña = reader["campaña"]?.ToString() ?? string.Empty,
                            OfertaMax = reader["oferta_max"]?.ToString() ?? string.Empty,
                            Agencia = reader["agencia_comercial"]?.ToString() ?? string.Empty,
                            TipoBase = reader["tipo_base"]?.ToString() ?? string.Empty,
                            SupervisorNombre = reader["Nombres_Completos"]?.ToString() ?? string.Empty,
                            NombreLista = reader["nombre_lista"]?.ToString() ?? string.Empty,
                            FuenteBase = reader["d_base"]?.ToString() ?? string.Empty,
                        });
                    }
                }
            }

            return resultado;
        }

        public async Task<(bool IsSuccess, string Message, int numAsignaciones)> AssignLeads(string dni_supervisor, string nombre_lista)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@DniSup", dni_supervisor),
                    new SqlParameter("@NombreLista", nombre_lista)
                };
                _context.Database.SetCommandTimeout(600);
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.SP_GESTION_ASIGNACION_AUTO_REFACTORIZADA @DniSup, @NombreLista",
                    parameters
                );
                return (true, "Clientes asignados correctamente.", result);
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al asignar clientes: {ex.Message}", 0);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<DetallesAsignacionesDelSupDTO> asignaciones)> GetAllAssignmentsFromSupervisor()
        {
            try
            {
                var result = await _context.gestion_conseguir_todas_las_asignaciones_por_listas
                    .FromSqlRaw("EXEC dbo.SP_GESTION_CONSEGUIR_TODAS_LAS_ASIGNACIONES_POR_LISTAS")
                    .ToListAsync();
                if (result == null || result.Count == 0)
                {
                    return (false, "No se encontraron asignaciones para el supervisor. O es probable que no hayan listas existentes durante este mes", new List<DetallesAsignacionesDelSupDTO>());
                }
                var detallesAsignaciones = result.Select(x => new DetallesAsignacionesDelSupDTO(x)).ToList();
                return (true, "Asignaciones obtenidas correctamente.", detallesAsignaciones);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al obtener las asignaciones: {ex.Message}");
                return (false, $"Error en la base de datos al obtener las listas de asignacion", new List<DetallesAsignacionesDelSupDTO>());
            }
        }

        public async Task<(bool IsSuccess, string Message, DetallesAsignacionesDescargaSupDTO asignaciones)> GetDetailedAssignmentsFromSupervisor(string nombre_lista, int page = -1)
        {
            try
            {
                var dni_supervisor = nombre_lista.Split('_')[0];
                var usuarioinfo = await _repositoryUsuarios.GetUser(dni_supervisor);
                if (!usuarioinfo.IsSuccess || usuarioinfo.user == null)
                {
                    return (false, "El supervisor no existe o no se pudo encontrar.", new DetallesAsignacionesDescargaSupDTO());
                }
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@NombreLista", nombre_lista),
                    new SqlParameter("@Pagina", page)
                };
                _context.Database.SetCommandTimeout(600);
                var result = await _context.gestion_conseguir_o_descargar_asignacion_de_leads_de_sup
                    .FromSqlRaw("EXEC dbo.SP_GESTION_CONSEGUIR_O_DESCARGAR_ASIGNACION_DE_LEADS_DE_SUPERVISORES @NombreLista, @Pagina", parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (result == null || result.Count == 0)
                {
                    return (false, "No se encontraron asignaciones detalladas para el supervisor.", new DetallesAsignacionesDescargaSupDTO());
                }
                var detallesAsignaciones = new DetallesAsignacionesDescargaSupDTO(result);
                detallesAsignaciones.dni_supervisor = dni_supervisor;
                detallesAsignaciones.nombres_supervisor = usuarioinfo.user.NombresCompletos ?? "INGRESAR NOMBRE DEL SUPERVISOR";
                return (true, "Asignaciones detalladas obtenidas correctamente.", detallesAsignaciones);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al obtener las asignaciones detalladas: {ex.Message}");
                return (false, "Error al obtener las asignaciones detalladas del supervisor.", new DetallesAsignacionesDescargaSupDTO());
            }
        }
    }
}