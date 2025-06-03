using ALFINapp.Domain.Entities;

namespace ALFINapp.Application.Interfaces.Referidos
{
    public interface IUseCaseReferirCliente
    {
        public Task<(bool IsSuccess, string Message)> Execute(
            Cliente cliente,
            ALFINapp.Domain.Entities.Vendedor asesor);
    }
}