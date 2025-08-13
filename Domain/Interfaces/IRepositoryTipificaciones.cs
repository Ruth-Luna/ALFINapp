using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryTipificaciones
    {
        public Task<List<Tipificaciones>> GetTipificacionesDescripcion();
    }
}