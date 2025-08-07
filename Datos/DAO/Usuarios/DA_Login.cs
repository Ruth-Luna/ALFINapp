using ALFINapp.API.Models;
using ALFINapp.Datos.DAO;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace ALFINapp.Datos
{
    public class DA_Login
    {
        public (bool Resultado, Usuario usuario) ValidarUsuario(string usuario, string password)
        {
            try
            {
                var cn = new Conexion();

                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_USUARIO_VALIDAR_LOGIN", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usuario", usuario);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                int resultado = Convert.ToInt32(dr["Resultado"]);

                                if (resultado == -1)
                                {
                                    Console.WriteLine("El usuario está inactivo.");
                                    return (false, new Usuario());
                                }

                                if (resultado == 1)
                                {
                                    return (true, new Usuario
                                    {
                                        IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                        usuario = dr["usuario"].ToString() ?? string.Empty,
                                        Correo = dr["correo"].ToString(),
                                        Nombres = dr["nombres"].ToString(),
                                        Apellido_Paterno = dr["apellido_paterno"].ToString(),
                                        Apellido_Materno = dr["apellido_materno"].ToString(),

                                        Estado = dr["estado"].ToString(),
                                        IdRol = Convert.ToInt32(dr["id_rol"]),
                                    });
                                }
                                return (false, new Usuario());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al validar usuario: " + ex.Message);
            }

            return (false, new Usuario());
        }

        public ViewUsuario ValidarCorreo_Usuario(string usuario, string correo)
        {
            try
            {
                var cn = new Conexion();

                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_USUARIO_VERIFICAR_USUARIO_CORREO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Usuario", usuario);
                        cmd.Parameters.AddWithValue("@Correo", correo);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                int resultado = Convert.ToInt32(dr["Resultado"]);
                                return new ViewUsuario { Resultado = resultado };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al validar correo y usuario: " + ex.Message);
            }

            return new ViewUsuario { Resultado = 0 };
        }

        public ViewCorreoRecuperacion InsertarSolicitudYObtenerCodigo(string correo, string usuario, string ipAddress)
        {
            var resultado = new ViewCorreoRecuperacion();

            try
            {
                using (SqlConnection conn = new SqlConnection(new Conexion().getCadenaSQL()))
                using (SqlCommand cmd = new SqlCommand("SP_USUARIO_INSERTAR_SOLICITUD_CORREO", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Correo", correo);
                    cmd.Parameters.AddWithValue("@IP", ipAddress);

                    var paramCodigo = new SqlParameter("@CodigoGenerado", SqlDbType.VarChar, 6)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(paramCodigo);

                    var paramIdUsuario = new SqlParameter("@IdUsuario", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(paramIdUsuario);

                    var paramIdSolicitud = new SqlParameter("@IdSolicitud", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(paramIdSolicitud);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    resultado.Codigo = paramCodigo.Value?.ToString();
                    resultado.Correo = correo;
                    resultado.IdUsuario = paramIdUsuario.Value != DBNull.Value ? Convert.ToInt32(paramIdUsuario.Value) : 0;
                    resultado.IdSolicitud = paramIdSolicitud.Value != DBNull.Value ? Convert.ToInt32(paramIdSolicitud.Value) : 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return resultado;
        }

        public int VerificarCodigoCorreo(int idUsuario, string codigo)
        {
            int resultado = -1;

            try
            {
                using (SqlConnection conn = new SqlConnection(new Conexion().getCadenaSQL()))
                using (SqlCommand cmd = new SqlCommand("SP_USUARIO_VERIFICAR_CODIGO_CORREO", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@CodigoIngresado", codigo);

                    // Valor de retorno
                    var returnParameter = new SqlParameter
                    {
                        ParameterName = "@ReturnVal",
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(returnParameter);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    // Obtener valor de retorno
                    resultado = (int)returnParameter.Value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al verificar código: " + ex.Message);
                resultado = -1;
            }

            return resultado;
        }

        public ViewCorreoRecuperacion VerificarEstadoCodigo(int idSolicitud)
        {
            ViewCorreoRecuperacion resultado = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(new Conexion().getCadenaSQL()))
                using (SqlCommand cmd = new SqlCommand("SP_USUARIO_VERIFICAR_ESTADO_CODIGO_CORREO", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idSolicitud", idSolicitud);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            resultado = new ViewCorreoRecuperacion
                            {
                                //IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                                FechaExpiracion = reader.GetDateTime(reader.GetOrdinal("FechaExpira")),
                                Estado = reader.IsDBNull(reader.GetOrdinal("Estado"))
                                    ? (bool?)null
                                    : reader.GetBoolean(reader.GetOrdinal("Estado"))
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener última solicitud: " + ex.Message);
            }

            return resultado;
        }

        public bool ActualizarContraseniaCorreo(int idUsuario, string nuevaContrasenia)
        {
            var cn = new Conexion();

            using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
            {
                using (SqlCommand cmd = new SqlCommand("SP_USUARIO_ACTUALIZAR_CONTRASENIA_CORREO", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@nueva_contrasenia", nuevaContrasenia);

                    try
                    {
                        connection.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int filas = reader.GetInt32(reader.GetOrdinal("filas_actualizadas"));
                                return filas > 0;
                            }
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("❌ Error al actualizar contraseña: " + ex.Message);
                        return false;
                    }
                }
            }
        }
    }

}
