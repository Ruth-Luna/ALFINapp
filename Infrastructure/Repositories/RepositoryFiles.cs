using System.Data;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Domain.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryFiles : IRepositoryFiles
    {
        private readonly MDbContext _context;

        public RepositoryFiles(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool success, string message)> UploadFilesAsync(string fileContent, string fileType, string fileName, int idDerivacion)
        {
            try
            {
                byte[] fileContentBytes = ServicesHex.HexStringToBytes(fileContent);
                var sql =
                    "INSERT INTO [dbo].[files_evidencias_only_upload] ([file_name], [file_type], [file_content], [id_derivacion]) " +
                    "VALUES (@fileName, @fileType, @fileContent, @idDerivacion)";
                var parameters = new[]
                {
                    new SqlParameter("@fileName", fileName),
                    new SqlParameter("@fileType", fileType),
                    new SqlParameter("@fileContent", SqlDbType.VarBinary) { Value = fileContentBytes },
                    new SqlParameter("@idDerivacion", idDerivacion)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);
                if (result <= 0)
                {
                    return (true, "Se obvio el siguiente archivo porque no cumple con nuestras directrices.");
                }
                return (true, "Archivo subido correctamente.");
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al subir el archivo: {ex.Message}");
            }
            finally
            {
                // Aquí podrías liberar recursos si es necesario
                // Por ejemplo, cerrar conexiones o limpiar variables
                // En este caso, el contexto se maneja automáticamente por EF Core
                // y no es necesario hacer nada adicional.
                // _context.Dispose(); // No es necesario, EF Core maneja el ciclo de vida
                // del contexto automáticamente.
            }
        }
    }
}