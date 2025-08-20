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
        public async Task<(bool success, string message)> Execute(DtoVDerivacionEvidencia evidencia)
        {
            try
            {
                var check = await _repositoryDerivaciones.marcarEvidenciaDisponible(evidencia.idDerivacion, evidencia.urlEvidencias);
                if (!check.success)
                {
                    return (false, check.message);
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