namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryVendedor
    {
        public Task<Infrastructure.Persistence.Models.Usuario?> GetVendedor(int IdUsuario);
    }
}