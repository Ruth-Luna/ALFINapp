using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Services
{

    public class DBServicesRoles
    {
        private readonly MDbContext _context;
        public DBServicesRoles(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message, List<VistasPorRolDTO>? Data)> getVistasPorRol(int idRol)
        {
            try
            {
                var vistas = await Task.Run(() => _context.vistas_por_rol_dto.FromSqlRaw("EXEC sp_Roles_get_vistas {0}", idRol).ToList());
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
        public async Task<(bool IsSuccess, string Message, VistasPorRolDTO? Data)> getVistaPorDefecto(int idRol)
        {
            try
            {
                var vista = await _context.vistas_por_rol_dto.FromSqlRaw("EXEC sp_Roles_Vista_por_defecto {0}", idRol).FirstOrDefaultAsync();
                if (vista != null)
                {
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

        public async Task<(bool IsSuccess, string Message, bool? Data)> tienePermiso(int idRol, string vista, string ruta)
        {
            try
            {
                var permiso = await _context.numeros_enteros_dto.FromSqlRaw("EXEC sp_Roles_tiene_permisos {0}, {1}, {2}", idRol, vista, ruta).ToListAsync();
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
    }
}