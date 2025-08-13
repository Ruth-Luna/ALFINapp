using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryTipificaciones : IRepositoryTipificaciones
    {
        MDbContext _context;
        public RepositoryTipificaciones(MDbContext context)
        {
            _context = context;
        }
        public async Task<List<Tipificaciones>> GetTipificacionesDescripcion()
        {
            try
            {
                var tipificaciones = await _context.tipificaciones.ToListAsync();
                return tipificaciones;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Tipificaciones>();
            }
        }
    }
}