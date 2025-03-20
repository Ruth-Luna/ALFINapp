using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryReports : IRepositoryReports
    {
        MDbContext _context;
        public RepositoryReports(MDbContext context)
        {
            _context = context;
        }
        public async Task<DetallesReportesAdministradorDTO?> GetReportesAdministradorAsesores()
        {
            try
            {
                var actualYear = DateTime.Now.Year;
                var actualMonth = DateTime.Now.Month;
                var AllAsesores = await _context
                    .usuarios
                    .Where(x => x.IdRol == 3
                        && x.Estado == "ACTIVO")
                    .ToListAsync();
                var getDnis = AllAsesores.Select(x => x.Dni).ToHashSet();
                var Derivaciones = await _context.derivaciones_asesores
                    .Where(x => getDnis.Contains(x.DniAsesor) 
                        && x.FechaDerivacion.Year == actualYear
                        && x.FechaDerivacion.Month == actualMonth)
                    .ToListAsync();
                var DerivacionesDnisClientes = Derivaciones.Select(x => x.DniCliente).ToHashSet();
                var Desembolsos = await _context.desembolsos
                    .Where(x => DerivacionesDnisClientes.Contains(x.DniDesembolso)
                        && x.FechaDesembolsos.HasValue
                        && x.FechaDesembolsos.Value.Year == DateTime.Now.Year
                        && x.FechaDesembolsos.Value.Month == DateTime.Now.Month)
                    .ToListAsync();
                var DerivacionesAllInfo = new DetallesReportesAdministradorDTO ();
                foreach (var asesor in AllAsesores)
                {
                    var DerivacionesAsesores = new DerivacionesAsesoresList();
                    DerivacionesAsesores.DniAsesor = asesor.Dni;
                    DerivacionesAsesores.NombreCompletoAsesor = asesor.NombresCompletos;
                    DerivacionesAsesores.CantidadDerivaciones = Derivaciones
                        .Where(x => x.DniAsesor == asesor.Dni)
                        .Count();
                    DerivacionesAsesores.AllDerivaciones = Derivaciones
                        .Where(x => x.DniAsesor == asesor.Dni)
                        .Select(x => new DerivacionInformacionDesembolsos
                        {
                            DerivacionAsesor = x,
                            FueDesembolsado = Desembolsos
                                .Where(y => y.DniDesembolso == x.DniCliente)
                                .Any(),
                            Desembolso = Desembolsos
                                .Where(y => y.DniDesembolso == x.DniCliente)
                                .FirstOrDefault()
                        })
                        .ToList();
                    DerivacionesAsesores.CantidadDerivacionesProcesadas = DerivacionesAsesores
                        .AllDerivaciones
                        .Where(x => x.DerivacionAsesor != null && x.DerivacionAsesor.FueProcesado == true)
                        .Count();
                    DerivacionesAsesores.CantidadDerivacionesDesembolsadas = DerivacionesAsesores
                        .AllDerivaciones
                        .Where(x => x.FueDesembolsado == true)
                        .Count();
                    DerivacionesAsesores.CantidadDerivacionesPendientes = DerivacionesAsesores
                        .AllDerivaciones
                        .Where(x => x.FueDesembolsado == false)
                        .Count();
                    DerivacionesAllInfo.DerivacionesAllInfo = DerivacionesAsesores;
                }
                return DerivacionesAllInfo;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<DetallesReportesAsesorDTO?> GetReportesAsesor(int idUsuario)
        {
            try
            {
                var getUsuario = await _context.usuarios
                    .Where(x => x.IdUsuario == idUsuario)
                    .FirstOrDefaultAsync();
                if (getUsuario == null)
                {
                    Console.WriteLine("Usuario no encontrado");
                    return null;
                }
                var getAllAsignaciones = await _context.clientes_asignados
                    .Where(x => x.IdUsuarioV == idUsuario
                        && x.FechaAsignacionVendedor.HasValue
                        && x.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                        && x.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month)
                    .ToListAsync();

                var getAllIds = getAllAsignaciones.Select(x => x.IdAsignacion).ToHashSet();
                var getAllDerivaciones = await _context.derivaciones_asesores
                    .Where(x => x.DniAsesor == getUsuario.Dni
                        && x.FechaDerivacion.Year == DateTime.Now.Year
                        && x.FechaDerivacion.Month == DateTime.Now.Month)
                    .ToListAsync();
                var getAllClientesTipificados = await _context.clientes_tipificados
                    .Where(x => getAllIds.Contains(x.IdAsignacion)
                        && x.FechaTipificacion.HasValue
                        && x.FechaTipificacion.Value.Year == DateTime.Now.Year
                        && x.FechaTipificacion.Value.Month == DateTime.Now.Month)
                    .GroupBy(x => x.IdAsignacion)
                    .Select(g => g.OrderByDescending(x => x.IdClientetip).First())
                    .ToListAsync();

                var detallesReporte = new DetallesReportesAsesorDTO();
                detallesReporte.Usuario = getUsuario;
                detallesReporte.ClientesAsignados = getAllAsignaciones;
                detallesReporte.DerivacionesDelAsesor = getAllDerivaciones;
                detallesReporte.UltimaTipificacionXAsignacion = getAllClientesTipificados;
                return detallesReporte;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<DetallesReportesDerivacionesDTO?> GetReportesDerivacionGral ()
        {
            try
            {
                var getDerivacionesGral = await _context.derivaciones_asesores
                    .Where(x => x.FechaDerivacion.Year == DateTime.Now.Year
                        && x.FechaDerivacion.Month == DateTime.Now.Month)
                    .ToListAsync();
                var getDnis = getDerivacionesGral.Select(x => x.DniAsesor).ToHashSet();
                var getGestionDetalles = await _context.GESTION_DETALLE
                    .Where(x => getDnis.Contains(x.DocCliente)
                        && x.FechaGestion.Year == DateTime.Now.Year
                        && x.FechaGestion.Month == DateTime.Now.Month)
                    .GroupBy(x => x.DocCliente)
                    .Select(g => g.OrderByDescending(x => x.IdFeedback).First())
                    .ToListAsync();
                var getDesembolsos = await _context.desembolsos
                    .Where(x => getDnis.Contains(x.DniDesembolso)
                        && x.FechaDesembolsos.HasValue
                        && x.FechaDesembolsos.Value.Year == DateTime.Now.Year
                        && x.FechaDesembolsos.Value.Month == DateTime.Now.Month
                        && x.Sucursal != null)
                    .ToListAsync();
                var detallesReporte = new DetallesReportesDerivacionesDTO();
                detallesReporte.DerivacionesGral = getDerivacionesGral;
                detallesReporte.GestionDetalles = getGestionDetalles;
                detallesReporte.Desembolsos = getDesembolsos;
                return detallesReporte;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}