using ALFINapp.API.DTOs;
using ALFINapp.Application.Interfaces.Asignacion;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Asignacion
{
    public class UseCaseAsignarClientes : IUseCaseAsignarClientes
    {
        private readonly IRepositorySupervisor _repositorySupervisor;
        private readonly IRepositoryAsignacion _repositoryAsignacion;
        public UseCaseAsignarClientes(
            IRepositorySupervisor repositorySupervisor,
            IRepositoryAsignacion repositoryAsignacion)
        {
            _repositorySupervisor = repositorySupervisor;
            _repositoryAsignacion = repositoryAsignacion;
        }
        public async Task<(bool success, string message)> exec(
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
                var totalClientes = await _repositorySupervisor.GetAllAsignacionesFromDestino(idSupervisor, filter);
                if (totalClientes.Count == 0)
                {
                    return (false, "No se han encontrado clientes disponibles para la asignación.");
                }
                
                foreach (var asignacion in asignacionAsesor)
                {
                    if (asignacion.NumClientes == 0)
                    {
                        continue;
                    }

                    int nClientes = asignacion.NumClientes;
                    contadorClientesAsignados = contadorClientesAsignados + nClientes;
                    if (totalClientes.Count < nClientes)
                    {
                        mensajesError = mensajesError + $"En la base '{filter}', solo hay {totalClientes.Count} clientes disponibles para la asignación. La entrada ha sido obviada para el usuario '{asignacion.IdVendedor}'.";
                        continue;
                    }
                    if (contadorClientesAsignados > totalClientes.Count)
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
                    var clientesDisponibles = totalClientes
                        .Where(c => c.IdUsuarioV == null && c.Destino == filter)
                        .Take(nClientes)
                        .ToList();
                    totalClientes = totalClientes
                        .Where(c => c.IdUsuarioV == null && c.Destino == filter)
                        .Skip(nClientes)
                        .ToList();

                    foreach (var cliente in clientesDisponibles)
                    {
                        cliente.IdUsuarioV = asignacion.IdVendedor;
                        cliente.FechaAsignacionVendedor = DateTime.Now;
                    }
                    var actualizacion = await _repositoryAsignacion.AsignarClientesMasivoAsesor(clientesDisponibles);
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
    }
}