using ALFINapp.API.DTOs;

namespace ALFINapp.Application.Interfaces.Tipificacion
{
    public interface IUseCaseUploadTipificaciones
    {
        public Task<(bool success, string message)> execute(int idUsuario, List<DtoVTipificarCliente> tipificaciones, int IdAsignacion, int typeTipificacion);
    }
}