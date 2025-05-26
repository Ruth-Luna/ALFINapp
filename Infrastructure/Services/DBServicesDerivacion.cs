using System.Data;
using System.Text.RegularExpressions;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Services
{
    /// <summary>
    /// Service class that provides database operations for client referral/derivation management in the ALFINapp system.
    /// Handles creation, verification, and retrieval of derivation data between advisors and clients.
    /// </summary>
    public class DBServicesDerivacion
    {
        private readonly MDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="DBServicesDerivacion"/> class.
        /// </summary>
        /// <param name="context">The database context used for database operations.</param>
        public DBServicesDerivacion(MDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Generates a new client derivation record in the system.
        /// </summary>
        /// <param name="FechaVisitaDerivacion">Scheduled date for the client visit.</param>
        /// <param name="AgenciaDerivacion">Agency name where the derivation will be processed.</param>
        /// <param name="DNIAsesorDerivacion">DNI of the advisor handling the derivation.</param>
        /// <param name="TelefonoDerivacion">Contact phone number for the derivation.</param>
        /// <param name="DNIClienteDerivacion">DNI of the client being derived.</param>
        /// <param name="NombreClienteDerivacion">Full name of the client being derived.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the derivation was successfully generated
        /// - Message: Descriptive message about the result
        /// </returns>
        /// <remarks>
        /// This method first checks if a derivation already exists for the same advisor-client pair in the current month.
        /// If not, it creates a new derivation record through the stored procedure SP_derivacion_insertar_derivacion_test.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string Message)> GenerarDerivacion(DateTime FechaVisitaDerivacion,
                                                    string AgenciaDerivacion,
                                                    string DNIAsesorDerivacion,
                                                    string TelefonoDerivacion,
                                                    string DNIClienteDerivacion,
                                                    string NombreClienteDerivacion)
        {
            try
            {
                var verificarDerivacion = (from ad in _context.derivaciones_asesores
                                           where ad.DniAsesor == DNIAsesorDerivacion
                                                && ad.DniCliente == DNIClienteDerivacion
                                                && ad.FechaDerivacion.Year == DateTime.Now.Year
                                                && ad.FechaDerivacion.Month == DateTime.Now.Month
                                           select ad).FirstOrDefault();
                if (verificarDerivacion != null)
                {
                    return (false, "Ya se ha generado una derivación para este cliente en este mes, puede verificar el estado de la derivacion en la pestaña de derivaciones");
                }

                var idDni = await (from bc in _context.base_clientes
                                   join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                   where bc.Dni == DNIClienteDerivacion
                                   select (int?)ce.IdCliente).FirstOrDefaultAsync();

                var parametros = new[]
                {
                    new SqlParameter("@fecha_visita_derivacion", FechaVisitaDerivacion) { SqlDbType = SqlDbType.DateTime },
                    new SqlParameter("@dni_asesor_derivacion", DNIAsesorDerivacion),
                    new SqlParameter("@DNI_cliente_derivacion", DNIClienteDerivacion),
                    new SqlParameter("@id_cliente", idDni != null ? idDni.Value : DBNull.Value),
                    new SqlParameter("@nombre_cliente_derivacion", NombreClienteDerivacion),
                    new SqlParameter("@telefono_derivacion", TelefonoDerivacion),
                    new SqlParameter("@agencia_derivacion", AgenciaDerivacion),
                    new SqlParameter("@num_agencia", DBNull.Value)
                };

                var generarDerivacion = _context.Database.ExecuteSqlRaw(
                    "EXEC SP_derivacion_insertar_derivacion_test @fecha_visita_derivacion, @dni_asesor_derivacion, @DNI_cliente_derivacion, @id_cliente, @nombre_cliente_derivacion, @telefono_derivacion, @agencia_derivacion, @num_agencia",
                    parametros);

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
        /// <summary>
        /// Verifies if a client derivation has been sent and processed in the system.
        /// </summary>
        /// <param name="dni">DNI of the client to verify derivation status for.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the derivation was successfully processed
        /// - Message: Descriptive message about the result or current status
        /// </returns>
        /// <remarks>
        /// This method implements a polling mechanism that checks the derivation status periodically:
        /// - Maximum wait time: 40 seconds
        /// - Polling interval: 1 second
        /// - Uses stored procedure sp_Derivacion_verificar_derivacion_enviada to check status
        /// 
        /// The method returns different messages based on the verification outcome:
        /// - Derivation not found
        /// - Derivation processed successfully
        /// - Derivation saved but not yet processed (timeout)
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string Message)> VerificarDerivacionEnviada(string dni)
        {
            try
            {
                int tiempoMaximoEspera = 40000;
                int intervaloEspera = 1000;
                int tiempoTranscurrido = 0;

                var verificarDerivacionEnviada = await _context.derivaciones_asesores
                        .FromSqlRaw("EXEC sp_Derivacion_verificar_derivacion_enviada {0}", dni)
                        .AsNoTracking()
                        .ToListAsync();
                var derivacionEnviada = verificarDerivacionEnviada.FirstOrDefault();
                while (tiempoTranscurrido < tiempoMaximoEspera)
                {
                    verificarDerivacionEnviada = await _context.derivaciones_asesores
                        .FromSqlRaw("EXEC sp_Derivacion_verificar_derivacion_enviada {0}", dni)
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
                return (false, ex.Message);
            }
        }
    }
}