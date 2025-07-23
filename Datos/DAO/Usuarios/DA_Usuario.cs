using ALFINapp.API.Models;
using ALFINapp.Datos.DAO;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace ALFINapp.Datos
{
    public class DA_Usuario
    {
        public Usuario ValidarUsuario(string usuario, string password)
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
                                    return new Usuario { Resultado = -1 };
                                }

                                if (resultado == 1)
                                {
                                    return new Usuario
                                    {
                                        IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                        usuario = dr["usuario"].ToString(),
                                        Correo = dr["correo"].ToString(),
                                        Nombres = dr["nombres"].ToString(),
                                        Apellido_Paterno = dr["apellido_paterno"].ToString(),
                                        Apellido_Materno = dr["apellido_materno"].ToString(),
                                        Resultado = 1,
                                        Estado = dr["estado"].ToString(),
                                        IdRol = Convert.ToInt32(dr["id_rol"]),
                                    };
                                }
                                return new Usuario { Resultado = 0 };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al validar usuario: " + ex.Message);
            }

            return new Usuario { Resultado = 0 };
        }


        public async Task<(bool IsSuccess, string Message)> CrearUsuario(Usuario usuario, int idUsuarioAccion)
        {
            try
            {
                var cn = new Conexion();

                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand command = new SqlCommand("SP_USUARIO_REGISTRAR_NUEVO", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Dni", usuario.Dni);
                        command.Parameters.AddWithValue("@Paterno", usuario.Apellido_Paterno);
                        command.Parameters.AddWithValue("@Materno", usuario.Apellido_Materno);
                        command.Parameters.AddWithValue("@Nombres", usuario.Nombres);
                        command.Parameters.AddWithValue("@Rol", usuario.Rol ?? "ASESOR");
                        command.Parameters.AddWithValue("@Departamento", (object?)usuario.Departamento ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Provincia", (object?)usuario.Provincia ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Distrito", (object?)usuario.Distrito ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Telefono", (object?)usuario.Telefono ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Estado", (object?)usuario.Estado ?? "ACTIVO");
                        command.Parameters.AddWithValue("@IDUSUARIOSUP", (object?)usuario.IDUSUARIOSUP ?? DBNull.Value);
                        command.Parameters.AddWithValue("@RESPONSABLESUP", (object?)usuario.RESPONSABLESUP ?? DBNull.Value);
                        command.Parameters.AddWithValue("@REGION", (object?)usuario.REGION ?? DBNull.Value);
                        command.Parameters.AddWithValue("@NOMBRECAMPANIA", (object?)usuario.NOMBRECAMPANIA ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IdRol", usuario.IdRol);
                        command.Parameters.AddWithValue("@Usuario", usuario.usuario);
                        command.Parameters.AddWithValue("@id_usuario_accion", idUsuarioAccion);
                        command.Parameters.AddWithValue("@Correo", (object?)usuario.Correo ?? DBNull.Value);

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return (true, "Usuario creado correctamente");
            }
            catch (Exception ex)
            {
                return (false, "Error al crear el usuario: " + ex.Message);
            }
        }

        //private string LeerClaveMaestraDesdeAppSettings()
        //{
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        //        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        //    var configuration = builder.Build();
        //    return configuration["ClaveMaestra"] ?? string.Empty;
        //}


        public List<ViewUsuario> ListarUsuarios(int? idUsuario = null)
        {
            var lista = new List<ViewUsuario>();
            var cn = new Conexion();

            using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
            {
                using (SqlCommand cmd = new SqlCommand("SP_USUARIO_LISTAR_USUARIOS", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (idUsuario.HasValue)
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario.Value);

                    connection.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ViewUsuario
                            {
                                IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                Dni = dr["dni"].ToString(),
                                Nombres = dr["Nombres"].ToString(),
                                Apellido_Paterno = dr["Apellido_Paterno"].ToString(),
                                Apellido_Materno = dr["Apellido_Materno"].ToString(),
                                Correo = dr["correo"].ToString(),
                                NombresCompletos = dr["Nombre_Completo"].ToString(),
                                NOMBRECAMPANIA = dr["NOMBRE_CAMPAÑA"].ToString(),
                                RESPONSABLESUP = dr["RESPONSABLE_SUP"].ToString(),
                                REGION = dr["region"].ToString(),
                                Rol = dr["rol"].ToString(),
                                Estado = dr["estado"].ToString(),
                                FechaActualizacion = dr["fecha_actualizacion"] == DBNull.Value ? null : Convert.ToDateTime(dr["fecha_actualizacion"]),
                                FechaInicio = dr["fecha_inicio"] == DBNull.Value ? null : Convert.ToDateTime(dr["fecha_inicio"]),
                                FechaCese = dr["fecha_cese"] == DBNull.Value ? null : Convert.ToDateTime(dr["fecha_cese"]),
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public List<ViewRol> ListarRoles(int? idUsuario = null)
        {
            var lista = new List<ViewRol>();
            var cn = new Conexion();

            using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
            {
                using (SqlCommand cmd = new SqlCommand("SP_USUARIO_LISTAR_ROLES", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ViewRol
                            {
                                IdRol = Convert.ToInt32(dr["IdRol"]),
                                Rol = dr["Rol"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}
