using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositorySupervisor
    {
        public Task<DetallesInicioSupervisorDTO> GetInicioSupervisor(int idUsuario);
        public Task<DetallesAsignacionContadorFromVendedorDTO> GetContadorAllAsignacionesFromVendedor(List<int> IdsUsuariosVendedores, int idUsuarioS);
    }
}