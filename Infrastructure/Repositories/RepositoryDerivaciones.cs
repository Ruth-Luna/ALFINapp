using Microsoft.EntityFrameworkCore;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using ALFINapp.Domain.Entities;
using ALFINapp.Application.DTOs;

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
                    "EXEC SP_derivacion_insertar_derivacion_test_ONLY_SVL @agencia_derivacion, @fecha_visita, @telefono, @id_base, @id_usuario, @nombre_completos",
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
                    new SqlParameter("@id_derivacion", idDer)
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
                return (false, "Tiempo de espera agotado. La derivación no fue procesada");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error en la base de datos");
            }
        }

        public async Task<(bool success, string message)> verDisponibilidad(string DniCliente, string DniAsesor)
        {
            try
            {
                var parametros = new[]
                {
                    new SqlParameter("@dni_cliente", DniCliente),
                    new SqlParameter("@dni_asesor", DniAsesor)
                };
                var verDerivacion = await _context.resultado_verificacion
                    .FromSqlRaw("EXEC SP_verificar_disponibilidad_para_derivacion @dni_cliente, @dni_asesor", parametros)
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