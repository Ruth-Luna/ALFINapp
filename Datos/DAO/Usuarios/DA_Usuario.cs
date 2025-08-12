using ALFINapp.API.Models;
using ALFINapp.Datos.DAO;
using ALFINapp.Datos.Persistence.Procedures;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace ALFINapp.Datos
{
    public class DA_Usuario
    {
        public async Task<(bool IsSuccess, string Message)> CrearUsuario(ViewUsuario usuario, int idUsuarioAccion)
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
                        command.Parameters.AddWithValue("@Departamento", (object?)usuario.Departamento ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Provincia", (object?)usuario.Provincia ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Distrito", (object?)usuario.Distrito ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Telefono", (object?)usuario.Telefono ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Estado", (object?)usuario.Estado ?? "ACTIVO");
                        command.Parameters.AddWithValue("@IDUSUARIOSUP", (object?)usuario.IDUSUARIOSUP ?? DBNull.Value);
                        command.Parameters.AddWithValue("@RESPONSABLESUP", (object?)usuario.RESPONSABLESUP ?? DBNull.Value);
                        command.Parameters.AddWithValue("@REGION", (object?)usuario.REGION ?? DBNull.Value);
                        command.Parameters.AddWithValue("@NOMBRECAMPAÑA", (object?)usuario.NOMBRECAMPAÑA ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IdRol", usuario.IdRol);
                        command.Parameters.AddWithValue("@Usuario", usuario.Usuario);
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
                                Dni = dr["dni"].ToString() ?? string.Empty,
                                TipoDocumento = dr["tipo_doc"].ToString() ?? string.Empty,
                                Nombres = dr["Nombres"].ToString() ?? string.Empty,
                                Apellido_Paterno = dr["Apellido_Paterno"].ToString() ?? string.Empty,
                                Apellido_Materno = dr["Apellido_Materno"].ToString() ?? string.Empty,
                                Usuario = dr["Usuario"].ToString() ?? string.Empty,
                                Contrasenia = dr["contraseñaH"] == DBNull.Value
                                ? null
                                : System.Text.Encoding.Unicode.GetString((byte[])dr["contraseñaH"]),
                                Correo = dr["correo"].ToString() ?? string.Empty,
                                NombresCompletos = dr["Nombre_Completo"].ToString() ?? string.Empty,
                                NOMBRECAMPANIA = dr["NOMBRE_CAMPAÑA"].ToString() ?? string.Empty,
                                RESPONSABLESUP = dr["RESPONSABLE_SUP"].ToString() ?? string.Empty,
                                REGION = dr["region"].ToString() ?? string.Empty,
                                Rol = dr["rol"].ToString() ?? string.Empty,
                                Estado = dr["estado"].ToString() ?? string.Empty,
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

        public bool ActualizarUsuario(ViewUsuario usuario)
        {
            try
            {
                var cn = new Conexion();
                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_USUARIO_ACTUALIZAR_USUARIO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_usuario", usuario.IdUsuario);
                        cmd.Parameters.AddWithValue("@dni", usuario.Dni != null ? usuario.Dni : DBNull.Value);
                        cmd.Parameters.AddWithValue("@tipo_doc", usuario.TipoDocumento != null ? usuario.TipoDocumento : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Apellido_Paterno", usuario.Apellido_Paterno != null || usuario.Apellido_Paterno != "" ? usuario.Apellido_Paterno : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Apellido_Materno", usuario.Apellido_Materno != null || usuario.Apellido_Materno != "" ? usuario.Apellido_Materno : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Usuario", usuario.Usuario != null ? usuario.Usuario : DBNull.Value);
                        cmd.Parameters.Add("@contraseniaH", SqlDbType.VarBinary, -1).Value =
                            usuario.Contrasenia != null
                                ? (object)System.Text.Encoding.Unicode.GetBytes(usuario.Contrasenia)
                                : DBNull.Value;
                        cmd.Parameters.AddWithValue("@Nombres", usuario.Nombres != null ? usuario.Nombres : DBNull.Value);
                        cmd.Parameters.AddWithValue("@NOMBRE_CAMPANIA", usuario.NOMBRECAMPANIA != null ? usuario.NOMBRECAMPANIA : DBNull.Value);
                        cmd.Parameters.AddWithValue("@rol", usuario.Rol != null ? usuario.Rol : DBNull.Value);
                        cmd.Parameters.AddWithValue("@region", usuario.REGION != null ? usuario.REGION : DBNull.Value);
                        cmd.Parameters.AddWithValue("@correo", usuario.Correo != null ? usuario.Correo : DBNull.Value);
                        cmd.Parameters.AddWithValue("@estado", usuario.Estado != null ? usuario.Estado : DBNull.Value);

                        connection.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ActualizarUsuario: " + ex.Message);
                // Puedes lanzar la excepción para que suba al controlador y se capture allí también
                throw;
            }
        }
        public bool ActualizarEstado(int idUsuario, string estado)
        {
            try
            {
                var cn = new Conexion();
                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_USUARIO_ACTUALIZAR_ESTADO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                        cmd.Parameters.AddWithValue("@estado", estado);

                        connection.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ActualizarEstado: " + ex.Message);
                throw;
            }
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
        public List<Usuario> ListarAsesores(int? idUsuario = null)
        {
            try
            {
                var lista = new List<Usuario>();
                var cn = new Conexion();

                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_USUARIO_LISTAR_ASESORES_ASIGNADOS", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (idUsuario.HasValue)
                            cmd.Parameters.AddWithValue("@idUsuario", idUsuario.Value);

                        connection.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lista.Add(new Usuario
                                {
                                    IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                    NombresCompletos = dr["nombres_completos"].ToString(),
                                    Rol = dr["rol"].ToString(),
                                    Estado = dr["estado"].ToString(),
                                    IDUSUARIOSUP = Convert.ToInt32(dr["ID_USUARIO_SUP"]),
                                    IdRol = Convert.ToInt32(dr["id_rol"]),
                                    Dni = dr["dni"].ToString(),
                                    TipoDocumento = dr["tipo_doc"].ToString(),
                                    Telefono = dr["telefono"].ToString(),
                                });
                            }
                        }
                    }
                }

                return lista;
            }
            catch (System.Exception)
            {
                return new List<Usuario>();
            }
        }
        public List<Usuario> ListarSupervisores(int? idUsuario = null)
        {
            try
            {
                var lista = new List<Usuario>();
                var cn = new Conexion();
                using (var connection = new SqlConnection(cn.getCadenaSQL()))
                using (var cmd = new SqlCommand("SP_USUARIO_LISTAR_SUPERVISORES", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (idUsuario.HasValue)
                        cmd.Parameters.AddWithValue("@idUsuario", idUsuario.Value);

                    connection.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario
                            {
                                IdUsuario = dr.IsDBNull(dr.GetOrdinal("id_usuario")) ? 0 : dr.GetInt32(dr.GetOrdinal("id_usuario")),
                                Dni = dr.IsDBNull(dr.GetOrdinal("dni")) ? string.Empty : dr.GetString(dr.GetOrdinal("dni")),
                                NombresCompletos = dr.IsDBNull(dr.GetOrdinal("Nombres_Completos")) ? string.Empty : dr.GetString(dr.GetOrdinal("Nombres_Completos")),
                                Rol = dr.IsDBNull(dr.GetOrdinal("rol")) ? string.Empty : dr.GetString(dr.GetOrdinal("rol")),
                                Estado = dr.IsDBNull(dr.GetOrdinal("estado")) ? string.Empty : dr.GetString(dr.GetOrdinal("estado")),
                                IDUSUARIOSUP = dr.IsDBNull(dr.GetOrdinal("ID_USUARIO_SUP")) ? 0 : dr.GetInt32(dr.GetOrdinal("ID_USUARIO_SUP")),
                                RESPONSABLESUP = dr.IsDBNull(dr.GetOrdinal("RESPONSABLE_SUP")) ? string.Empty : dr.GetString(dr.GetOrdinal("RESPONSABLE_SUP")),
                                REGION = dr.IsDBNull(dr.GetOrdinal("REGION")) ? string.Empty : dr.GetString(dr.GetOrdinal("REGION")),
                            });
                        }
                    }
                }
                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ListarSupervisores: " + ex.Message);
                return new List<Usuario>();
            }
        }
        public Usuario? getUsuario(int idUsuario)
        {
            try
            {
                var cn = new Conexion();
                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_USUARIO_GET_USUARIO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                        connection.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                return new Usuario
                                {
                                    IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                    Dni = dr["dni"].ToString(),
                                    TipoDocumento = dr["tipo_doc"].ToString(),
                                    NombresCompletos = dr["Nombres_Completos"].ToString(),
                                    Rol = dr["rol"].ToString(),
                                    Estado = dr["estado"].ToString(),
                                    IdRol = Convert.ToInt32(dr["id_rol"]),
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en getUsuario: " + ex.Message);
                return null;
            }
        }

        public Usuario? getUsuarioPorDni(string dni)
        {
            try
            {
                var cn = new Conexion();
                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_USUARIO_GET_USUARIO_DNI", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@dni", dni);

                        connection.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                return new Usuario
                                {
                                    IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                    Dni = dr["dni"].ToString(),
                                    TipoDocumento = dr["tipo_doc"].ToString(),
                                    NombresCompletos = dr["Nombres_Completos"].ToString(),
                                    Rol = dr["rol"].ToString(),
                                    Estado = dr["estado"].ToString(),
                                    IdRol = Convert.ToInt32(dr["id_rol"]),
                                    IDUSUARIOSUP = dr.IsDBNull(dr.GetOrdinal("ID_USUARIO_SUP")) ? 0 : dr.GetInt32(dr.GetOrdinal("ID_USUARIO_SUP")),
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en getUsuario: " + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Updates a specific field for a user in the database.
        /// </summary>
        /// <param name="usuarioId">ID of the user to update.</param>
        /// <param name="campo">Name of the field/column to update.</param>
        /// <param name="nuevoValor">New value to set for the specified field.</param>
        /// <returns>A tuple containing success status and a message describing the result.</returns>
        public (bool IsSuccess, string Message) UpdateCampo(int usuarioId, string campo, string nuevoValor)
        {
            try
            {
                var usuario = getUsuario(usuarioId);
                if (usuario == null)
                {
                    return (false, "El usuario no existe");
                }
                var validFields = new List<string> { "Dni", "NombresCompletos", "Rol", "Departamento", "Provincia", "Distrito", "Telefono", "Estado", "IDUSUARIOSUP", "RESPONSABLESUP", "REGION", "NOMBRECAMPANIA", "IdRol", "TipoDocumento", "Correo" };
                if (!validFields.Contains(campo))
                {
                    return (false, $"El campo '{campo}' no es válido.");
                }
                usuario.GetType().GetProperty(campo)?.SetValue(usuario, nuevoValor);
                var usuariov = new ViewUsuario(usuario);
                var result = ActualizarUsuario(usuariov);
                return (true, "Campo actualizado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a hidden user by their DNI number.
        /// </summary>
        /// <param name="dni">Document identification number to search for.</param>
        /// <returns>
        /// A tuple containing:
        /// - Success status
        /// - Message describing the result
        /// - User data if found, otherwise null
        /// </returns>
        public (bool IsSuccess, string Message, AsesoresOcultos? Data) GetUsuarioOculto(string dni)
        {
            try
            {
                var cn = new Conexion();
                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_USUARIO_GET_USUARIO_OCULTO_DNI", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@dni", dni);

                        connection.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                var asesoroculto = new AsesoresOcultos
                                {
                                    DniVicidial = dr["DNI_VICIDIAL"].ToString(),
                                    NombreRealAsesor = dr["NOMBRE_REAL_ASESOR"].ToString(),
                                    NombreCambio = dr["NOMBRE_CAMBIO"].ToString(),
                                    DniAlBanco = dr["DNI_AL_BANCO"].ToString(),
                                    IdAsesorOculto = dr.IsDBNull(dr.GetOrdinal("id_asesor_oculto"))
                                        ? throw new Exception("IdAsesorOculto es nulo")
                                        : dr.GetInt32(dr.GetOrdinal("id_asesor_oculto")),
                                };
                                return (true, "Usuario encontrado", asesoroculto);
                            }
                        }
                    }
                }
                return (false, "Usuario no encontrado", null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}
