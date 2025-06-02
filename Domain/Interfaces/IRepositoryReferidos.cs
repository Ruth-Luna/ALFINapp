using ALFINapp.Domain.Entities;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryReferidos
    {
        public Task<(bool, string)> ReferirCliente(Cliente cliente);
    }
}