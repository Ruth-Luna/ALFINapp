using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryDerivaciones : IRepositoryDerivaciones
    {
        MDbContext _context;
        public RepositoryDerivaciones(MDbContext context)
        {
            _context = context;
        }
        public async Task<List<DerivacionesAsesores>?> getDerivaciones(int idCliente, string docAsesor)
        {
            try
            {
                var derivacion = await _context
                    .derivaciones_asesores
                    .AsNoTracking()
                    .Where(x => x.IdCliente == idCliente 
                        && x.DniAsesor == docAsesor
                        && x.FechaDerivacion.Year == DateTime.Now.Year
                        && x.FechaDerivacion.Month == DateTime.Now.Month)
                    .ToListAsync();
                if (derivacion != null)
                {
                    return derivacion;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<GESTIONDETALLE?> getGestionDerivacion(string docCliente, string docAsesor)
        {
            try
            {
                var gestion = await _context
                    .GESTION_DETALLE
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.DocCliente == docCliente 
                        && x.DocAsesor == docAsesor
                        && x.FechaGestion.Year == DateTime.Now.Year
                        && x.FechaGestion.Month == DateTime.Now.Month);
                if (gestion != null)
                {
                    return gestion;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}