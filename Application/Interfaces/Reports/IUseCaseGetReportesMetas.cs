using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Reports
{
    public interface IUseCaseGetReportesMetas
    {
        public Task<(bool IsSuccess, string Message, ViewReportesMetas Data)> Execute(int idUsuario);
    }
}