using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositorySupervisor
    {
        public Task<DetallesAsignacionContadorFromVendedorDTO> GetContadorAllAsignacionesFromVendedor(List<int> IdsUsuariosVendedores, int idUsuarioS);
        public Task<List<DetallesAsignacionesDTO>> GetAllAsignacionesFromDestino(int idUsuarioS, string filter = "", string type_filter = "");
        public Task<List<DetalleBaseClienteDTO>> GetClientesGeneralPaginadoYFiltradoFromSupervisor(
            int idUsuario,
            string filter = "",
            string search = "",
            string order = "tipificacion",
            bool orderAsc = true,
            int intervaloInicio = 0,
            int intervaloFin = 1);
        public Task<(int total, int totalAsignados, int totalPendientes)> GetCantidadClientesGeneralTotalFromSupervisor(
            int idUsuario,
            string filter = "",
            string search = ""
            );
        public Task<(bool IsSuccess, string Message, List<String?> Destinos)> GetDestinos(int idUsuario);
        public Task<(bool IsSuccess, string Message, List<String?> Listas)> GetListas(int idUsuario);
    }
}