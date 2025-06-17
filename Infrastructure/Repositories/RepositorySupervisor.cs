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

        public async Task<List<DetallesAsignacionesDTO>> GetAllAsignacionesFromDestino(int idUsuarioS, string filter = "", string type_filter = "")
        {
            try
            {
                var actualdate = DateTime.Now;
                var year = actualdate.Year;
                var month = actualdate.Month;

                if (string.IsNullOrEmpty(filter) || string.IsNullOrEmpty(type_filter))
                {
                    Console.WriteLine("El filtro o tipo de filtro no puede estar vacío.");
                    return new List<DetallesAsignacionesDTO>();
                }
                if (type_filter != "lista" && type_filter != "destino" && type_filter != "fecha" && type_filter != "base")
                {
                    Console.WriteLine("El tipo de filtro no es válido. Debe ser 'lista', 'destino', 'fecha' o 'base'.");
                    return new List<DetallesAsignacionesDTO>();
                }

                var allclientes = (
                    from ca in _context.clientes_asignados
                    join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                    join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                    where ca.IdUsuarioS == idUsuarioS
                        && ca.IdUsuarioV == null
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
                        Dni = bc.Dni,
                        IdLista = ca.IdLista,
                        Base = ca.FuenteBase
                    }
                );

                if (type_filter == "lista")
                {
                    var lista = _context.listas_asignacion
                        .AsNoTracking()
                        .Where(x => x.NombreLista != null && x.NombreLista.Contains(filter))
                        .Select(x => x.IdLista)
                        .ToHashSet();
                    allclientes = allclientes.Where(x => x.IdLista != null && x.IdLista.ToString() == filter);
                }
                else if (type_filter == "destino")
                {
                    allclientes = allclientes.Where(x => x.Destino != null && x.Destino == filter);
                }
                else if (type_filter == "fecha")
                {
                    if (!DateTime.TryParse(filter, out DateTime fechaFiltro))
                    {
                        Console.WriteLine("El formato de fecha no es válido.");
                        return new List<DetallesAsignacionesDTO>();
                    }
                    allclientes = allclientes.Where(x => x.FechaAsignacionSup.HasValue
                        && x.FechaAsignacionSup.Value.Date >= fechaFiltro.Date);
                }
                else if (type_filter == "base")
                {
                    allclientes = allclientes.Where(x => x.Base != null && x.Base.Contains(filter));
                }
                else
                {
                    Console.WriteLine("Tipo de filtro no reconocido.");
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
                
                if (!getAsignacionesFinal.Any() || getAsignacionesFinal.Count == 0 || getAsignacionesFinal == null)
                {
                    Console.WriteLine("No hay clientes disponibles para la asignación.");
                    return new List<DetallesAsignacionesDTO>();
                }

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
                var ahora = DateTime.Now;
                var año = ahora.Year;
                var mes = ahora.Month;

                var clientesQuery = _context.clientes_asignados
                    .AsNoTracking()
                    .Where(x => x.IdUsuarioS == idUsuarioS &&
                                x.FechaAsignacionSup.HasValue &&
                                x.FechaAsignacionSup.Value.Year == año &&
                                x.FechaAsignacionSup.Value.Month == mes);

                var resumenClientes = await clientesQuery
                    .GroupBy(x => x.IdUsuarioV)
                    .Select(g => new
                    {
                        IdUsuario = g.Key,
                        Total = g.Count(),
                        Gestionados = g.Count(c => c.PesoTipificacionMayor != null),
                        Pendientes = g.Count(c => c.PesoTipificacionMayor == null)
                    })
                    .ToListAsync();

                var vendedores = await _context.usuarios
                    .AsNoTracking()
                    .Where(x => IdsUsuariosVendedores.Contains(x.IdUsuario))
                    .Select(x => new { x.IdUsuario, x.NombresCompletos, x.Estado })
                    .ToListAsync();

                var numeroClientesFromVendedor = vendedores.Select(v =>
                {
                    var resumen = resumenClientes.FirstOrDefault(r => r.IdUsuario == v.IdUsuario);
                    return new DetalleAsignacionContadorFromVendedorDTO
                    {
                        IdUsuario = v.IdUsuario,
                        NombresCompletos = v.NombresCompletos,
                        NumeroClientes = resumen?.Total ?? 0,
                        NumeroClientesGestionados = resumen?.Gestionados ?? 0,
                        NumeroClientesPendientes = resumen?.Pendientes ?? 0,
                        estaActivado = v.Estado == "ACTIVO"
                    };
                }).ToList();

                var getDestinos = await clientesQuery
                    .Select(x => x.Destino)
                    .Distinct()
                    .ToListAsync();
                
                var getListas = await clientesQuery
                    .Select(x => x.IdLista)
                    .Distinct()
                    .ToListAsync();

                var nomListas = await _context.listas_asignacion
                    .AsNoTracking()
                    .Where(x => getListas.Contains(x.IdLista))
                    .Select(x => x.NombreLista)
                    .ToListAsync();
                
                var getBases = await clientesQuery
                    .AsNoTracking()
                    .Select(x => x.FuenteBase)
                    .Distinct()
                    .ToListAsync();

                return new DetallesAsignacionContadorFromVendedorDTO
                {
                    DetallesAsignacionContadorFromVendedor = numeroClientesFromVendedor,
                    Destinos = getDestinos.Where(d => d != null).Cast<string>().ToList(),
                    ListasAsignacion = nomListas.Where(l => l != null).Cast<string>().ToList(),
                    BasesAsignacion = getBases.Where(b => b != null).Cast<string>().ToList(),
                };
            }
            catch (Exception ex)
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