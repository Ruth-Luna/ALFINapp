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
        public RepositoryAsignaciones(MDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<(bool IsSuccess, string Message, string NombreLista)> CreateListName(string dni_supervisor)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (System.Exception ex)
            {

                throw;
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
                            cliente.Telefonos.ElementAtOrDefault(0),
                            cliente.Telefonos.ElementAtOrDefault(1),
                            cliente.Telefonos.ElementAtOrDefault(2),
                            cliente.Telefonos.ElementAtOrDefault(3),
                            cliente.Telefonos.ElementAtOrDefault(4),
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
    }
}