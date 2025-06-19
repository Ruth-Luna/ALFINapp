using ALFINapp.Models;

namespace ALFINapp.Application.Interfaces.Asignaciones
{
    public interface IUseCaseGetAsignacionesDelSup
    {
        public Task<(bool success, string message, ViewVerAsignacionesDelSupervisor data)> exec();
    }
}