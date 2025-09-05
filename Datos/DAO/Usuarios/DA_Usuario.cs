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

                        command.Parameters.AddWithValue("@Dni", string.IsNullOrWhiteSpace(usuario.Dni) ? DBNull.Value : usuario.Dni);
                        command.Parameters.AddWithValue("@Paterno", string.IsNullOrWhiteSpace(usuario.Apellido_Paterno) ? DBNull.Value : usuario.Apellido_Paterno);
                        command.Parameters.AddWithValue("@Materno", string.IsNullOrWhiteSpace(usuario.Apellido_Materno) ? DBNull.Value : usuario.Apellido_Materno);
                        command.Parameters.AddWithValue("@Nombres", string.IsNullOrWhiteSpace(usuario.Nombres) ? DBNull.Value : usuario.Nombres);
                        command.Parameters.AddWithValue("@Departamento", string.IsNullOrWhiteSpace(usuario.Departamento) ? DBNull.Value : usuario.Departamento);
                        command.Parameters.AddWithValue("@Provincia", string.IsNullOrWhiteSpace(usuario.Provincia) ? DBNull.Value : usuario.Provincia);
                        command.Parameters.AddWithValue("@Distrito", string.IsNullOrWhiteSpace(usuario.Distrito) ? DBNull.Value : usuario.Distrito);
                        command.Parameters.AddWithValue("@Telefono", string.IsNullOrWhiteSpace(usuario.Telefono) ? DBNull.Value : usuario.Telefono);
                        command.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(usuario.Estado) ? "ACTIVO" : usuario.Estado);
                        command.Parameters.AddWithValue("@IDUSUARIOSUP", usuario.IDUSUARIOSUP == 0 ? DBNull.Value : usuario.IDUSUARIOSUP);
                        command.Parameters.AddWithValue("@RESPONSABLESUP", string.IsNullOrWhiteSpace(usuario.RESPONSABLESUP) ? DBNull.Value : usuario.RESPONSABLESUP);
                        command.Parameters.AddWithValue("@REGION", string.IsNullOrWhiteSpace(usuario.REGION) ? DBNull.Value : usuario.REGION);
                        command.Parameters.AddWithValue("@NOMBRECAMPAÑA", string.IsNullOrWhiteSpace(usuario.NOMBRECAMPANIA) ? DBNull.Value : usuario.NOMBRECAMPANIA);
                        command.Parameters.AddWithValue("@IdRol", usuario.IdRol);
                        command.Parameters.AddWithValue("@Usuario", string.IsNullOrWhiteSpace(usuario.Usuario) ? DBNull.Value : usuario.Usuario);
                        command.Parameters.AddWithValue("@id_usuario_accion", idUsuarioAccion);
                        command.Parameters.AddWithValue("@Correo", string.IsNullOrWhiteSpace(usuario.Correo) ? DBNull.Value : usuario.Correo);
                        command.Parameters.AddWithValue("@TipoDocumento", string.IsNullOrWhiteSpace(usuario.TipoDocumento) ? DBNull.Value : usuario.TipoDocumento);

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


        public List<ViewUsuario> ListarUsuarios(int? idUsuario = null, int? idSupervisor = null)
        {
            var lista = new List<ViewUsuario>();
            var cn = new Conexion();

            using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_USUARIO_LISTAR_USUARIOS]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario.HasValue ? idUsuario.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_supervisor", idSupervisor.HasValue ? idSupervisor.Value : DBNull.Value);

                    connection.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ViewUsuario
                            {
                                IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                Dni = dr["dni"].ToString() ?? string.Empty,
                                TipoDocumento = dr["tipo_documento"].ToString() ?? string.Empty,
                                Nombres = dr["Nombres"].ToString() ?? string.Empty,
                                Apellido_Paterno = dr["Apellido_Paterno"].ToString() ?? string.Empty,
                                Apellido_Materno = dr["Apellido_Materno"].ToString() ?? string.Empty,
                                Usuario = dr["Usuario"].ToString() ?? string.Empty,
                                Contrasenia = dr["Contrasenia"].ToString() ?? string.Empty,
                                Correo = dr["correo"].ToString() ?? string.Empty,
                                NombresCompletos = dr["Nombres_Completos"].ToString() ?? string.Empty,
                                NOMBRECAMPANIA = dr["NOMBRE_CAMPAÑA"].ToString() ?? string.Empty,
                                RESPONSABLESUP = dr["RESPONSABLE_SUP"].ToString() ?? string.Empty,
                                IDUSUARIOSUP = dr["ID_USUARIO_SUP"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID_USUARIO_SUP"]),
                                REGION = dr["region"].ToString() ?? string.Empty,
                                Distrito = dr["distrito"].ToString() ?? string.Empty,
                                Provincia = dr["provincia"].ToString() ?? string.Empty,
                                Departamento = dr["departamento"].ToString() ?? string.Empty,
                                Telefono = dr["telefono"].ToString() ?? string.Empty,
                                Rol = dr["Rol"].ToString() ?? string.Empty,
                                IdRol = dr["id_rol"] == DBNull.Value ? 0 : Convert.ToInt32(dr["id_rol"]),
                                Estado = dr["estado"].ToString() ?? string.Empty,
                                FechaActualizacion = dr["fecha_actualizacion"] == DBNull.Value ? null : Convert.ToDateTime(dr["fecha_actualizacion"]),
                                FechaInicio = dr["fecha_inicio"] == DBNull.Value ? null : Convert.ToDateTime(dr["fecha_inicio"]),
                                FechaRegistro = dr["fecha_registro"] == DBNull.Value ? null : Convert.ToDateTime(dr["fecha_registro"]),
                                FechaCese = dr["fecha_cese"] == DBNull.Value ? null : Convert.ToDateTime(dr["fecha_cese"]),
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<(bool IsSuccess, string Message)> ActualizarUsuario(ViewUsuario usuario)
        {
            bool isSuccess = false;
            string message = string.Empty;

            try
            {
                var cn = new Conexion();
                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_USUARIO_ACTUALIZAR_USUARIO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_usuario", usuario.IdUsuario);
                        cmd.Parameters.AddWithValue("@dni", string.IsNullOrWhiteSpace(usuario.Dni) ? DBNull.Value : usuario.Dni);
                        cmd.Parameters.AddWithValue("@tipo_doc", string.IsNullOrWhiteSpace(usuario.TipoDocumento) ? DBNull.Value : usuario.TipoDocumento);
                        cmd.Parameters.AddWithValue("@Apellido_Paterno", string.IsNullOrWhiteSpace(usuario.Apellido_Paterno) ? DBNull.Value : usuario.Apellido_Paterno);
                        cmd.Parameters.AddWithValue("@Apellido_Materno", string.IsNullOrWhiteSpace(usuario.Apellido_Materno) ? DBNull.Value : usuario.Apellido_Materno);
                        cmd.Parameters.AddWithValue("@Usuario", string.IsNullOrWhiteSpace(usuario.Usuario) ? DBNull.Value : usuario.Usuario);
                        cmd.Parameters.AddWithValue("@contrasenia", string.IsNullOrWhiteSpace(usuario.Contrasenia) ? DBNull.Value : usuario.Contrasenia);
                        cmd.Parameters.AddWithValue("@Nombres", string.IsNullOrWhiteSpace(usuario.Nombres) ? DBNull.Value : usuario.Nombres);
                        cmd.Parameters.AddWithValue("@NOMBRE_CAMPANIA", string.IsNullOrWhiteSpace(usuario.NOMBRECAMPANIA) ? DBNull.Value : usuario.NOMBRECAMPANIA);
                        cmd.Parameters.AddWithValue("@idRol", usuario.IdRol == 0 ? DBNull.Value : usuario.IdRol);
                        cmd.Parameters.AddWithValue("@region", string.IsNullOrWhiteSpace(usuario.REGION) ? DBNull.Value : usuario.REGION);
                        cmd.Parameters.AddWithValue("@correo", string.IsNullOrWhiteSpace(usuario.Correo) ? DBNull.Value : usuario.Correo);
                        cmd.Parameters.AddWithValue("@estado", string.IsNullOrWhiteSpace(usuario.Estado) ? DBNull.Value : usuario.Estado);

                        cmd.Parameters.AddWithValue("@Departamento", string.IsNullOrWhiteSpace(usuario.Departamento) ? DBNull.Value : usuario.Departamento);
                        cmd.Parameters.AddWithValue("@Provincia", string.IsNullOrWhiteSpace(usuario.Provincia) ? DBNull.Value : usuario.Provincia);
                        cmd.Parameters.AddWithValue("@Distrito", string.IsNullOrWhiteSpace(usuario.Distrito) ? DBNull.Value : usuario.Distrito);


                        cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrWhiteSpace(usuario.Telefono) ? DBNull.Value : usuario.Telefono);

                        // Datos supervisor
                        cmd.Parameters.AddWithValue("@IDUSUARIOSUP", usuario.IDUSUARIOSUP == 0 ? DBNull.Value : usuario.IDUSUARIOSUP);
                        cmd.Parameters.AddWithValue("@RESPONSABLESUP", string.IsNullOrWhiteSpace(usuario.RESPONSABLESUP) ? DBNull.Value : usuario.RESPONSABLESUP);

                        await connection.OpenAsync();
                        // Recoger resultado y mensaje
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                isSuccess = reader.GetBoolean(reader.GetOrdinal("resultado"));
                                message = reader.GetString(reader.GetOrdinal("mensaje"));
                            }
                            return (isSuccess, message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ActualizarUsuario: " + ex.Message);
                return (false, "Error al procesar la solicitud");
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
                                    IdUsuario       = dr["id_usuario"]     != DBNull.Value ? Convert.ToInt32(dr["id_usuario"]) : 0,
                                    NombresCompletos= dr["nombres_completos"]?.ToString(),
                                    Rol             = dr["rol"]?.ToString(),
                                    Estado          = dr["estado"]?.ToString(),
                                    IDUSUARIOSUP    = dr["ID_USUARIO_SUP"] != DBNull.Value ? Convert.ToInt32(dr["ID_USUARIO_SUP"]) : null,
                                    IdRol           = dr["id_rol"]         != DBNull.Value ? Convert.ToInt32(dr["id_rol"]) : null,
                                    Dni             = dr["dni"]?.ToString(),
                                    TipoDocumento   = dr["tipo_doc"]?.ToString(),
                                    Telefono        = dr["telefono"]?.ToString(),
                                });
                            }
                        }
                    }
                }

                return lista;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error en ListarAsesores: " + ex.Message);
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
                    using (SqlCommand cmd = new SqlCommand("[SP_USUARIO_LISTAR_USUARIOS]", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                        connection.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                return new Usuario
                                {
                                    IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                    Dni = dr["dni"].ToString(),
                                    TipoDocumento = dr["tipo_documento"].ToString(),
                                    Apellido_Materno = dr["Apellido_Materno"].ToString(),
                                    Apellido_Paterno = dr["Apellido_Paterno"].ToString(),
                                    usuario = dr["usuario"].ToString(),
                                    NombresCompletos = dr["Nombres_Completos"].ToString(),
                                    Nombres = dr["Nombres"].ToString(),
                                    Rol = dr["rol"].ToString(),
                                    Estado = dr["estado"].ToString(),
                                    IdRol = Convert.ToInt32(dr["id_rol"]),
                                    Telefono = dr["telefono"].ToString(),
                                    Correo = dr["correo"].ToString(),
                                    Departamento = dr["departamento"].ToString(),
                                    Provincia = dr["provincia"].ToString(),
                                    Distrito = dr["distrito"].ToString(),
                                    FechaRegistro = Convert.ToDateTime(dr["fecha_registro"]),
                                    NOMBRECAMPANIA = dr["NOMBRE_CAMPAÑA"].ToString(),
                                    RESPONSABLESUP = dr["RESPONSABLE_SUP"].ToString(),
                                    REGION = dr["REGION"].ToString(),
                                    FechaInicio = dr.IsDBNull(dr.GetOrdinal("fecha_inicio")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("fecha_inicio")),
                                    FechaCese = dr.IsDBNull(dr.GetOrdinal("fecha_cese")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("fecha_cese")),
                                    IDUSUARIOSUP = dr.IsDBNull(dr.GetOrdinal("ID_USUARIO_SUP")) ? 0 : dr.GetInt32(dr.GetOrdinal("ID_USUARIO_SUP")),
                                    contraseña = dr["Contrasenia"].ToString(),
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

        public List<object> ExportarUsuariosExcel(string? dni = null, string? usuario = null, int? idRol = null, string? estado = null)
        {
            try
            {
                var lista = new List<object>();
                var cn = new Conexion();

                using (var connection = new SqlConnection(cn.getCadenaSQL()))
                using (var cmd = new SqlCommand("SP_USUARIO_EXPORTAR_EXCEL", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@dni", (object?)dni ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@usuario", (object?)usuario ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_rol", (object?)idRol ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@estado", (object?)estado ?? DBNull.Value);

                    connection.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var item = new
                            {
                                Dni = dr.IsDBNull(dr.GetOrdinal("Dni")) ? null : dr.GetString(dr.GetOrdinal("Dni")),
                                TipoDocumento = dr.IsDBNull(dr.GetOrdinal("tipo_documento")) ? null : dr.GetString(dr.GetOrdinal("tipo_documento")),
                                ApellidoPaterno = dr.IsDBNull(dr.GetOrdinal("Apellido_Paterno")) ? null : dr.GetString(dr.GetOrdinal("Apellido_Paterno")),
                                ApellidoMaterno = dr.IsDBNull(dr.GetOrdinal("Apellido_Materno")) ? null : dr.GetString(dr.GetOrdinal("Apellido_Materno")),
                                Nombres = dr.IsDBNull(dr.GetOrdinal("Nombres")) ? null : dr.GetString(dr.GetOrdinal("Nombres")),
                                Rol = dr.IsDBNull(dr.GetOrdinal("Rol")) ? null : dr.GetString(dr.GetOrdinal("Rol")),
                                Departamento = dr.IsDBNull(dr.GetOrdinal("Departamento")) ? null : dr.GetString(dr.GetOrdinal("Departamento")),
                                Provincia = dr.IsDBNull(dr.GetOrdinal("Provincia")) ? null : dr.GetString(dr.GetOrdinal("Provincia")),
                                Distrito = dr.IsDBNull(dr.GetOrdinal("Distrito")) ? null : dr.GetString(dr.GetOrdinal("Distrito")),
                                Telefono = dr.IsDBNull(dr.GetOrdinal("Telefono")) ? null : dr.GetString(dr.GetOrdinal("Telefono")),
                                FechaRegistro = dr.IsDBNull(dr.GetOrdinal("fecha_registro")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("fecha_registro")),
                                Estado = dr.IsDBNull(dr.GetOrdinal("Estado")) ? null : dr.GetString(dr.GetOrdinal("Estado")),
                                Supervisor = dr.IsDBNull(dr.GetOrdinal("Supervisor")) ? null : dr.GetString(dr.GetOrdinal("Supervisor")),
                                Region = dr.IsDBNull(dr.GetOrdinal("REGION")) ? null : dr.GetString(dr.GetOrdinal("REGION")),
                                NombreCampania = dr.IsDBNull(dr.GetOrdinal("NOMBRE_CAMPANIA")) ? null : dr.GetString(dr.GetOrdinal("NOMBRE_CAMPANIA"))
                            };

                            lista.Add(item);
                        }
                    }
                }

                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ExportarUsuariosExcel: " + ex.Message);
                return new List<object>();
            }
        }
        public async Task<(bool IsSuccess, string Message)> ActualizarCamposUsuario(int idUsuario, string departamento, string provincia, string distrito, string telefono, string correo)
        {
            try
            {
                var cn = new Conexion();
                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_USUARIO_ACTUALIZAR_USUARIO_GENERAL", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                        cmd.Parameters.AddWithValue("@Departamento", (object?)departamento ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Provincia", (object?)provincia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Distrito", (object?)distrito ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Telefono", (object?)telefono ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Correo", (object?)correo ?? DBNull.Value);

                        await connection.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int filasAfectadas = reader.GetInt32(reader.GetOrdinal("FilasAfectadas"));
                                string mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                                return (filasAfectadas > 0 || mensaje == "No se realizaron cambios en los campos", mensaje);
                            }
                            return (false, "Error al procesar la respuesta del servidor");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, "Error al actualizar los campos: " + ex.Message);
            }
        }
    }
}
