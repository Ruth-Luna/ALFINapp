using ALFINapp.Application.Interfaces.Consulta;
using ALFINapp.Domain.Interfaces;
using ALFINapp.API.Models;

namespace ALFINapp.Application.UseCases.Consulta
{
    public class UseCaseConsultaClienteDni : IUseCaseConsultaClienteDni
    {
        private readonly IRepositoryClientes _repositoryClientes;
        public UseCaseConsultaClienteDni(IRepositoryClientes repositoryClientes)
        {
            _repositoryClientes = repositoryClientes;
        }
        public async Task<(bool IsSuccess, string Message, ViewClienteDetalles Data)> Execute(string dni)
        {
            try
            {
                var cliente = await _repositoryClientes.getClientesFromDBandBank(dni);
                if (cliente.IsSuccess == false)
                {
                    return (false, cliente.Message, new ViewClienteDetalles());
                }
                var clienteDetallesVista = cliente.Data != null ? cliente.Data.toViewConsulta() : new ViewClienteDetalles();
                return (true, "Cliente encontrado", clienteDetallesVista);
            }
            catch (System.NullReferenceException ex)
            {
                return (false, $"Error: {ex.Message}", new ViewClienteDetalles());
            }
        }
    }
}