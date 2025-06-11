using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Leads;
using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.UseCases.Leads
{
    public class UseCaseGetAsignacionLeads : IUseCaseGetAsignacionLeads
    {
        private readonly IRepositoryVendedor _repositoryVendedor;
        private readonly IRepositorySupervisor _repositorySupervisor;
        public UseCaseGetAsignacionLeads(
            IRepositoryVendedor repositoryVendedor,
            IRepositorySupervisor repositorySupervisor)
        {
            _repositoryVendedor = repositoryVendedor;
            _repositorySupervisor = repositorySupervisor;
        }
        public async Task<(bool IsSuccess, string Message, ViewGestionLeads Data)> Execute(
            int usuarioId,
            int rol,
            int intervaloInicio = 0,
            int intervaloFin = 1,
            string filter = "",
            string search = "",
            string order = "tipificacion",
            bool orderAsc = true)
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                var usuario = await _repositoryVendedor.GetVendedor(usuarioId);
                if (usuario == null)
                {
                    return (false, "No se encontró el usuario", new ViewGestionLeads());
                }
                var usuarioDTO = new DetallesUsuarioDTO(usuario);
                var clientes = new List<DetalleBaseClienteDTO>();
                if (rol == 3)
                {
                    clientes = await _repositoryVendedor
                        .GetClientesGeneralPaginadoFromVendedor(
                            usuarioId, 
                            intervaloInicio, 
                            intervaloFin,
                            filter,
                            search,
                            order,
                            orderAsc);
                    if (clientes == null)
                    {
                        return (true, "No se encontraron clientes", new ViewGestionLeads());
                    }
                    var cantidades = await _repositoryVendedor.GetCantidadClientesGeneralTotalFromVendedor (usuarioId);
                    if (cantidades.IsSuccess == false)
                    {
                        return (false, "Ha ocurrido un error en la consulta", new ViewGestionLeads());
                    }
                    var convertView = new ViewGestionLeads
                    {
                        ClientesA365 = clientes.Select(c => c.DtoToCliente()).ToList(),
                        Vendedor = usuarioDTO.ToEntityVendedor(),
                        ClientesAlfin = new List<Cliente>(),
                        clientesPendientes = cantidades.Pendientes,
                        clientesTipificados = cantidades.Tipificados,
                        clientesTotal = cantidades.Total,
                        filtro = filter,
                        searchfield = search,
                        order = order,
                        orderAsc = orderAsc,
                        PaginaActual = intervaloInicio,
                    };
                    return (true, "Se encontraron los siguientes clientes", convertView);
                }
                else if (rol == 2)
                {
                    clientes = await _repositorySupervisor.GetClientesGeneralPaginadoFromSupervisor(usuarioId);
                    if (clientes == null)
                    {
                        return (true, "No se encontraron clientes", new ViewGestionLeads());
                    }
                    var destinos = clientes.Select(c => c.Destino).Distinct().ToList();
                    var cantidades = await _repositorySupervisor.GetCantidadClientesGeneralTotalFromSupervisor(usuarioId);
                    var convertView = new ViewGestionLeads
                    {
                        ClientesA365 = clientes.Select(c => c.DtoToCliente()).ToList(),
                        Supervisor = usuarioDTO.ToEntitySupervisor(),
                        ClientesAlfin = new List<Cliente>(),
                        clientesPendientes = cantidades.totalPendientes,
                        clientesTipificados = cantidades.totalAsignados,
                        clientesTotal = cantidades.total,
                        destinoBases = destinos
                            .Where(destino => destino != null)
                            .Cast<string>()
                            .ToList()
                    };
                    return (true, "Se encontraron los siguientes clientes", convertView);
                }
                else
                {
                    return (false, "No se encontró el rol del usuario", new ViewGestionLeads());
                }

            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewGestionLeads());
            }
        }
    }
}