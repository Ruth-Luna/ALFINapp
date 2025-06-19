using ALFINapp.Application.DTOs;

namespace ALFINapp.Application.Interfaces.Asignaciones
{
    public interface IUseCaseDownloadAsignaciones
    {
        /// <summary>
        /// Executes the use case to download assignments.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation, containing a tuple with:
        /// - success: a boolean indicating if the operation was successful,
        /// - message: a string with the result message,
        /// - data: a list of assignments to be downloaded.
        /// </returns>
        public Task<(bool success, string message, DetallesAsignacionesDescargaSupDTO data)> exec(string? nombre_lista, int page = -1);
    }
}