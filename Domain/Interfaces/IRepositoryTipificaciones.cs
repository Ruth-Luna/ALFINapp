using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryTipificaciones
    {
        public Task<IEnumerable<ClientesTipificado>> GetTipificaciones();
        public Task<bool> UploadTipificacion(ClientesTipificado clientesTipificado);
        public Task<Tipificaciones?> GetTipificacion (int id);
        public Task<bool> UploadGestionTip(ClientesAsignado clienteA, ClientesTipificado clienteT, ClientesEnriquecido clienteE, int idUsuario);
        public Task<List<Tipificaciones>> GetTipificacionesDescripcion();
    }
}