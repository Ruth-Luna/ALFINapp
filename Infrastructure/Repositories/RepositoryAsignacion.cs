using System.Data;
using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryAsignacion : IRepositoryAsignacion
    {
        private readonly MDbContext _context;
        public RepositoryAsignacion(MDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AsignarClientesMasivoAsesor(List<DetallesAsignacionesDTO> nuevasAsignaciones)
        {
            try
            {
                var clientesAsignados = nuevasAsignaciones.Select(c => c.convertToModel()).ToList();
                
                var table = new DataTable();
                table.Columns.Add("IdCliente", typeof(int));
                table.Columns.Add("IdUsuarioV", typeof(int));
                table.Columns.Add("IdAsignacion", typeof(int));

                foreach (var item in clientesAsignados)
                {
                    table.Rows.Add(item.IdCliente, item.IdUsuarioV, item.IdAsignacion);
                }

                var param = new SqlParameter("@asignaciones", SqlDbType.Structured)
                {
                    TypeName = "dbo.asignacion_clientes_masivo",
                    Value = table
                };
                        
                var updateAsignacion = await _context
                    .Database
                    .ExecuteSqlRawAsync("EXEC sp_Asignacion_clientes_masivo @asignaciones", 
                        param);

                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error en la asignación masiva de clientes: {ex.Message}");
                return false;
            }
        }
    }
}