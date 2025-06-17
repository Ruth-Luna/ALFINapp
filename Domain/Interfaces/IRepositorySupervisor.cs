using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositorySupervisor
    {
        public Task<DetallesAsignacionContadorFromVendedorDTO> GetContadorAllAsignacionesFromVendedor(List<int> IdsUsuariosVendedores, int idUsuarioS);
        public Task<List<DetallesAsignacionesDTO>> GetAllAsignacionesFromDestino(int idUsuarioS, string filter = "", string type_filter = "");
        public Task<List<DetalleBaseClienteDTO>> GetClientesGeneralPaginadoFromSupervisor(int idUsuario);
        public Task<(int total, int totalAsignados, int totalPendientes)> GetCantidadClientesGeneralTotalFromSupervisor(int idUsuario);
    }
}