using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;
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

        public async Task<DetallesAsignacionesDTO?> GetAsignacion(int idAsignacion)
        {
            try
            {
                var getAsig = await _context.clientes_asignados
                    .AsNoTracking()
                    .Where(x => x.IdAsignacion == idAsignacion)
                    .FirstOrDefaultAsync();
                if (getAsig != null)
                {
                    return new DetallesAsignacionesDTO(getAsig);
                }
                return null;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<BaseCliente?> getBase(int idBase)
        {
            try
            {
                var basecliente = await _context.base_clientes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdBase == idBase);
                return basecliente;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<ClientesEnriquecido?> GetEnriquecido(int idCliente)
        {
            try
            {
                var enriquecido = await _context.clientes_enriquecidos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdCliente == idCliente);
                return enriquecido;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<ClientesEnriquecido?> GetEnriquecidoxBase(int idBase)
        {
            try
            {
                var enriquecido = await _context.clientes_enriquecidos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdBase == idBase);
                return enriquecido;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> UpdateAsignacion(ClientesAsignado asignacion)
        {
            try
            {
                var ClienteAsignado = await _context.clientes_asignados
                    .FirstOrDefaultAsync(x => x.IdAsignacion == asignacion.IdAsignacion);
                if (ClienteAsignado != null)
                {
                    ClienteAsignado.TipificacionMayorPeso = asignacion.TipificacionMayorPeso;
                    ClienteAsignado.PesoTipificacionMayor = asignacion.PesoTipificacionMayor;
                    ClienteAsignado.FechaTipificacionMayorPeso = asignacion.FechaTipificacionMayorPeso;
                    await _context.SaveChangesAsync();
                    return true;
                }
                Console.WriteLine("No se encontró la asignación");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateEnriquecido(ClientesEnriquecido enriquecido)
        {
            try
            {
                var ClienteEnriquecido = await _context.clientes_enriquecidos
                    .FirstOrDefaultAsync(x => x.IdCliente == enriquecido.IdCliente);
                if (ClienteEnriquecido != null)
                {
                    ClienteEnriquecido.ComentarioTelefono1 = enriquecido.ComentarioTelefono1;
                    ClienteEnriquecido.ComentarioTelefono2 = enriquecido.ComentarioTelefono2;
                    ClienteEnriquecido.ComentarioTelefono3 = enriquecido.ComentarioTelefono3;
                    ClienteEnriquecido.ComentarioTelefono4 = enriquecido.ComentarioTelefono4;
                    ClienteEnriquecido.ComentarioTelefono5 = enriquecido.ComentarioTelefono5;
                    ClienteEnriquecido.UltimaTipificacionTelefono1 = enriquecido.UltimaTipificacionTelefono1;
                    ClienteEnriquecido.UltimaTipificacionTelefono2 = enriquecido.UltimaTipificacionTelefono2;
                    ClienteEnriquecido.UltimaTipificacionTelefono3 = enriquecido.UltimaTipificacionTelefono3;
                    ClienteEnriquecido.UltimaTipificacionTelefono4 = enriquecido.UltimaTipificacionTelefono4;
                    ClienteEnriquecido.UltimaTipificacionTelefono5 = enriquecido.UltimaTipificacionTelefono5;
                    ClienteEnriquecido.FechaUltimaTipificacionTelefono1 = enriquecido.FechaUltimaTipificacionTelefono1;
                    ClienteEnriquecido.FechaUltimaTipificacionTelefono2 = enriquecido.FechaUltimaTipificacionTelefono2;
                    ClienteEnriquecido.FechaUltimaTipificacionTelefono3 = enriquecido.FechaUltimaTipificacionTelefono3;
                    ClienteEnriquecido.FechaUltimaTipificacionTelefono4 = enriquecido.FechaUltimaTipificacionTelefono4;
                    ClienteEnriquecido.FechaUltimaTipificacionTelefono5 = enriquecido.FechaUltimaTipificacionTelefono5;
                    ClienteEnriquecido.IdClientetipTelefono1 = enriquecido.IdClientetipTelefono1;
                    ClienteEnriquecido.IdClientetipTelefono2 = enriquecido.IdClientetipTelefono2;
                    ClienteEnriquecido.IdClientetipTelefono3 = enriquecido.IdClientetipTelefono3;
                    ClienteEnriquecido.IdClientetipTelefono4 = enriquecido.IdClientetipTelefono4;
                    ClienteEnriquecido.IdClientetipTelefono5 = enriquecido.IdClientetipTelefono5;
                    await _context.SaveChangesAsync();
                    return true;
                }
                Console.WriteLine("No se encontró el cliente enriquecido");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}