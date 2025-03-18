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
        public List<ALFINapp.Application.DTOs.DetalleBaseClienteDTO>? GetClientesFromVendedor(int IdUsuarioVendedor);
        public Task<Usuario?> GetVendedor(int IdUsuario);
        public List<ALFINapp.Application.DTOs.DetalleBaseClienteDTO>? GetClientesAlfinFromVendedor(int IdUsuarioVendedor);
    }
}