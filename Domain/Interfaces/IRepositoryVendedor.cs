using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryVendedor
    {
        public Task<Usuario?> GetVendedor(int IdUsuario);
        public List<ALFINapp.Application.DTOs.DetalleBaseClienteDTO>? GetClientesAlfinFromVendedor(int IdUsuarioVendedor);
        public Task<List<DetalleBaseClienteDTO>> GetClientesGeneralPaginadoFromVendedor(
            int IdUsuarioVendedor,
            int IntervaloInicio,
            int IntervaloFin);
        public Task<List<DetalleBaseClienteDTO>> GetClientesFiltradoPaginadoFromVendedor(
            int IdUsuarioVendedor,
            string? filter,
            string? searchfield,
            int IntervaloInicio,
            int IntervaloFin);
    }
}