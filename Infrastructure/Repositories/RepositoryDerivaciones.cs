using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryDerivaciones : IRepositoryDerivaciones
    {
        MDbContext _context;
        public RepositoryDerivaciones(MDbContext context)
        {
            _context = context;
        }
        public async Task<List<DerivacionesAsesores>?> getDerivaciones(int idCliente, string docAsesor)
        {
            try
            {
                var derivacion = await _context
                    .derivaciones_asesores
                    .AsNoTracking()
                    .Where(x => x.IdCliente == idCliente 
                        && x.DniAsesor == docAsesor
                        && x.FechaDerivacion.Year == DateTime.Now.Year
                        && x.FechaDerivacion.Month == DateTime.Now.Month)
                    .ToListAsync();
                if (derivacion != null)
                {
                    return derivacion;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<GESTIONDETALLE?> getGestionDerivacion(string docCliente, string docAsesor)
        {
            try
            {
                var gestion = await _context
                    .GESTION_DETALLE
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.DocCliente == docCliente 
                        && x.DocAsesor == docAsesor
                        && x.FechaGestion.Year == DateTime.Now.Year
                        && x.FechaGestion.Month == DateTime.Now.Month
                        && x.CodTip == 2
                        );
                if (gestion != null)
                {
                    return gestion;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> uploadDerivacion(DerivacionesAsesores derivacion)
        {
            try
            {
                var verificarDerivacion = (from ad in _context.derivaciones_asesores
                                           where ad.DniAsesor == derivacion.DniAsesor
                                                && ad.DniCliente == derivacion.DniCliente
                                                && ad.FechaDerivacion.Year == DateTime.Now.Year
                                                && ad.FechaDerivacion.Month == DateTime.Now.Month
                                           select ad).FirstOrDefault();
                if (verificarDerivacion != null)
                {
                    return false;
                }

                var idDni = await (from bc in _context.base_clientes
                                   join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                   where bc.Dni == derivacion.DniCliente
                                   select (int?)ce.IdCliente).FirstOrDefaultAsync();

                var parametros = new[]
                {
                    new SqlParameter("@fecha_visita_derivacion", derivacion.FechaVisita) { SqlDbType = SqlDbType.DateTime },
                    new SqlParameter("@dni_asesor_derivacion", derivacion.DniAsesor),
                    new SqlParameter("@DNI_cliente_derivacion", derivacion.DniCliente),
                    new SqlParameter("@id_cliente", idDni != null ? idDni.Value : DBNull.Value),
                    new SqlParameter("@nombre_cliente_derivacion", derivacion.NombreCliente),
                    new SqlParameter("@telefono_derivacion", derivacion.TelefonoCliente),
                    new SqlParameter("@agencia_derivacion", derivacion.NombreAgencia),
                    new SqlParameter("@num_agencia", DBNull.Value)
                };

                var generarDerivacion = _context.Database.ExecuteSqlRaw(
                    "EXEC SP_derivacion_insertar_derivacion @fecha_visita_derivacion, @dni_asesor_derivacion, @DNI_cliente_derivacion, @id_cliente, @nombre_cliente_derivacion, @telefono_derivacion, @agencia_derivacion, @num_agencia",
                    parametros);

                if (generarDerivacion == 0)
                {
                    return false;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<(bool success,string message)> verDerivacion(string Dni)
        {
            try
            {
                int tiempoMaximoEspera = 40000;
                int intervaloEspera = 1000;
                int tiempoTranscurrido = 0;

                var verificarDerivacionEnviada = await _context.derivaciones_asesores
                        .FromSqlRaw("EXEC sp_Derivacion_verificar_derivacion_enviada {0}", Dni)
                        .AsNoTracking()
                        .ToListAsync();
                var derivacionEnviada = verificarDerivacionEnviada.FirstOrDefault();
                while (tiempoTranscurrido < tiempoMaximoEspera)
                {
                    verificarDerivacionEnviada = await _context.derivaciones_asesores
                        .FromSqlRaw("EXEC sp_Derivacion_verificar_derivacion_enviada {0}", Dni)
                        .AsNoTracking()
                        .ToListAsync();
                    if (verificarDerivacionEnviada.Count == 0)
                    {
                        return (false, "No se mando la derivación, esta no fue guardada en la base de datos, vuelva a intentarlo");
                    }
                    derivacionEnviada = verificarDerivacionEnviada.FirstOrDefault();
                    if (derivacionEnviada != null && derivacionEnviada.FueProcesado == true)
                    {
                        return (true, "Entrada correctamente procesada");
                    }
                    await Task.Delay(intervaloEspera);
                    tiempoTranscurrido += intervaloEspera;
                }
                if (derivacionEnviada == null)
                {
                    return (false, "No se encontró la derivación en la base de datos, intentelo nuevamente");
                }
                return (false, "Tiempo de espera agotado. La entrada no fue procesada. Pero fue guardada correctamente en nuestro sistema no sera necesario que envie mas derivaciones de este cliente en caso su rol sea Asesor. Su derivacion sera procesada muy pronto. Para conocer el estado de su derivacion puede dirigirse a la pestaña de Derivaciones, ademas no se olvide de guardar la Tipificacion");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error en la base de datos");
            }
        }
    }
}