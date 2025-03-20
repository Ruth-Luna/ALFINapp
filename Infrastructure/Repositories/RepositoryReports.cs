using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    }
}