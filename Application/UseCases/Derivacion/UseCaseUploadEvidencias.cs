using ALFINapp.Application.Interfaces.Derivacion;
using ALFINapp.Domain.Interfaces;
using ALFINapp.DTOs;

namespace ALFINapp.Application.UseCases.Derivacion
{
    public class UseCaseUploadEvidencias : IUseCaseUploadEvidencias
    {
        private readonly IRepositoryFiles _repositoryEvidencias;
        private readonly IRepositoryDerivaciones _repositoryDerivaciones;
        public UseCaseUploadEvidencias(
            IRepositoryFiles repositoryEvidencias,
            IRepositoryDerivaciones repositoryDerivaciones)
        {
            _repositoryEvidencias = repositoryEvidencias;
            _repositoryDerivaciones = repositoryDerivaciones;
        }
        public async Task<(bool success, string message)> Execute(List<DtoVUploadFiles> files)
        {
            try
            {
                foreach (var file in files)
                {
                    if (string.IsNullOrEmpty(file.fileContent) || string.IsNullOrEmpty(file.fileType) || string.IsNullOrEmpty(file.fileName) || file.idDerivacion <= 0)
                    {
                        return (false, "Datos de archivo incompletos");
                    }
                    var result = await _repositoryEvidencias.UploadFilesAsync(
                                    file.fileContent,
                                    file.fileType,
                                    file.fileName,
                                    file.idDerivacion);
                    if (!result.success)
                    {
                        return (false, result.message);
                    }
                    var check = await _repositoryDerivaciones.marcarEvidenciaDisponible(file.idDerivacion);
                    if (!check.success)
                    {
                        return (false, check.message);
                    }
                }
                return (true, "Archivos subidos y procesados correctamente");
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al subir las evidencias: {ex.Message}");
            }
        }
    }
}