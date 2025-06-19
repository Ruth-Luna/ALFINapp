using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Asignaciones;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Asignaciones
{
    public class UseCaseDownloadAsignaciones : IUseCaseDownloadAsignaciones
    {
        private readonly IRepositoryAsignaciones repositoryAsignaciones;
        public UseCaseDownloadAsignaciones(IRepositoryAsignaciones repositoryAsignaciones)
        {
            this.repositoryAsignaciones = repositoryAsignaciones;
        }
        public async Task<(bool success, string message, DetallesAsignacionesDescargaSupDTO data)> exec(string? nombre_lista, int page = -1)
        {
            try
            {
                if (string.IsNullOrEmpty(nombre_lista))
                {
                    return (false, "El nombre de la lista no puede estar vacío.", new DetallesAsignacionesDescargaSupDTO());
                }
                var result = await repositoryAsignaciones.GetDetailedAssignmentsFromSupervisor(nombre_lista, page);
                if (!result.IsSuccess)
                {
                    return (false, result.Message, new DetallesAsignacionesDescargaSupDTO());
                }
                return (true, "Asignaciones obtenidas correctamente.", result.asignaciones);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, new DetallesAsignacionesDescargaSupDTO());
            }
        }
    }
}