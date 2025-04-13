using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Vendedor;
using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;

namespace Application.UseCases.Vendedor
{
    public class UseCaseGetInicio : IUseCaseGetInicio
    {
        private readonly IRepositoryVendedor _repositoryVendedor;
        public UseCaseGetInicio(IRepositoryVendedor repositoryVendedor)
        {
            _repositoryVendedor = repositoryVendedor;
        }
        public async Task<(bool IsSuccess, string Message, ViewInicioVendedor? Data)> Execute(int idUsuario)
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            var usuario = await _repositoryVendedor.GetVendedor(idUsuario);
            if (usuario == null)
            {
                return (false, "No se encontró el usuario", null);
            }

            var usuarioVendedor = new DetallesUsuarioDTO (usuario);

            var convertVarInicio = new ViewInicioVendedor
            {
                Vendedor = usuarioVendedor.ToEntityVendedor(),
            };

            return (true, "Clientes encontrados", convertVarInicio);
        }
    }
}