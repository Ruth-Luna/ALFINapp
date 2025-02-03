using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                var generarDerivacion = _context.Database.ExecuteSqlRaw("EXEC SP_derivacion_insertar_derivacion {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
                    FechaVisitaDerivacion, AgenciaDerivacion, AsesorDerivacion, DNIAsesorDerivacion, TelefonoDerivacion, DNIClienteDerivacion, NombreClienteDerivacion, idDni, "");
                    
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
    }
}