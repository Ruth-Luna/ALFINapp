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
                var cn = new Conexion();

                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand command = new SqlCommand("[sp_derivacion_upload_nueva_evidencia]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@id_derivacion", SqlDbType.Int) { Value = evidencias.idDerivacion });
                        command.Parameters.Add(new SqlParameter("@urls", SqlDbType.NVarChar, 4000) { Value = urls_string });

                        connection.Open();
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return (true, "Evidencia marcada como disponible y procesada correctamente");
                        }
                        else
                        {
                            return (false, "No se encontró la derivación para actualizar");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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