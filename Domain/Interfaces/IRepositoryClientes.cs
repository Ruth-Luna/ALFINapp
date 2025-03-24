using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryClientes
    {
        public Task<List<DetallesAsignacionesDTO>?> GetAllAsignaciones(int idCliente);
        public Task<List<DetallesAsignacionesDTO>?> GetAllAsignaciones();
        public Task<List<DetallesAsignacionesDTO>?> GetAllAsignacionesTrabajadas();
        public Task<DetallesAsignacionesDTO?> GetAsignacion(int idAsignacion);
    }
}