using ALFINapp.Application.DTOs;

namespace ALFINapp.Application.Interfaces.Asignaciones
{
    public interface IUseCaseAsignarClientesSup
    {
        public Task<(bool IsSuccess, string Message)> AsignarMasivoAsync(
            DetallesAssignmentsMasive clientes);
    }
}