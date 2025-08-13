using System.Data;
using ALFINapp.API.DTOs;
using ALFINapp.Application.DTOs;
using ALFINapp.Datos.DAO.Miscelaneos;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Supervisores
{
    public class DAO_SupervisorAsignarClientesAAsesores
    {
        private readonly DA_Usuario _usuario = new DA_Usuario();
        private readonly DAO_ConsultasMiscelaneas _consultasMiscelaneas;
        private readonly MDbContext _context;
        private readonly DAO_SupervisorConsultas _dao_supervisorConsultas;
        public DAO_SupervisorAsignarClientesAAsesores(
            MDbContext context,
            DAO_ConsultasMiscelaneas consultasMiscelaneas,
            DAO_SupervisorConsultas dao_supervisorConsultas)
        {
            _context = context;
            _consultasMiscelaneas = consultasMiscelaneas;
            _dao_supervisorConsultas = dao_supervisorConsultas;
        }

        public async Task<(bool success, string message)> AsignarClientesAAsesores(
            List<DtoVAsignarClientes> asignacionAsesor,
            string filter,
            string type_filter,
            int idSupervisor)
        {
            try
            {

                string mensajesError = "";
                if (asignacionAsesor == null)
                {
                    return (false, "No se han enviado datos para asignar asesores.");
                }

                var valid_filters = new List<string>
                {
                    "lista",
                    "destino",
                    "fecha",
                    "base"
                };
                if (!valid_filters.Contains(type_filter))
                {
                    return (false, $"El tipo de filtro no es válido. Debe ser uno de los siguientes: {string.Join(", ", valid_filters)}");
                }

                if (string.IsNullOrEmpty(filter))
                {
                    return (false, "Debe seleccionar un Destino o Lista de la Base.");
                }

                if (asignacionAsesor.All(a => a.NumClientes == 0 || a.IdVendedor == 0))
                {
                    return (false, "No se ha llenado ninguna entrada. Los campos no pueden estar vacíos.");
                }
                int contadorClientesAsignados = 0;
                var clientes = await _dao_supervisorConsultas.ConsultarAsignaciones(idSupervisor, filter, type_filter);
                if (clientes.Data.Count == 0)
                {
                    return (false, "No se encontraron clientes para asignar.");
                }
                if (clientes.Data.All(c => c.IdUsuarioV != null))
                {
                    return (false, "No hay clientes disponibles para asignar en la base seleccionada.");
                }

                foreach (var asignacion in asignacionAsesor)
                {
                    if (asignacion.NumClientes == 0)
                    {
                        continue;
                    }

                    int nClientes = asignacion.NumClientes;
                    contadorClientesAsignados = contadorClientesAsignados + nClientes;
                    if (clientes.Data.Count < nClientes)
                    {
                        mensajesError = mensajesError + $"En la base '{filter}', solo hay {clientes.Data.Count} clientes disponibles para la asignación. La entrada ha sido obviada para el usuario '{asignacion.IdVendedor}'.";
                        continue;
                    }
                    if (contadorClientesAsignados > clientes.Data.Count)
                    {
                        mensajesError = mensajesError + $"Ha ocurrido un error al asignar los clientes. Esta mandando mas entradas del total disponible";
                        continue;
                    }
                }
                if (mensajesError != "")
                {
                    return (false, mensajesError);
                }
                contadorClientesAsignados = 0;
                foreach (var asignacion in asignacionAsesor)
                {
                    if (asignacion.NumClientes == 0)
                    {
                        continue;
                    }
                    int nClientes = asignacion.NumClientes;
                    contadorClientesAsignados = contadorClientesAsignados + nClientes;
                    var clientesDisponibles = clientes.Data
                        .Where(c => c.IdUsuarioV == null && c.Destino == filter)
                        .Take(nClientes)
                        .ToList();
                    clientes.Data = clientes.Data
                        .Where(c => c.IdUsuarioV == null && c.Destino == filter)
                        .Skip(nClientes)
                        .ToList();
                    foreach (var cliente in clientesDisponibles)
                    {
                        cliente.IdUsuarioV = asignacion.IdVendedor;
                        cliente.FechaAsignacionVendedor = DateTime.Now;
                    }
                    var actualizacion = await AsignarClientesMasivoAsesor(clientesDisponibles);
                    if (!actualizacion)
                    {
                        mensajesError = mensajesError + $"No se pudo asignar al asesor {asignacion.IdVendedor}. Se saltará la asignación de este asesor. ";
                    }
                }
                if (mensajesError != "")
                {
                    return (false, mensajesError);
                }
                else
                {
                    return (true, "Los clientes han sido asignados correctamente.");
                }
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<bool> AsignarClientesMasivoAsesor(List<ClientesAsignado> nuevasAsignaciones)
        {
            try
            {
                var table = new DataTable();
                table.Columns.Add("IdCliente", typeof(int));
                table.Columns.Add("IdUsuarioV", typeof(int));
                table.Columns.Add("IdAsignacion", typeof(int));

                foreach (var item in nuevasAsignaciones)
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