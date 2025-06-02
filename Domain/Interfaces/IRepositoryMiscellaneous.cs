using ALFINapp.Domain.Entities;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryMiscellaneous
    {
        public Task<List<Agencia>> GetUAgenciasConNumeros();
    }
}