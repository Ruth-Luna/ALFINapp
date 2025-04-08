using ALFINapp.API.Models;
namespace ALFINapp.Application.Interfaces.Consulta
{
    public interface IUseCaseConsultaClienteDni
    {
        public Task<(bool IsSuccess, string Message, ViewClienteDetalles Data)> Execute(string dni);
    }
}