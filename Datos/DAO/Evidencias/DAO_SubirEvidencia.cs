using System.Data;
using ALFINapp.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Evidencias
{
    public class DAO_SubirEvidencia
    {
        private readonly MDbContext _context;
        public DAO_SubirEvidencia(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool success, string message)> marcarEvidenciaDisponible(DtoVDerivacionEvidencia evidencias)
        {
            try
            {
                var verificar = verificarEvidenciaProcesada(evidencias.urlEvidencias);
                if (!verificar)
                {
                    return (false, "Error al verificar las evidencias procesadas. Por favor, vuelva a subir las imágenes de evidencia.");
                }
                var urls_string = string.Join(",", evidencias.urlEvidencias);
                var parametros = new[]
                {
                    new SqlParameter("@id_derivacion", evidencias.idDerivacion) { SqlDbType = SqlDbType.Int },
                    new SqlParameter("@urls", urls_string) { SqlDbType = SqlDbType.NVarChar, Size = 4000 }
                };

                var resultado = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_derivacion_upload_nueva_evidencia @id_derivacion, @urls", parametros);

                if (resultado == 0)
                {
                    return (false, "Error al marcar la evidencia como disponible");
                }

                return (true, "Evidencia marcada como disponible y procesada correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public bool verificarEvidenciaProcesada(List<string> urls)
        {
            try
            {
                foreach (var url in urls)
                {
                    var fileName = Path.GetFileName(url);
                    var checkImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp-images", fileName);

                    if (!File.Exists(checkImagePath))
                    {
                        throw new FileNotFoundException($"No se encontró la imagen: {fileName}");
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}