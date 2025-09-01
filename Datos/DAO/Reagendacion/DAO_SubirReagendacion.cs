using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Reagendacion
{
    public class DAO_SubirReagendacion
    {
        private readonly MDbContext _context;
        public DAO_SubirReagendacion(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool success, string message)> reagendarCliente(int idDerivacion, DateTime fechaReagendamiento, List<string>? urls = null)
        {
            try
            {
                urls ??= new List<string>();
                var checkDis = await checkDisReagendacion(idDerivacion, fechaReagendamiento);
                if (!checkDis.success)
                {
                    return (false, checkDis.message);
                }
                var upload = await uploadReagendacion(idDerivacion, fechaReagendamiento, string.Join(",", urls));
                if (!upload.success)
                {
                    return (false, upload.message);
                }
                return (true, "Reagendamiento realizado con éxito.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool success, string message)> checkDisReagendacion(int idDerivacion, DateTime fechaReagendamiento)
        {
            try
            {
                var cn = new Conexion();

                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand command = new SqlCommand("[SP_reagendamiento_verificar_disponibilidad_para_reagendamiento_derivacion]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdDerivacion", idDerivacion);
                        command.Parameters.AddWithValue("@FechaDerivacion", fechaReagendamiento);
                        
                        connection.Open();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string mensaje = reader["mensaje"]?.ToString() ?? "Error desconocido";
                                int resultado = reader["resultado"] != DBNull.Value ? Convert.ToInt32(reader["resultado"]) : 1;

                                if (resultado == 1)
                                {
                                    return (false, mensaje);
                                }
                                else
                                {
                                    return (true, mensaje);
                                }
                            }
                            else
                            {
                                return (false, "No se ha encontrado la derivación.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (false, "Ha ocurrido un error en su red, o en la Base de Datos.");
            }
        }
        public async Task<(bool success, string message)> uploadReagendacion(int idDer, DateTime fechaReagendamiento, string urls)
        {
            try
            {
                var cn = new Conexion();

                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand command = new SqlCommand("[sp_reagendamiento_upload_nueva_reagendacion_refac]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@nueva_fecha_visita", SqlDbType.DateTime) { Value = fechaReagendamiento });
                        command.Parameters.Add(new SqlParameter("@id_derivacion", SqlDbType.Int) { Value = idDer });
                        command.Parameters.Add(new SqlParameter("@urls", SqlDbType.NVarChar) { Value = urls ?? string.Empty });

                        connection.Open();
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return (true, "Reagendacion subida correctamente");
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
                return (false, "Ha ocurrido un error en su red, o en la Base de Datos.");
            }
        }
    }
}