using Microsoft.EntityFrameworkCore;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using ALFINapp.Domain.Entities;
using ALFINapp.Application.DTOs;
using ALFINapp.DTOs;
using ALFINapp.Domain.Services;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryDerivaciones : IRepositoryDerivaciones
    {
        MDbContext _context;
        public RepositoryDerivaciones(MDbContext context)
        {
            _context = context;
        }

        public async Task<DetallesDerivacionesAsesoresDTO?> getDerivacion(int idDer)
        {
            try
            {
                var derivacion = await _context
                    .derivaciones_asesores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdDerivacion == idDer);
                if (derivacion != null)
                {
                    return new DetallesDerivacionesAsesoresDTO(derivacion);
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
        public async Task<List<DetallesDerivacionesAsesoresDTO>> getDerivaciones(List<Vendedor> asesores)
        {
            try
            {
                var dnis = new DataTable();
                dnis.Columns.Add("Dni", typeof(string));
                var getAllDnisClientes = asesores.Select(x => x.Dni).ToHashSet();
                foreach (var dni in getAllDnisClientes)
                {
                    dnis.Rows.Add(dni);
                }
                var parameter = new SqlParameter("@Dni", SqlDbType.Structured)
                {
                    TypeName = "dbo.DniTableType",
                    Value = dnis
                };
                var result = await _context.derivaciones_asesores_for_view_derivacion
                    .FromSqlRaw("EXEC sp_Derivacion_consulta_derivaciones_x_asesor_por_dni_con_reagendacion @Dni = {0}",
                        parameter)
                    .ToListAsync();
                if (result == null || result.Count == 0)
                {
                    return new List<DetallesDerivacionesAsesoresDTO>();
                }
                return result.Select(x => new DetallesDerivacionesAsesoresDTO(x)).ToList();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<DetallesDerivacionesAsesoresDTO>();
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

        public async Task<(bool success, string message)> marcarEvidenciaDisponible(int idDerivacion)
        {
            try
            {
                var parametros = new[]
                {
                    new SqlParameter("@id_derivacion", idDerivacion) { SqlDbType = SqlDbType.Int }
                };
                var derivacion = await _context.derivaciones_asesores
                    .FirstOrDefaultAsync(x => x.IdDerivacion == idDerivacion);
                if (derivacion == null)
                {
                    return (false, "No se encontró la derivación");
                }
                derivacion.HayEvidencias = true;
                derivacion.FueProcesadaEvidencia = false;
                derivacion.FechaEvidencia = DateTime.Now;
                await _context.SaveChangesAsync();

                var checkProcesamiento = await checkProcesamientoEvidencias(idDerivacion);
                if (!checkProcesamiento.success)
                {
                    return (false, checkProcesamiento.message);
                }
                return (true, "Evidencia marcada como disponible y procesada correctamente");
            }
            catch (System.Exception ex)
            {
                return (false, "Error al marcar la evidencia como disponible: " + ex.Message);
            }
        }

        private async Task<(bool success, string message)> checkProcesamientoEvidencias(int idDerivacion, int maxWaitingTime = 40000, int interval = 1000)
        {
            var waitingTime = 0;

            while (waitingTime < maxWaitingTime)
            {
                var checkDer = await _context.derivaciones_asesores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdDerivacion == idDerivacion);
                if (checkDer == null)
                {
                    return (false, "No se encontró la derivación");
                }
                if (checkDer.HayEvidencias == false)
                {
                    return (true, "Evidencia procesada y eliminada correctamente");
                }
                await Task.Delay(interval);
                waitingTime += interval;
            }
            return (false, "Tiempo de espera agotado. La evidencia no fue procesada, mas si fue guardada no envie mas evidencias para esta derivación");
        }

        public async Task<(bool success, string message)> uploadNuevaDerivacion(
            DerivacionesAsesores derivacion,
            int idBase,
            int idUsuario)
        {
            try
            {
                var parametros = new[]
                {
                    new SqlParameter("@agencia_derivacion", derivacion.NombreAgencia),
                    new SqlParameter("@fecha_visita", derivacion.FechaVisita) { SqlDbType = SqlDbType.DateTime },
                    new SqlParameter("@telefono", derivacion.TelefonoCliente),
                    new SqlParameter("@id_base", idBase),
                    new SqlParameter("@id_usuario", idUsuario),
                    new SqlParameter("@nombre_completos", derivacion.NombreCliente ?? string.Empty)
                };
                var generarDerivacion = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_derivacion_insertar_derivacion_test_N @agencia_derivacion, @fecha_visita, @telefono, @id_base, @id_usuario, @nombre_completos",
                    parametros);
                if (generarDerivacion == 0)
                {
                    return (false, "Error al subir la derivacion");
                }
                return (true, "Derivacion subida correctamente");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error en la base de datos al subir la derivacion" + ex.Message);
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<(bool success, string message)> uploadReagendacion(int idDer, DateTime fechaReagendamiento)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                var parametros = new[]
                {
                    new SqlParameter("@nueva_fecha_visita", fechaReagendamiento) { SqlDbType = SqlDbType.DateTime },
                    new SqlParameter("@id_derivacion", idDer) { SqlDbType = SqlDbType.Int }
                };
                var generarReagendacion = _context.Database.ExecuteSqlRaw(
                    "EXEC sp_reagendamiento_upload_nueva_reagendacion @nueva_fecha_visita, @id_derivacion;",
                    parametros);
                if (generarReagendacion == 0)
                {
                    return (false, "Error al subir la reagendacion");
                }
                return (true, "Reagendacion subida correctamente");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error en la base de datos al subir la reagendacion");
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<(bool success, string message)> uploadReagendacion(string dniCliente, DateTime fechaReagendamiento)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                var parametros = new[]
                {
                    new SqlParameter("@dni_cliente", dniCliente),
                    new SqlParameter("@nueva_fecha_visita", fechaReagendamiento) { SqlDbType = SqlDbType.DateTime }
                };
                var generarReagendacion = _context.Database.ExecuteSqlRaw(
                    "EXEC sp_reagendamiento_upload_nueva_reagendacion @dni_cliente, @nueva_fecha_visita",
                    parametros);
                if (generarReagendacion == 0)
                {
                    return (false, "Error al subir la reagendacion");
                }
                return (true, "Reagendacion subida correctamente");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error en la base de datos al subir la reagendacion");
            }
        }

        public async Task<(bool success, string message)> uploadReagendacionConEvidencias(List<DtoVUploadFiles> evidencias, int idDerivacion, DateTime fechaReagendamiento)
        {
            try
            {
                List<(string file_name, string file_type, byte[] data)> filecontentBytes = new List<(string file_name, string file_type, byte[] data)>();
                foreach (var item in evidencias)
                {
                    byte[] fileContent = ServicesHex.HexStringToBytes(item.fileContent ?? string.Empty);
                    filecontentBytes.Add((item.fileName ?? string.Empty, item.fileType ?? string.Empty, fileContent));
                }

                foreach (var file in filecontentBytes)
                {
                    var sql = "INSERT INTO [dbo].[files_evidencias_only_upload] ([file_name], [file_type], [file_content], [id_derivacion]) " +
                              "VALUES (@fileName, @fileType, @fileContent, @idDerivacion)";
                    var parameters = new[]
                    {
                        new SqlParameter("@fileName", file.file_name),
                        new SqlParameter("@fileType", file.file_type),
                        new SqlParameter("@fileContent", SqlDbType.VarBinary) { Value = file.data },
                        new SqlParameter("@idDerivacion", idDerivacion)
                    };
                    await _context.Database.ExecuteSqlRawAsync(sql, parameters);
                }

                var derivacion = await _context.derivaciones_asesores
                    .FirstOrDefaultAsync(x => x.IdDerivacion == idDerivacion);
                if (derivacion == null)
                {
                    return (false, "No se encontró la derivación");
                }
                var hoy = DateTime.Now;

                derivacion.FechaReagendamiento = hoy;
                derivacion.FechaVisita = fechaReagendamiento;
                derivacion.FueProcesado = false;
                derivacion.FueEnviadoEmail = false;
                derivacion.EstadoDerivacion = "DERIVACION REAGENDADA PENDIENTE";
                derivacion.FueReagendado = true;
                derivacion.FueReprocesado = false;
                derivacion.HayEvidencias = true;
                derivacion.FueProcesadaEvidencia = false;

                await _context.SaveChangesAsync();

                var checkProcesamiento = await checkProcesamientoEvidencias(idDerivacion);
                if (!checkProcesamiento.success)
                {
                    return (false, "Error al procesar las evidencias en el reagendamiento: " + checkProcesamiento.message);
                }
                return (true, "Reagendacion con evidencias subido y procesado correctamente");
            }
            catch (System.Exception ex)
            {
                return (false, "Error al subir la reagendacion con evidencias: " + ex.Message);
            }
        }

        public async Task<(bool success, string message)> verDerivacion(string Dni)
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
                return (false, "Tiempo de espera agotado. La derivación no fue procesada");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error en la base de datos");
            }
        }
        public async Task<(bool success, string message)> verDisponibilidad(int idBase)
        {
            try
            {
                var verDerivacion = await _context.resultado_verificacion
                    .FromSqlRaw("EXEC SP_derivaciones_verificar_disponibilidad_para_derivacion_id_base @id_base", new SqlParameter("@id_base", idBase))
                    .AsNoTracking()
                    .ToListAsync();
                var resultadoVerificacion = verDerivacion.FirstOrDefault();
                if (resultadoVerificacion == null)
                {
                    return (false, "Ha ocurrido un error en la base de datos, intentelo nuevamente");
                }
                if (resultadoVerificacion.Resultado == 0)
                {
                    return (false, resultadoVerificacion.Mensaje);
                }
                else
                {
                    return (true, resultadoVerificacion.Mensaje);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error en la base de datos");
            }
        }
    }
}