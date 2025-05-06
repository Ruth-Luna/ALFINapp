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

        public async Task<(bool success, string message)> AsignarClienteManual(string dniCliente, string baseTipo, int idVendedor)
        {
            try
            {
                var param = new SqlParameter[]
                {
                    new SqlParameter("@DNIBusqueda", dniCliente),
                    new SqlParameter("@IdUsuarioV", idVendedor),
                    new SqlParameter("@BaseTipo", baseTipo)
                };
                var result = await _context
                    .Database
                    .ExecuteSqlRawAsync("EXEC sp_Asignacion_cliente_manual @DNIBusqueda, @IdUsuarioV, @BaseTipo", param);
                if (result > 0)
                {
                    return (true, "Asignación manual exitosa");
                }
                else
                {
                    Console.WriteLine("No se pudo guardar la asignación manual del cliente.");
                    return (false, "No se pudo guardar la asignación manual del cliente.");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error en la asignación manual de cliente: {ex.Message}");
                return (false, $"Error en la asignación manual de cliente: {ex.Message}");
            }
        }

        public async Task<bool> AsignarClientesAsesor(DetallesAsignacionesDTO nuevaAsignacion)
        {
            try
            {
                var getClienteAsignado = nuevaAsignacion.convertToModel();
                var clienteAsignado = await _context.clientes_asignados
                    .FirstOrDefaultAsync(c => c.IdCliente == getClienteAsignado.IdCliente
                                              && c.IdUsuarioS == getClienteAsignado.IdUsuarioS
                                              && c.Destino == getClienteAsignado.Destino);
                if (clienteAsignado != null)
                {
                    clienteAsignado.IdUsuarioV = getClienteAsignado.IdUsuarioV;
                    clienteAsignado.FechaAsignacionVendedor = DateTime.Now;
                    clienteAsignado.ComentarioGeneral = getClienteAsignado.ComentarioGeneral;
                    clienteAsignado.TipificacionMayorPeso = getClienteAsignado.TipificacionMayorPeso;
                    clienteAsignado.PesoTipificacionMayor = getClienteAsignado.PesoTipificacionMayor;
                    clienteAsignado.ClienteDesembolso = getClienteAsignado.ClienteDesembolso;
                    clienteAsignado.ClienteRetirado = getClienteAsignado.ClienteRetirado;
                    clienteAsignado.FechaTipificacionMayorPeso = getClienteAsignado.FechaTipificacionMayorPeso;

                    _context.clientes_asignados.Update(clienteAsignado);
                }
                else
                {
                    _context.clientes_asignados.Add(getClienteAsignado);
                }
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("No se pudo guardar la asignación del cliente.");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error en la asignación de clientes: {ex.Message}");
                return false;
            }
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

        public Task<bool> AsignarClientesSupervisor(int idSupervisor, int idVendedor, string destino, int numClientes)
        {
            throw new NotImplementedException();
        }
    }
}