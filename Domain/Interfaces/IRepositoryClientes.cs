using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryClientes
    {
        public Task<List<DetallesAsignacionesDTO>?> GetAllAsignaciones(int idCliente);
        public Task<List<DetallesAsignacionesDTO>?> GetAllAsignaciones();
        public Task<List<DetallesAsignacionesDTO>?> GetAllAsignacionesTrabajadas();
        public Task<DetallesAsignacionesDTO?> GetAsignacion(int idAsignacion);
        public Task<ClientesEnriquecido?> GetEnriquecido(int idCliente);
        public Task<ClientesEnriquecido?> GetEnriquecidoxBase(int idBase);
        public Task<bool> UpdateAsignacion(ClientesAsignado asignacion);
        public Task<bool> UpdateEnriquecido(ClientesEnriquecido enriquecido);
        public Task<BaseCliente?> getBase(int idBase);
        public Task<BaseCliente?> getBase(string dni);
        public Task<(bool IsSuccess, string Message, Application.DTOs.DetallesClienteDTO? Data)> getClientesFromDBandBank (string dni);
        public Task<(bool IsSuccess, string Message, Application.DTOs.DetallesClienteDTO? Data)> getClientesFromTelefono (string telefono);
    }
}