namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryFiles
    {
        public Task<(bool success, string message)> UploadFilesAsync(string fileContent, string fileType, string fileName, int idDerivacion);
    }
}