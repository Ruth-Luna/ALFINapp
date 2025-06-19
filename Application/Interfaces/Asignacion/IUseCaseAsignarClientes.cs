using ALFINapp.API.DTOs;

namespace ALFINapp.Application.Interfaces.Asignacion
{
    public interface IUseCaseAsignarClientes
    {
        public Task<(bool success, string message)> exec(
            List<DtoVAsignarClientes> asignacionAsesor,
            string filter,
            string type_filter,
            int idSupervisor);
    }
}