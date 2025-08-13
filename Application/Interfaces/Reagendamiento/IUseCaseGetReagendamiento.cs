using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Reagendamiento
{
    public interface IUseCaseGetReagendamiento
    {
        public Task<(bool IsSuccess, string Message, ViewClienteReagendado Data)> exec(
            int IdDerivacion);
    }
}