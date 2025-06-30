using ALFINapp.DTOs;

namespace ALFINapp.Application.Interfaces.Derivacion
{
    public interface IUseCaseUploadEvidencias
    {
        public Task<(bool success, string message)> Execute(List<DtoVUploadFiles> files);
    }
}