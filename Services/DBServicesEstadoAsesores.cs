using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Services
{
    public class DBServicesEstadoAsesores
    {
        private readonly MDbContext _context;
        public DBServicesEstadoAsesores(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, string message)> ActivarAsesor(Usuario asesor)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_activar @IdUsuario", 
                    new SqlParameter("@IdUsuario", asesor.IdUsuario));
                return (true, "Asesor activado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string message)> DesactivarAsesor(Usuario asesor)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_desactivar @IdUsuario",
                    new SqlParameter("@IdUsuario", asesor.IdUsuario));
                return (true, "Asesor desactivado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}