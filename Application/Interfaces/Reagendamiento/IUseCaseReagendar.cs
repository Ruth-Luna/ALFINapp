using ALFINapp.API.DTOs;

namespace ALFINapp.Application.Interfaces.Reagendamiento
{
    public interface IUseCaseReagendar
    {
        public Task<(bool IsSuccess, string Message)> exec(
            int IdDerivacion,
            DateTime FechaReagendamiento);
    }
}