using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ALFINapp.Infrastructure.Services
{
    public class DBServicesTipificaciones
    {
        private readonly MDbContext _context;

        public DBServicesTipificaciones(MDbContext context)
        {
            _context = context;
        }
        
        public async Task<(bool IsSuccess, string Message, List<Tipificaciones>? Data)> ObtenerTipificaciones()
        {
            try
            {
                var tipificaciones = await _context.tipificaciones.ToListAsync();
                if (tipificaciones == null)
                {
                    return (false, "No se pudo encontrar tipificaciones en la base de datos", null);
                }
                return (true, "Se han encontrado las tipificaciones en la base de datos", tipificaciones);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        // Other DB services can be added here
    }
}