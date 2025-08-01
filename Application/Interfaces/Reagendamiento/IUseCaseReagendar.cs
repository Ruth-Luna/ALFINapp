using ALFINapp.API.DTOs;
using ALFINapp.DTOs;

namespace ALFINapp.Application.Interfaces.Reagendamiento
{
    public interface IUseCaseReagendar
    {
        public Task<(bool IsSuccess, string Message)> exec(
            int IdDerivacion,
            DateTime FechaReagendamiento,
            List<string>? urls = null);
    }
}