using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Vendedor
{
    public interface IUseCaseGetInicio
    {
        public Task<(bool IsSuccess, string Message, ViewInicioVendedor? Data)> Execute(int idUsuario);
    }
}