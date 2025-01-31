using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Services
{
    public class DBServicesConsultasAdministrador
    {
        private readonly MDbContext _context;
        public DBServicesConsultasAdministrador(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message, List<AsignacionFiltrarBasesDTO>? Data)> AsignacionFiltrarBases(
            string? base_busqueda, string? campaña, decimal? oferta)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@base", base_busqueda ?? (object)DBNull.Value),
                    new SqlParameter("@campaña", campaña ?? (object)DBNull.Value),
                    new SqlParameter("@oferta", oferta ?? (object)DBNull.Value)
                };

                var asignacionFiltrarBases = await _context.asignacion_filtrar_bases_dto
                    .FromSqlRaw("EXEC sp_Asignacion_FiltrarBases @base, @campaña, @oferta", parameters)
                    .ToListAsync();

                if (asignacionFiltrarBases == null)
                {
                    return (false, "La Asignacion al Filtrar Bases no se ha encontrado", null);
                }

                return (true, "Asignacion Filtrar Bases ha devuelto una tabla", asignacionFiltrarBases);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}