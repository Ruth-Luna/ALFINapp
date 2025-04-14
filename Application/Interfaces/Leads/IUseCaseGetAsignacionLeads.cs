using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Leads
{
    public interface IUseCaseGetAsignacionLeads
    {
        public Task<(bool IsSuccess, string Message, ViewGestionLeads Data)> Execute(int usuarioId, int rol, int intervaloInicio = 0, int intervaloFin = 1);
    }
}