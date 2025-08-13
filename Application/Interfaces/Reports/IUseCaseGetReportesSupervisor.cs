using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Reports
{
    public interface IUseCaseGetReportesSupervisor
    {
        public Task<(bool IsSuccess, string Message, ViewReportesSupervisor? Data)> Execute(
            int idUsuario,
            int? anio = null,
            int? mes = null);
        
    }
}