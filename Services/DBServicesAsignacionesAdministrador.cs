using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ALFINapp.Services
{
    public class DBServicesAsignacionesAdministrador
    {
        private readonly MDbContext _context;
        public DBServicesAsignacionesAdministrador(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message)> InsertarAsignacionASupervisores(
            int? idCliente, int? idSupervisor, string? fuenteBase, string? destino)
        {
            try
            {
                var insertarAsignacion = await _context.Database
                    .ExecuteSqlRawAsync("EXEC SP_INSERTAR_ASIGNACION @id_cliente, @id_usuarioS, @fuente_base, @destino",
                        new SqlParameter("@id_cliente", idCliente ?? (object)DBNull.Value),
                        new SqlParameter("@id_usuarioS", idSupervisor ?? (object)DBNull.Value),
                        new SqlParameter("@fuente_base", fuenteBase ?? (object)DBNull.Value),
                        new SqlParameter("@destino", destino ?? (object)DBNull.Value));
                if (insertarAsignacion <= 0)
                {
                    return (false, "La Asignacion al Filtrar Bases no se ha insertado");
                }
                return (true, "La Asignacion al Filtrar Bases se ha ejecutado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}