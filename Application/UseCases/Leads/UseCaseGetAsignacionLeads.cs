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
        public UseCaseGetAsignacionLeads(IRepositoryVendedor repositoryVendedor)
        {
            _repositoryVendedor = repositoryVendedor;
        }
        public async Task<(bool IsSuccess, string Message, ViewGestionLeads Data)> Execute(
            int usuarioId,
            int intervaloInicio = 0,
            int intervaloFin = 1)
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
                var clientes = await _repositoryVendedor.GetClientesGeneralPaginadoFromVendedor(usuarioId, intervaloInicio, intervaloFin);
                if (clientes == null)
                {
                    return (true, "No se encontraron clientes", new ViewGestionLeads());
                }
                var convertView = new ViewGestionLeads
                {
                    ClientesA365 = clientes.Select(c => c.DtoToCliente()).ToList(),
                    Vendedor = usuarioDTO.ToEntityVendedor(),
                    ClientesAlfin = new List<Cliente>(),
                    clientesPendientes = clientes.Count(dc =>
                        (!dc.FechaTipificacionDeMayorPeso.HasValue ||
                        (dc.FechaTipificacionDeMayorPeso.Value.Year != currentYear &&
                        dc.FechaTipificacionDeMayorPeso.Value.Month != currentMonth))),
                    clientesTipificados = clientes.Count(dc =>
                        dc.FechaTipificacionDeMayorPeso.HasValue &&
                        dc.FechaTipificacionDeMayorPeso.Value.Year == currentYear &&
                        dc.FechaTipificacionDeMayorPeso.Value.Month == currentMonth),
                    clientesTotal = clientes.Count()
                };
                return (true, "Se encontraron los siguientes clientes", convertView);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewGestionLeads());
            }
        }
    }
}