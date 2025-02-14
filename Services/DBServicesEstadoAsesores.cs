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
        public async Task<(bool IsSuccess, string message)> ActivarAsesor(Usuario asesor, int idAccion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", asesor.IdUsuario),
                    new SqlParameter("@id_usuario_accion", idAccion)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_activar @IdUsuario, @id_usuario_accion", 
                    parameters);
                return (true, "Asesor activado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string message)> DesactivarAsesor(Usuario asesor, int idAccion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", asesor.IdUsuario),
                    new SqlParameter("@id_usuario_accion", idAccion)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_desactivar @IdUsuario, @id_usuario_accion", 
                    parameters);
                return (true, "Asesor desactivado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}