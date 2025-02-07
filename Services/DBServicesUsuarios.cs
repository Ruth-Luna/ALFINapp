using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Services
{
    public class DBServicesUsuarios
    {
        private readonly MDbContext _context;
        public DBServicesUsuarios(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message)> ModificarUsuario(Usuario usuario)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", usuario.IdUsuario),
                    new SqlParameter("@Dni", usuario.Dni),
                    new SqlParameter("@NombresCompletos", usuario.NombresCompletos),
                    new SqlParameter("@Rol", usuario.Rol),
                    new SqlParameter("@Departamento", usuario.Departamento),
                    new SqlParameter("@Provincia", usuario.Provincia),
                    new SqlParameter("@Distrito", usuario.Distrito),
                    new SqlParameter("@Telefono", usuario.Telefono),
                    new SqlParameter("@FechaRegistro", usuario.FechaRegistro),
                    new SqlParameter("@Estado", usuario.Estado),
                    new SqlParameter("@IDUSUARIOSUP", usuario.IDUSUARIOSUP),
                    new SqlParameter("@RESPONSABLESUP", usuario.RESPONSABLESUP),
                    new SqlParameter("@REGION", usuario.REGION),
                    new SqlParameter("@NOMBRECAMPAÑA", usuario.NOMBRECAMPAÑA),
                    new SqlParameter("@IdRol", usuario.IdRol)
                };
 
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_modificacion_existente @IdUsuario, @Dni, @NombresCompletos, @Rol, @Departamento, @Provincia, @Distrito, @Telefono, @FechaRegistro, @Estado, @IDUSUARIOSUP, @RESPONSABLESUP, @REGION, @NOMBRECAMPAÑA, @IdRol", parameters);

                return (true, "Datos actualizados correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

    }
}