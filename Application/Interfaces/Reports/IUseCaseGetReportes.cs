using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Reports
{
    public interface IUseCaseGetReportes
    {
        public Task<(bool IsSuccess, string Message, ViewReportesGeneral? Data)> Execute(int idUsuario);
    }
}