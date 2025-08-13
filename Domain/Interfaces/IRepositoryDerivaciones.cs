using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Entities;
using ALFINapp.DTOs;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryDerivaciones
    {
        public Task<DetallesDerivacionesAsesoresDTO?> getDerivacion(int idDer);
    }
}