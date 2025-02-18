using System.Data;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
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
                    "EXEC SP_derivacion_insertar_derivacion @fecha_visita_derivacion, @dni_asesor_derivacion, @DNI_cliente_derivacion, @id_cliente, @nombre_cliente_derivacion, @telefono_derivacion, @agencia_derivacion, @num_agencia",
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
        public async Task<(bool IsSuccess, string Message, List<GESTIONDETALLE>? Data)> GetEntradasBSDialXSupervisor(List<Usuario> asesores)
        {
            try
            {
                var getEntradasBSDial = new List<GESTIONDETALLE>();
                foreach (var asesor in asesores)
                {
                    var entradas = await _context.GESTION_DETALLE.FromSqlRaw("EXEC sp_Derivacion_consulta_derivaciones_x_asesor_BS_dial_ACTUALIZADO {0}", asesor.Dni).ToListAsync();
                    getEntradasBSDial.AddRange(entradas);
                }
                return (true, "Se encontraron las siguientes entradas en BSDIAL", getEntradasBSDial);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message)> VerificarDerivacionEnviada(string dni)
        {
            try
            {
                int tiempoMaximoEspera = 30000; // 30 segundos
                int intervaloEspera = 3000; // 3 segundos
                int tiempoTranscurrido = 0;

                while (tiempoTranscurrido < tiempoMaximoEspera)
                {
                    var verificarDerivacionEnviada = await _context.derivaciones_asesores
                        .FromSqlRaw("EXEC sp_Derivacion_verificar_derivacion_enviada {0}", dni)
                        .AsNoTracking()
                        .ToListAsync();
                    if (verificarDerivacionEnviada.Count == 0)
                    {
                        return (false, "No se mando la derivación, esta no fue guardada en la base de datos");
                    }

                    var derivacionEnviada = verificarDerivacionEnviada.FirstOrDefault();
                    if (derivacionEnviada != null && derivacionEnviada.FueProcesado == true)
                    {
                        return (true, "Entrada correctamente procesada");
                    }

                    await Task.Delay(intervaloEspera);
                    tiempoTranscurrido += intervaloEspera;
                }

                return (false, "Tiempo de espera agotado. La entrada no fue procesada.");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
        
    }
}