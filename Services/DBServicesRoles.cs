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
    }
}