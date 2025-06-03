using ALFINapp.Domain.Entities;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryReferidos
    {
        public Task<(bool IsSuccess, string Message)> ReferirCliente(Cliente cliente, Vendedor asesor);
        public Task<(bool IsSuccess, string Message)> EnviarCorreoReferido(string dni);
    }
}