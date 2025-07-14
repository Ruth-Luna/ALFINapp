using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Consulta;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Consulta
{
    public class UseCaseConsultaClienteTelefono : IUseCaseConsultaClienteTelefono
    {
        private readonly IRepositoryClientes _repositoryClientes;
        public UseCaseConsultaClienteTelefono(IRepositoryClientes repositoryClientes)
        {
            _repositoryClientes = repositoryClientes;
        }
        public async Task<(bool IsSuccess, string Message, ViewClienteDetalles Data)> exec(string telefono)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(telefono))
                {
                    return (false, "El telefono no puede estar vacio", new ViewClienteDetalles());
                }
                if (telefono.Length < 7 || telefono.Length > 15)
                {
                    return (false, "El telefono debe tener entre 7 y 15 caracteres", new ViewClienteDetalles());
                }
                if (!telefono.All(char.IsDigit))
                {
                    return (false, "El telefono solo puede contener numeros", new ViewClienteDetalles());
                }
                var cliente = await _repositoryClientes.getClientesFromTelefono(telefono);
                if (!cliente.IsSuccess)
                {
                    return (false, cliente.Message, new ViewClienteDetalles());
                }
                var clienteDetalles = cliente.Data != null ? cliente.Data.toViewConsulta() : new ViewClienteDetalles();
                return (true, "Consulta realizada con exito", clienteDetalles);
            }
            catch (System.Exception)
            {
                return (false, "Error", new ViewClienteDetalles());
            }
        }
    }
}