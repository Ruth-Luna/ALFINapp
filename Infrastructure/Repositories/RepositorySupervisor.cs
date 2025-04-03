using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositorySupervisor : IRepositorySupervisor
    {
        private readonly MDbContext _context;
        public RepositorySupervisor(MDbContext context)
        {
            _context = context;
        }

        public async Task<DetallesAsignacionContadorFromVendedorDTO> GetContadorAllAsignacionesFromVendedor(List<int> IdsUsuariosVendedores, int idUsuarioS)
        {
            try
            {
                var allclientes = await _context.clientes_asignados
                    .AsNoTracking()
                    .Where(x => x.IdUsuarioS == idUsuarioS
                        && x.FechaAsignacionSup.HasValue
                        && x.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                        && x.FechaAsignacionSup.Value.Month == DateTime.Now.Month)
                    .Select(
                        x => new ClientesAsignado
                        {
                            IdCliente = x.IdCliente,
                            IdUsuarioV = x.IdUsuarioV,
                            IdUsuarioS = x.IdUsuarioS,
                            FechaAsignacionSup = x.FechaAsignacionSup,
                            PesoTipificacionMayor = x.PesoTipificacionMayor,
                            Destino = x.Destino
                        })
                    .ToListAsync();

                var numeroClientesFromVendedor = _context.usuarios
                    .AsNoTracking()
                    .Where(x => IdsUsuariosVendedores.Contains(x.IdUsuario))
                    .AsEnumerable()
                    .Select(x => new DetalleAsignacionContadorFromVendedorDTO
                    {
                        NombresCompletos = x.NombresCompletos,
                        IdUsuario = x.IdUsuario,
                        NumeroClientes = allclientes.Count(c => c.IdUsuarioV == x.IdUsuario),
                        NumeroClientesGestionados = allclientes.Count(c => c.IdUsuarioV == x.IdUsuario && c.PesoTipificacionMayor != null),
                        NumeroClientesPendientes = allclientes.Count(c => c.IdUsuarioV == x.IdUsuario && c.PesoTipificacionMayor == null),
                        estaActivado = x.Estado == "ACTIVO" ? true : false
                    })
                    .ToList();
                if (numeroClientesFromVendedor == null || numeroClientesFromVendedor.Count == 0)
                {
                    Console.WriteLine("No hay clientes asignados al vendedor.");
                    return new DetallesAsignacionContadorFromVendedorDTO();
                }
                var getDestinos = allclientes
                    .Select(x => x.Destino)
                    .Distinct()
                    .ToList();

                var detallesAsignacionContadorFromVendedor = new DetallesAsignacionContadorFromVendedorDTO
                {
                    DetallesAsignacionContadorFromVendedor = numeroClientesFromVendedor,
                    DetallesClientesAsignados = allclientes.Select(cliente => new ALFINapp.Application.DTOs.DetalleBaseClienteDTO(cliente)).ToList(),
                    Destinos = getDestinos.Where(destino => destino != null).Cast<string>().ToList()
                };
                return detallesAsignacionContadorFromVendedor;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new DetallesAsignacionContadorFromVendedorDTO();
            }
        }

        public async Task<DetallesInicioSupervisorDTO> GetInicioSupervisor(int idUsuario)
        {
            try
            {
                var hoy = DateTime.Now;
                var añoActual = hoy.Year;
                var mesActual = hoy.Month;

                var supervisorData = await _context.supervisor_get_inicio_data.FromSqlRaw(
                    "EXEC dbo.sp_supervisor_get_inicio_data @IdUsuario = {0}", new SqlParameter("@IdUsuario", idUsuario))
                    .ToListAsync();

                if (!supervisorData.Any())
                {
                    Console.WriteLine("No hay clientes asignados al supervisor.");
                    return new DetallesInicioSupervisorDTO
                    {
                        DetallesClientes = new List<ALFINapp.Application.DTOs.DetallesClienteDTO>()
                    };
                }

                var detallesClientes = supervisorData.Select(cliente => new ALFINapp.Application.DTOs.DetallesClienteDTO(cliente)).ToList();
                var detallesClientesl = new DetallesInicioSupervisorDTO
                {
                    DetallesClientes = detallesClientes
                };
                return detallesClientesl;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new DetallesInicioSupervisorDTO
                {
                    DetallesClientes = new List<ALFINapp.Application.DTOs.DetallesClienteDTO>()
                };
            }
        }
    }
}