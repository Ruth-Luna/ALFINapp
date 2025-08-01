using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Services
{
    /// <summary>
    /// Service class that provides database operations for user management in the ALFINapp system.
    /// Uses stored procedures to interact with the database for CRUD operations on users.
    /// </summary>
    public class DBServicesUsuarios
    {
        private readonly MDbContext _context;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DBServicesUsuarios"/> class.
        /// </summary>
        /// <param name="context">The database context used for database operations.</param>
        public DBServicesUsuarios(MDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing user's information in the database.
        /// </summary>
        /// <param name="usuario">The user object containing the updated information.</param>
        /// <param name="IdUsuarioAccion">ID of the user performing the modification action.</param>
        /// <returns>A tuple containing success status and a message describing the result.</returns>
        public async Task<(bool IsSuccess, string Message)> ModificarUsuario(Usuario usuario, int IdUsuarioAccion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", usuario.IdUsuario),
                    new SqlParameter("@Dni", usuario.Dni != null ? usuario.Dni : (object)DBNull.Value),
                    new SqlParameter("@NombresCompletos", usuario.NombresCompletos != null ? usuario.NombresCompletos : (object)DBNull.Value),
                    new SqlParameter("@Rol", usuario.Rol != null ? usuario.Rol : (object)DBNull.Value),
                    new SqlParameter("@Departamento", usuario.Departamento != null ? usuario.Departamento : (object)DBNull.Value),
                    new SqlParameter("@Provincia", usuario.Provincia != null ? usuario.Provincia : (object)DBNull.Value),
                    new SqlParameter("@Distrito", usuario.Distrito != null ? usuario.Distrito : (object)DBNull.Value),
                    new SqlParameter("@Telefono", usuario.Telefono != null ? usuario.Telefono : (object)DBNull.Value),
                    new SqlParameter("@Estado", usuario.Estado != null ? usuario.Estado : "ACTIVO"),
                    new SqlParameter("@IDUSUARIOSUP", usuario.IDUSUARIOSUP != null ? usuario.IDUSUARIOSUP : (object)DBNull.Value),
                    new SqlParameter("@RESPONSABLESUP", usuario.RESPONSABLESUP != null ? usuario.RESPONSABLESUP : (object)DBNull.Value),
                    new SqlParameter("@REGION", usuario.REGION != null ? usuario.REGION : (object)DBNull.Value),
                    new SqlParameter("@NOMBRECAMPAÑA", usuario.NOMBRECAMPANIA != null ? usuario.NOMBRECAMPANIA : (object)DBNull.Value),
                    new SqlParameter("@IdRol", usuario.IdRol != null ? usuario.IdRol : (object)DBNull.Value),
                    new SqlParameter("@id_usuario_accion", IdUsuarioAccion),
                    new SqlParameter("@FechaInicio", usuario.FechaInicio != null ? usuario.FechaInicio : (object)DBNull.Value),
                    new SqlParameter("@FechaCese", usuario.FechaCese != null ? usuario.FechaCese : (object)DBNull.Value),
                    new SqlParameter("@Correo", usuario.Correo != null ? usuario.Correo : (object)DBNull.Value)
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_modificacion_existente @IdUsuario, @Dni, @NombresCompletos, @Rol, @Departamento, @Provincia, @Distrito, @Telefono, @Estado, @IDUSUARIOSUP, @RESPONSABLESUP, @REGION, @NOMBRECAMPAÑA, @IdRol, @id_usuario_accion, @FechaInicio, @FechaCese, @Correo", 
                    parameters);

                return (true, "Datos actualizados correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deactivates a user in the system by changing their status.
        /// </summary>
        /// <param name="usuarioId">ID of the user to deactivate.</param>
        /// <param name="idAccion">ID of the user performing the deactivation action.</param>
        /// <returns>A tuple containing success status and a message describing the result.</returns>
        public async Task<(bool IsSuccess, string Message)> DesactivarUsuario(int usuarioId, int idAccion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", usuarioId),
                    new SqlParameter("@id_usuario_accion", idAccion)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_desactivar @IdUsuario, @id_usuario_accion", parameters);
                return (true, "Usuario desactivado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Activates a previously deactivated user in the system.
        /// </summary>
        /// <param name="usuarioId">ID of the user to activate.</param>
        /// <param name="idAccion">ID of the user performing the activation action.</param>
        /// <returns>A tuple containing success status and a message describing the result.</returns>
        public async Task<(bool IsSuccess, string Message)> ActivarUsuario(int usuarioId, int idAccion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", usuarioId),
                    new SqlParameter("@id_usuario_accion", idAccion)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_activar @IdUsuario, @id_usuario_accion", parameters);
                return (true, "Usuario activado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Creates a new user in the system with validation checks.
        /// </summary>
        /// <remarks>
        /// Validates:
        /// - Required role
        /// - DNI format (8-9 digits)
        /// - DNI uniqueness
        /// - Required name
        /// - Supervisor existence if specified
        /// </remarks>
        /// <param name="usuario">User object containing the information for the new user.</param>
        /// <param name="IdUsuarioAccion">ID of the user performing the creation action.</param>
        /// <returns>A tuple containing success status and a message describing the result.</returns>
        public async Task<(bool IsSuccess, string Message)> CrearUsuario(Usuario usuario, int IdUsuarioAccion)
        {
            try
            {
                if (usuario.IdRol == null)
                {
                    return (false, "El rol del usuario es obligatorio");
                }
                if (string.IsNullOrEmpty(usuario.Dni) || (usuario.Dni.Length != 8 && usuario.Dni.Length != 9) || !usuario.Dni.All(char.IsDigit))
                {
                    return (false, "El DNI o documento de Identidad del usuario es obligatorio y debe tener 8 o 9 dígitos");
                }
                var dniExiste = await _context.usuarios.AnyAsync(x => x.Dni == usuario.Dni);
                if (dniExiste)
                {
                    return (false, "El DNI ingresado ya existe en la base de datos");
                }

                if (usuario.NombresCompletos == null)
                {
                    return (false, "El nombre del usuario es obligatorio");
                }
                var getUsuario = new Usuario();
                if (usuario.IDUSUARIOSUP != null)
                {
                    getUsuario = await _context.usuarios.FirstOrDefaultAsync(x => x.IdUsuario == usuario.IDUSUARIOSUP);
                    if (getUsuario == null)
                    {
                        return (false, "El usuario supervisor no existe o no se ha encontrado");
                    }
                }
                var rolConseguir = await _context.roles.FirstOrDefaultAsync(x => x.IdRol == usuario.IdRol);
                
                var parameters = new[]
                {
                    new SqlParameter("@Dni", usuario.Dni),
                    new SqlParameter("@NombresCompletos", usuario.NombresCompletos),
                    new SqlParameter("@Rol", rolConseguir != null ? rolConseguir.Rol : "ASESOR"),
                    new SqlParameter("@Departamento", usuario.Departamento != null ? usuario.Departamento : (object)DBNull.Value),
                    new SqlParameter("@Provincia", usuario.Provincia != null ? usuario.Provincia : (object)DBNull.Value),
                    new SqlParameter("@Distrito", usuario.Distrito != null ? usuario.Distrito : (object)DBNull.Value),
                    new SqlParameter("@Telefono", usuario.Telefono != null ? usuario.Telefono : (object)DBNull.Value),
                    new SqlParameter("@Estado", usuario.Estado != null ? usuario.Estado : "ACTIVO"),
                    new SqlParameter("@IDUSUARIOSUP", usuario.IDUSUARIOSUP != null ? usuario.IDUSUARIOSUP : (object)DBNull.Value),
                    new SqlParameter("@RESPONSABLESUP", getUsuario?.NombresCompletos ?? (object)DBNull.Value),
                    new SqlParameter("@REGION", usuario.REGION != null ? usuario.REGION : (object)DBNull.Value),
                    new SqlParameter("@NOMBRECAMPAÑA", usuario.NOMBRECAMPANIA != null ? usuario.NOMBRECAMPANIA : (object)DBNull.Value),
                    new SqlParameter("@IdRol", usuario.IdRol),
                    new SqlParameter("@id_usuario_accion", IdUsuarioAccion),
                    new SqlParameter("@tipo_documento", usuario.TipoDocumento != null ? usuario.TipoDocumento : (object)DBNull.Value),
                    new SqlParameter("@Correo", usuario.Correo != null ? usuario.Correo : (object)DBNull.Value)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_crear_nuevo @Dni, @NombresCompletos, @Rol, @Departamento, @Provincia, @Distrito, @Telefono, @Estado, @IDUSUARIOSUP, @RESPONSABLESUP, @REGION, @NOMBRECAMPAÑA, @IdRol, @id_usuario_accion, @Correo",
                    parameters);
                return (true, "Usuario creado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Updates a specific field for a user in the database.
        /// </summary>
        /// <param name="usuarioId">ID of the user to update.</param>
        /// <param name="campo">Name of the field/column to update.</param>
        /// <param name="nuevoValor">New value to set for the specified field.</param>
        /// <returns>A tuple containing success status and a message describing the result.</returns>
        public async Task<(bool IsSuccess, string Message)> UpdateUsuarioXCampo(int usuarioId, string campo, string nuevoValor)
        {
            try
            {
                var usuario = await _context.usuarios.FirstOrDefaultAsync(x => x.IdUsuario == usuarioId);
                if (usuario == null)
                {
                    return (false, "El usuario no existe");
                }
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", usuarioId),
                    new SqlParameter("@Campo", campo),
                    new SqlParameter("@NuevoValor", nuevoValor ?? (object)DBNull.Value) // Si es null, lo manda como NULL a SQL
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_actualizar_campo @IdUsuario, @Campo, @NuevoValor", parameters);
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
        public async Task<(bool IsSuccess, string Message, AsesoresOcultos? Data)> GetUsuarioOculto(string dni)
        {
            try
            {
                var usuario = await _context.Asesores_Ocultos.FirstOrDefaultAsync(x => x.DniVicidial == dni);
                if (usuario == null)
                {
                    return (true, "Usuario no encontrado", null);
                }
                return (true, "Usuario encontrado", usuario);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}