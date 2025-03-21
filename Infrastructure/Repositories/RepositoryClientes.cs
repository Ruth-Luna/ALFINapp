using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryClientes : IRepositoryClientes
    {
        private readonly MDbContext _context;
        public RepositoryClientes(MDbContext context)
        {
            _context = context;
        }
        public async Task<List<DetallesAsignacionesDTO>?> GetAllAsignaciones(int idCliente)
        {
            try
            {
                return null;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<DetallesAsignacionesDTO>?> GetAllAsignaciones()
        {
            try
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                var firstDay = new DateTime(year, month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                var getAsignaciones = await _context.clientes_asignados
                    .AsNoTracking()
                    .Where(x => x.FechaAsignacionSup >= firstDay 
                        && x.FechaAsignacionSup <= lastDay
                        && x.IdUsuarioV != null)
                    .ToListAsync();
                var asignaciones = new List<DetallesAsignacionesDTO>();
                foreach (var asignacion in getAsignaciones)
                {
                    asignaciones.Add(new DetallesAsignacionesDTO(asignacion));
                }
                return asignaciones;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<DetallesAsignacionesDTO>?> GetAllAsignacionesTrabajadas()
        {
            try
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                var firstDay = new DateTime(year, month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                var getAsig = await _context.clientes_asignados
                    .AsNoTracking()
                    .Where(x => x.PesoTipificacionMayor != null
                        && x.FechaAsignacionSup >= firstDay 
                        && x.FechaAsignacionSup <= lastDay)
                    .ToListAsync();
                var asignaciones = new List<DetallesAsignacionesDTO>();
                foreach (var asignacion in getAsig)
                {
                    asignaciones.Add(new DetallesAsignacionesDTO(asignacion));
                }
                return asignaciones;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}