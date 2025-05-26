using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Services
{
    /// <summary>
    /// Service class that provides database operations for role management and permissions in the ALFINapp system.
    /// Handles retrieving and managing views, permissions, and roles through database operations.
    /// </summary>
    public class DBServicesRoles
    {
        private readonly MDbContext _context;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DBServicesRoles"/> class.
        /// </summary>
        /// <param name="context">The database context used for database operations.</param>
        public DBServicesRoles(MDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all views associated with a specific role.
        /// </summary>
        /// <param name="idRol">ID of the role to query views for.</param>
        /// <returns>
        /// A tuple containing:
        /// - Success status indicating if the operation was successful
        /// - Message describing the result or error
        /// - List of views associated with the role if successful, otherwise null
        /// </returns>
        public async Task<(bool IsSuccess, string Message, List<VistasPorRolDTO>? Data)> getVistasPorRol(int idRol)
        {
            try
            {
                var vistas = await Task.Run(() => _context.vistas_por_rol_dto.FromSqlRaw("EXEC sp_Roles_get_vistas {0}", idRol)
                    .ToList());
                if (vistas.Count > 0)
                {
                    return (true, "Vistas encontradas", vistas);
                }
                else
                {
                    return (false, "No se encontraron vistas", null);
                }
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }

        }
        /// <summary>
        /// Retrieves the default view for a specific role.
        /// </summary>
        /// <param name="idRol">ID of the role to query the default view for.</param>
        /// <returns>
        /// A tuple containing:
        /// - Success status indicating if the operation was successful
        /// - Message describing the result or error
        /// - Default view associated with the role if successful, otherwise null
        /// </returns>
        public async Task<(bool IsSuccess, string Message, VistasPorRolDTO? Data)> getVistaPorDefecto(int idRol)
        {
            try
            {
                var GetVista = await _context.vistas_por_rol_dto.FromSqlRaw("EXEC sp_Roles_Vista_por_defecto_test @idRol", new SqlParameter("@idRol", idRol)).ToListAsync();

                if (GetVista.Count() != 0)
                {
                    var vista = GetVista.FirstOrDefault();
                    if (vista == null)
                    {
                        return (false, "No se encontró la vista correspondiente", null);
                    }
                    return (true, "Vista encontrada", vista);
                }
                else
                {
                    return (false, "No se encontró vista", null);
                }
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        /// <summary>
        /// Checks if a role has permission to access a specific view.
        /// </summary>
        /// <param name="idRol">ID of the role to check permissions for.</param>
        /// <param name="controlador">Controller name to check permission against.</param>
        /// <param name="vista">View name to check permission against.</param>
        /// <returns>
        /// A tuple containing:
        /// - Success status indicating if the operation was successful
        /// - Message describing the result or error
        /// - Boolean indicating if the role has permission if successful, otherwise null
        /// </returns>
        public async Task<(bool IsSuccess, string Message, bool? Data)> tienePermiso(int idRol, string controlador, string vista)
        {
            try
            {
                var permiso = await _context.numeros_enteros_dto.FromSqlRaw("EXEC sp_Roles_tiene_permisos {0}, {1}, {2}", idRol, controlador, vista).ToListAsync();
                if (permiso.Count > 0)
                {
                    return (true, "Permiso encontrado", true);
                }
                else
                {
                    return (true, "No se encontró permiso", false);
                }
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        /// <summary>
        /// Retrieves all available view routes in the system.
        /// </summary>
        /// <returns>
        /// A tuple containing:
        /// - Success status indicating if the operation was successful
        /// - Message describing the result or error
        /// - List of all view routes if successful, otherwise null
        /// </returns>
        public async Task<(bool IsSuccess, string Message, List<VistaRutas>? Data)> getTodasLasVistasRutas()
        {
            try
            {
                var vistas = await (from v in _context.Vista_Rutas
                                    select v).ToListAsync();
                if (vistas.Count > 0)
                {
                    return (true, "Vistas encontradas", vistas);
                }
                return (false, "No se encontraron vistas", null);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        /// <summary>
        /// Retrieves all roles in the system.
        /// </summary>
        /// <returns>
        /// A tuple containing:
        /// - Success status indicating if the operation was successful
        /// - Message describing the result or error
        /// - List of all roles if successful, otherwise null
        /// </returns>
        public async Task<(bool IsSuccess, string Message, List<Roles>? Data)> getRoles()
        {
            try
            {
                var roles = await (from r in _context.roles
                                   select r).ToListAsync();
                if (roles.Count > 0)
                {
                    return (true, "Roles encontrados", roles);
                }
                return (false, "No se encontraron roles", null);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        /// <summary>
        /// Retrieves all roles in the system (alternative implementation).
        /// </summary>
        /// <returns>
        /// A tuple containing:
        /// - Success status indicating if the operation was successful
        /// - Message describing the result or error
        /// - List of all roles if successful, otherwise null
        /// </returns>
        public async Task<(bool IsSuccess, string Message, List<Roles>? Data)> getAllRoles()
        {
            try
            {
                var roles = await (from r in _context.roles
                                   select r).ToListAsync();
                if (roles.Count > 0)
                {
                    return (true, "Roles encontrados", roles);
                }
                return (false, "No se encontraron roles", null);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        /// <summary>
        /// Updates permission for a role to access a specific view.
        /// If the permission exists, it will be removed; if it doesn't exist, it will be added.
        /// </summary>
        /// <param name="idVista">ID of the view to update permission for.</param>
        /// <param name="idRol">ID of the role to update permission for.</param>
        /// <returns>
        /// A tuple containing:
        /// - Success status indicating if the operation was successful
        /// - Message describing the result or error
        /// </returns>
        public async Task<(bool IsSuccess, string Message)> actualizarPermisoRol(int idVista, int idRol)
        {
            try
            {
                var rolesExistentes = await (from p in _context.roles
                                            select p).ToListAsync();
                if (!rolesExistentes.Any(r => r.IdRol == idRol))
                {
                    return (false, "El rol especificado no existe");
                }
                var vistasExistentes = await (from v in _context.Vista_Rutas
                                             select v).ToListAsync();
                if (!vistasExistentes.Any(v => v.IdVista == idVista))
                {
                    return (false, "La vista especificada no existe");
                }
                var permiso = await (
                    from p in _context.Permisos_Roles_Vistas
                    where p.IdRol == idRol && p.IdVista == idVista
                    select p
                ).FirstOrDefaultAsync();
                if (permiso == null)
                {
                    var nuevoPermiso = new PermisosRolesVistas
                    {
                        IdRol = idRol,
                        IdVista = idVista
                    };
                    _context.Permisos_Roles_Vistas.Add(nuevoPermiso);
                    await _context.SaveChangesAsync();
                    return (true, "Permiso agregado correctamente");
                }
                else
                {
                    _context.Permisos_Roles_Vistas.Remove(permiso);
                    await _context.SaveChangesAsync();
                    return (true, "Permiso removido correctamente");
                }
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}