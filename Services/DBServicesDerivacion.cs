using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Services
{
    public class DBServicesDerivacion
    {
        private readonly MDbContext _context;
        public DBServicesDerivacion(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, string Message)> GenerarDerivacion(DateTime FechaVisitaDerivacion,
                                                    string AgenciaDerivacion,
                                                    string AsesorDerivacion,
                                                    string DNIAsesorDerivacion,
                                                    string TelefonoDerivacion,
                                                    string DNIClienteDerivacion,
                                                    string NombreClienteDerivacion)
        {
            try
            {
                var idDni = await (from bc in _context.base_clientes
                                   join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                   where bc.Dni == DNIClienteDerivacion
                                   select ce.IdCliente).FirstOrDefaultAsync();

                var generarDerivacion = _context.Database.ExecuteSqlRaw(
                    "EXEC SP_derivacion_insertar_derivacion {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                    FechaVisitaDerivacion,
                    DNIAsesorDerivacion,
                    DNIClienteDerivacion,
                    (object?)idDni ?? DBNull.Value,
                    NombreClienteDerivacion,
                    TelefonoDerivacion,
                    AgenciaDerivacion,
                    DBNull.Value);
                if (generarDerivacion == 0)
                {
                    return (false, "No se pudo generar la derivación");
                }
                return (true, "Derivación generada correctamente");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool IsSuccess, string Message, List<DerivacionesAsesores>? Data)> GetDerivacionesXAsesor(string dni)
        {
            try
            {
                var getDerivaciones = await _context.derivaciones_asesores.FromSqlRaw("EXEC sp_Derivacion_consulta_derivaciones_x_asesor {0}", dni).ToListAsync();
                if (getDerivaciones == null)
                {
                    return (false, "No se encontraron derivaciones", null);
                }
                return (true, "Derivaciones obtenidas correctamente", getDerivaciones);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string Message, List<DerivacionesAsesores>? Data)> GetClientesDerivadosGenerales(List<Usuario> asesores)
        {
            try
            {
                var getClientesDerivados = new List<DerivacionesAsesores>();
                foreach (var asesor in asesores)
                {
                    var derivaciones = await _context.derivaciones_asesores.FromSqlRaw("EXEC sp_Derivacion_consulta_derivaciones_x_asesor_por_dni {0}", asesor.Dni).ToListAsync();
                    getClientesDerivados.AddRange(derivaciones);
                }
                return (true, "Derivaciones obtenidas correctamente", getClientesDerivados);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string Message, List<FeedGReportes>? Data)> GetEntradasBSDialXSupervisor(List<Usuario> asesores)
        {
            try
            {
                var getEntradasBSDial = new List<FeedGReportes>();
                foreach (var asesor in asesores)
                {
                    var entradas = await _context.feed_G_REPORTES.FromSqlRaw("EXEC sp_Derivacion_consulta_derivaciones_x_asesor_BS_dial {0}", asesor.Dni).ToListAsync();
                    getEntradasBSDial.AddRange(entradas);
                }
                return (true, "Se encontraron las siguientes entradas en BSDIAL", getEntradasBSDial);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}