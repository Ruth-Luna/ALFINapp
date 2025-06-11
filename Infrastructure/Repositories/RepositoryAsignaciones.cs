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

                var nombreLista = $"{dni_supervisor}_{DateTime.Now:yyyyMMdd}_1";
                var getSupervisor = await _repositoryUsuarios.GetUser(dni_supervisor);

                if (getSupervisor.IsSuccess == false || getSupervisor.user == null)
                {
                    return (false, "El supervisor no existe o no se pudo encontrar.", string.Empty);
                }

                var existingList = await _context.listas_asignacion
                    .FirstOrDefaultAsync(l => l.NombreLista == nombreLista && l.IdUsuarioSup == getSupervisor.user.IdUsuario);

                if (existingList != null)
                {
                    nombreLista = $"{dni_supervisor}_{DateTime.Now:yyyyMMdd}_{existingList.IdLista + 1}";
                }

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@nombre_lista", nombreLista),
                    new SqlParameter("@id_usuario_sup", getSupervisor.user.IdUsuario)
                };

                var createList = _context.Database.ExecuteSqlRaw(
                    "EXEC dbo.SP_GESTION_ASIGNACION_CREAR_LISTAS_DE_ASIGNACION @nombre_lista, @id_usuario_sup",
                    parameters
                    );
                    
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

                _context.Database.SetCommandTimeout(600);
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

        public async Task<List<ClienteCruceDTO>> GetCrossed()
        {
            var resultado = new List<ClienteCruceDTO>();

            var connection = _context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SP_ASIGNACION_CRUCE_DNIS";
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        resultado.Add(new ClienteCruceDTO
                        {
                            DniCliente = reader["dni"]?.ToString(),
                            ClienteNombre = reader["CLIENTE"]?.ToString(),
                            Campaña = reader["campaña"]?.ToString(),
                            OfertaMax = reader["oferta_max"]?.ToString(),
                            Agencia = reader["agencia_comercial"]?.ToString(),
                            TipoBase = reader["tipo_base"]?.ToString(),
                            SupervisorNombre = reader["Nombres_Completos"]?.ToString(),
                            NombreLista = reader["nombre_lista"]?.ToString(),
                            FuenteBase = reader["d_base"]?.ToString()
                        });
                    }
                }
            }

            return resultado;
        }

        public async Task<(bool IsSuccess, string Message)> AssignLeads(string dni_supervisor, string nombre_lista)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@DniSup", dni_supervisor),
                    new SqlParameter("@NombreLista", nombre_lista)
                };
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.SP_GESTION_ASIGNACION_AUTO_REFACTORIZADA @DniSup, @NombreLista",
                    parameters
                );
                return (true, "Clientes asignados correctamente.");
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al asignar clientes: {ex.Message}");
            }
        }
    }
}