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

        public async Task<List<DetallesAsignacionesDTO>> GetAllAsignacionesFromDestino(int idUsuarioS, string destino)
        {
            try
            {
                var actualdate = DateTime.Now;
                var year = actualdate.Year;
                var month = actualdate.Month;
                var allclientes = await (
                    from ca in _context.clientes_asignados
                    join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                    join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                    where ca.IdUsuarioS == idUsuarioS
                        && ca.IdUsuarioV == null
                        && ca.Destino == destino
                        && ca.FechaAsignacionSup.HasValue
                        && ca.FechaAsignacionSup.Value.Year == year
                        && ca.FechaAsignacionSup.Value.Month == month
                    select new 
                    {
                        IdAsignacion = ca.IdAsignacion,
                        IdCliente = ca.IdCliente,
                        IdUsuarioS = ca.IdUsuarioS,
                        FechaAsignacionSup = ca.FechaAsignacionSup,
                        FechaAsignacionVendedor = ca.FechaAsignacionVendedor,
                        Destino = ca.Destino,
                        Dni = bc.Dni
                    }
                ).ToListAsync();

                if (allclientes == null || allclientes.Count == 0)
                {
                    Console.WriteLine("No hay clientes asignados al supervisor.");
                    return new List<DetallesAsignacionesDTO>();
                }

                var getDnis = allclientes
                    .Select(cliente => cliente.Dni)
                    .Distinct()
                    .ToHashSet();
                
                var getDesembolsos = await _context.desembolsos
                    .AsNoTracking()
                    .Where(x => getDnis.Contains(x.DniDesembolso ?? "") 
                        && x.FechaDesembolsos != null
                        && x.FechaDesembolsos.Value.Year == year
                        && x.FechaDesembolsos.Value.Month == month)
                    .Select(x => new
                    {
                        x.DniDesembolso
                    })
                    .ToHashSetAsync();
                    
                var getRetiros = await _context.retiros
                    .AsNoTracking()
                    .Where(x => getDnis.Contains(x.DniRetiros ?? "")
                        && x.FechaRetiro != null
                        && x.FechaRetiro.Value.Year == year
                        && x.FechaRetiro.Value.Month == month)
                    .Select(x => new
                    {
                        x.DniRetiros
                    })
                    .ToHashSetAsync();

                var getAsignacionesFinal = allclientes
                    .Where(cliente => !getDesembolsos.Any(d => d.DniDesembolso == cliente.Dni) && !getRetiros.Any(r => r.DniRetiros == cliente.Dni))
                    .ToList();
                var clientesDisponibles = getAsignacionesFinal
                    .Select(cliente => new ClientesAsignado
                    {
                        IdAsignacion = cliente.IdAsignacion,
                        IdCliente = cliente.IdCliente,
                        IdUsuarioS = cliente.IdUsuarioS,
                        FechaAsignacionSup = cliente.FechaAsignacionSup,
                        FechaAsignacionVendedor = cliente.FechaAsignacionVendedor,
                        Destino = cliente.Destino
                    })
                    .ToList();
                
                if (allclientes == null || allclientes.Count == 0)
                {
                    Console.WriteLine("No hay clientes asignados al supervisor.");
                    return new List<DetallesAsignacionesDTO>();
                }
                var detallesAsignaciones = clientesDisponibles
                    .Select(cliente => new DetallesAsignacionesDTO(cliente))
                    .ToList();
                
                return detallesAsignaciones;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<DetallesAsignacionesDTO>();
            }
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

        public async Task<List<DetalleBaseClienteDTO>> GetClientesGeneralPaginadoFromSupervisor(int idUsuario)
        {
            try
            {
                var hoy = DateTime.Now;
                var añoActual = hoy.Year;
                var mesActual = hoy.Month;

                var supervisorData = await _context.supervisor_get_asignacion_leads.FromSqlRaw(
                    "EXEC dbo.sp_supervisor_get_asignacion_de_leads @IdUsuario = {0}", new SqlParameter("@IdUsuario", idUsuario))
                    .ToListAsync();
                if (!supervisorData.Any())
                {
                    Console.WriteLine("No hay clientes asignados al supervisor.");
                    return new List<DetalleBaseClienteDTO>();
                }
                var detallesClientes = supervisorData.Select(cliente => new ALFINapp.Application.DTOs.DetalleBaseClienteDTO(cliente)).ToList();
                return detallesClientes;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<DetalleBaseClienteDTO>();
            }
        }

        public async Task<(int total, int totalAsignados, int totalPendientes)> GetCantidadClientesGeneralTotalFromSupervisor(int idUsuario)
        {
            try
            {
                var result = await _context.supervisor_get_number_of_leads
                    .FromSqlRaw("EXEC sp_supervisor_get_number_of_leads @IdUsuario", new SqlParameter("@IdUsuario", idUsuario))
                    .ToListAsync();
                if (result == null || !result.Any())
                {
                    Console.WriteLine("No se encontraron datos de clientes.");
                    return (0, 0, 0);
                }
                var resultado = result.FirstOrDefault();
                if (resultado == null)
                {
                    Console.WriteLine("No se encontraron datos de clientes.");
                    return (0, 0, 0);
                }
                return (resultado.TotalClientes, resultado.TotalClientesAsignados, resultado.TotalClientesPendientes);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return (0, 0, 0);
            }
        }
    }
}