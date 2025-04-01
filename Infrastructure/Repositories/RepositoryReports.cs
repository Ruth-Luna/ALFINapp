using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
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
                var DerivacionesAllInfo = new DetallesReportesAdministradorDTO();
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

        public async Task<DetallesReportesAsesorDTO> GetReportesAsesor(int idUsuario)
        {
            try
            {
                var getUsuario = await _context.usuarios
                    .Where(x => x.IdUsuario == idUsuario)
                    .FirstOrDefaultAsync();
                if (getUsuario == null)
                {
                    Console.WriteLine("Usuario no encontrado");
                    return new DetallesReportesAsesorDTO();
                }
                var getAllAsignaciones = await _context.clientes_asignados
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Asesor_asignaciones @DniAsesor",
                        new SqlParameter("@DniAsesor", getUsuario.Dni))
                    .ToListAsync();

                var getAllIds = getAllAsignaciones.Select(x => x.IdAsignacion).ToHashSet();
                var getAllDerivaciones = await _context.derivaciones_asesores
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Asesor_derivacion @DniAsesor",
                        new SqlParameter("@DniAsesor", getUsuario.Dni))
                    .AsNoTracking()
                    .ToListAsync();

                var getAllGestionDetalle = await _context.GESTION_DETALLE
                    .AsNoTracking()
                    .Where(x => x.DocAsesor == getUsuario.Dni
                        && x.FechaGestion.Year == DateTime.Now.Year
                        && x.FechaGestion.Month == DateTime.Now.Month)
                    .GroupBy(x => x.DocCliente)
                    .Select(g => g.OrderByDescending(x => x.FechaGestion).First())
                    .ToListAsync();

                var getAllDesembolsos = await _context.desembolsos
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Asesor_desembolsos @DniAsesor",
                        new SqlParameter("@DniAsesor", getUsuario.Dni))
                    .AsNoTracking()
                    .ToListAsync();

                var detallesReporte = new DetallesReportesAsesorDTO();
                detallesReporte.Usuario = getUsuario;
                detallesReporte.ClientesAsignados = getAllAsignaciones;
                detallesReporte.DerivacionesDelAsesor = getAllDerivaciones;
                detallesReporte.gESTIONDETALLEs = getAllGestionDetalle;
                detallesReporte.Desembolsos = getAllDesembolsos;
                return detallesReporte;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesAsesorDTO();
            }
        }

        public async Task<DetallesReportesDerivacionesDTO?> GetReportesDerivacionGral()
        {
            try
            {
                var getDerivacionesGral = await _context.derivaciones_asesores
                    .Where(x => x.FechaDerivacion.Year == DateTime.Now.Year
                        && x.FechaDerivacion.Month == DateTime.Now.Month)
                    .ToListAsync();
                var getDnisAsesor = getDerivacionesGral.Select(x => x.DniAsesor).ToHashSet();
                var getDnisCliente = getDerivacionesGral.Select(x => x.DniCliente).ToHashSet();
                var getGestionDetalles = await _context.GESTION_DETALLE
                    .Where(x => getDnisCliente.Contains(x.DocCliente)
                        && x.FechaGestion.Year == DateTime.Now.Year
                        && x.FechaGestion.Month == DateTime.Now.Month)
                    .GroupBy(x => x.DocCliente)
                    .Select(g => g.OrderByDescending(x => x.IdFeedback).First())
                    .ToListAsync();
                var getDesembolsos = await _context.desembolsos
                    .Where(x => getDnisCliente.Contains(x.DniDesembolso)
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

        public async Task<DetallesReportesDerivacionesDTO?> GetReportesGralSupervisor(int idSupervisor)
        {
            try
            {
                var getAsesores = await _context
                    .usuarios
                    .Where(x => x.IDUSUARIOSUP == idSupervisor)
                    .ToListAsync();
                var dniAsesores = getAsesores.Select(x => x.Dni).ToHashSet();
                var getDerivacionesGral = await _context.derivaciones_asesores
                    .Where(x => dniAsesores.Contains(x.DniAsesor)
                        && x.FechaDerivacion.Year == DateTime.Now.Year
                        && x.FechaDerivacion.Month == DateTime.Now.Month)
                    .ToListAsync();
                var getGestionDetalles = await _context.GESTION_DETALLE
                    .Where(x => dniAsesores.Contains(x.DocAsesor)
                        && x.FechaGestion.Year == DateTime.Now.Year
                        && x.FechaGestion.Month == DateTime.Now.Month)
                    .GroupBy(x => x.DocCliente)
                    .Select(g => g.OrderByDescending(x => x.IdFeedback).First())
                    .ToListAsync();
                var getDesembolsos = await _context.desembolsos
                    .Where(x => dniAsesores.Contains(x.DocAsesor)
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
        public async Task<DetallesReportesSupervisorDTO> GetReportesEspecificoSupervisor(int idUsuario)
        {
            try
            {
                var usuario = await _context.usuarios
                    .Where(x => x.IdUsuario == idUsuario)
                    .FirstOrDefaultAsync();
                if (usuario == null)
                {
                    Console.WriteLine("Usuario no encontrado");
                    return new DetallesReportesSupervisorDTO();
                }
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;

                var getAsignaciones = await _context.clientes_asignados.FromSqlRaw(
                    "EXEC SP_Reportes_Supervisor_asignaciones @DniSupervisor",
                    new SqlParameter("@DniSupervisor", usuario.Dni))
                    .AsNoTracking()
                    .ToListAsync();

                var idsAsignaciones = getAsignaciones.Select(x => x.IdAsignacion).ToHashSet();
                var getAsesores = await _context
                    .usuarios
                    .Where(x => x.IDUSUARIOSUP == idUsuario)
                    .ToListAsync();
                
                var getGestionDetalles = await _context.GESTION_DETALLE.FromSqlRaw(
                    "EXEC SP_Reportes_Supervisor_gestion @DniSupervisor",
                    new SqlParameter("@DniSupervisor", usuario.Dni))
                    .AsNoTracking()
                    .ToListAsync();

                var dniAsesores = getAsesores.Select(x => x.Dni).ToHashSet();
                var getDerivaciones = await _context
                    .derivaciones_asesores
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Supervisor_derivacion @DniSupervisor",
                        new SqlParameter("@DniSupervisor", usuario.Dni))
                    .ToListAsync();

                var getDesembolsos = await _context
                    .desembolsos
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Supervisor_desembolso @DniSupervisor",
                        new SqlParameter("@DniSupervisor", usuario.Dni))
                    .AsNoTracking()
                    .ToListAsync();

                var getDerivacionesFecha = await _context.reportes_derivacion_fecha
                    .FromSqlRaw(
                        "EXEC SP_Reportes_Supervisor_derivacion_fecha @DniSupervisor",
                        new SqlParameter("@DniSupervisor", usuario.Dni))
                    .AsNoTracking()
                    .ToListAsync();
                    
                var detallesReporte = new DetallesReportesSupervisorDTO();
                detallesReporte.Asesores = getAsesores;
                detallesReporte.ClientesAsignados = getAsignaciones;
                detallesReporte.DerivacionesSupervisor = getDerivaciones;
                detallesReporte.Desembolsos = getDesembolsos;
                detallesReporte.gESTIONDETALLEs = getGestionDetalles;
                return detallesReporte;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DetallesReportesSupervisorDTO();
            }
        }

        public async Task<DetallesReportesDerivacionesDTO> GetReportesGralAsesor(int idAsesor)
        {
            try
            {
                var getAsesor = await _context.usuarios
                    .Where(x => x.IdUsuario == idAsesor)
                    .FirstOrDefaultAsync();
                if (getAsesor == null)
                {
                    Console.WriteLine("Asesor no encontrado");
                    return new DetallesReportesDerivacionesDTO();
                }
                var getDerivacionesGral = await _context.derivaciones_asesores
                    .Where(x => x.DniAsesor == getAsesor.Dni
                        && x.FechaDerivacion.Year == DateTime.Now.Year
                        && x.FechaDerivacion.Month == DateTime.Now.Month)
                    .ToListAsync();
                var getGestionDetalles = await _context.GESTION_DETALLE
                    .Where(x => x.DocAsesor == getAsesor.Dni
                        && x.FechaGestion.Year == DateTime.Now.Year
                        && x.FechaGestion.Month == DateTime.Now.Month)
                    .GroupBy(x => x.DocCliente)
                    .Select(g => g.OrderByDescending(x => x.IdFeedback).First())
                    .ToListAsync();
                var getDesembolsos = await _context.desembolsos
                    .Where(x => x.DocAsesor == getAsesor.Dni
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
                return new DetallesReportesDerivacionesDTO();
            }
        }
    }
}