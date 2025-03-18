using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Services
{
    public class DBServicesReferido
    {
        private readonly MDbContext _context;

        public DBServicesReferido(MDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Guarda un cliente referido en la base de datos especificada.
        /// </summary>
        /// <param name="dni">El DNI del cliente referido.</param>
        /// <param name="fuenteBase">La fuente de la base de datos (DBA365 o DBALFIN).</param>
        /// <param name="nombres">Los nombres del asesor que refiere al cliente.</param>
        /// <param name="apellidos">Los apellidos del asesor que refiere al cliente.</param>
        /// <param name="dniUsuario">El DNI del usuario que refiere al cliente.</param>
        /// <returns>Una tupla que indica si la operación fue exitosa y un mensaje asociado.</returns>
        public async Task<(bool IsSuccess, string Message)> GuardarClienteReferido(string dni,
                                                                                string fuenteBase,
                                                                                string nombres,
                                                                                string apellidos,
                                                                                string dniUsuario,
                                                                                string telefono,
                                                                                string agencia,
                                                                                DateTime fechaVisita,
                                                                                string nombresCliente,
                                                                                decimal OfertaEnviada,
                                                                                string celular,
                                                                                string correo,
                                                                                string cci,
                                                                                string departamento,
                                                                                string ubigeo,
                                                                                string banco)
        {
            try
            {
                if (fuenteBase == "DBA365")
                {
                    var datosClienteReferido = await (from bc in _context.base_clientes
                                                      where bc.Dni == dni
                                                      select new
                                                      {
                                                          bc
                                                      }).FirstOrDefaultAsync();

                    if (datosClienteReferido == null)
                    {
                        return (false, "No se ha encontrado el usuario en la base de datos enviada");
                    }

                    var clienteReferido = new ClientesReferidos
                    {
                        IdBaseClienteA365 = datosClienteReferido.bc.IdBase,
                        IdSupervisorReferido = 39,
                        NombreCompletoAsesor = nombres + " " + apellidos,
                        NombreCompletoCliente = datosClienteReferido.bc.XNombre + " " + datosClienteReferido.bc.XAppaterno + " " + datosClienteReferido.bc.XApmaterno,
                        DniAsesor = dniUsuario,
                        DniCliente = datosClienteReferido.bc.Dni,
                        FechaReferido = DateTime.Now,
                        TraidoDe = "DBA365",
                        FueProcesado = false,
                        Telefono = telefono,
                        Agencia = agencia,
                        OfertaEnviada = OfertaEnviada,
                        FechaVisita = fechaVisita,
                        CelularAsesor = celular,
                        CorreoAsesor = correo,
                        CciAsesor = cci,
                        DepartamentoAsesor = departamento,
                        UbigeoAsesor = ubigeo,
                        BancoAsesor = banco,
                        EstadoReferencia = "DERIVACION PENDIENTE"
                    };
                    _context.Add(clienteReferido);
                    await _context.SaveChangesAsync();
                    return (true, "Usuario referido exitosamente");
                }
                else if (fuenteBase == "DBALFIN")
                {
                    var datosClienteReferido = await (from bcb in _context.base_clientes_banco
                                                      where bcb.Dni == dni
                                                      orderby bcb.FechaSubida descending
                                                      select new
                                                      {
                                                          bcb
                                                      }).FirstOrDefaultAsync();

                    if (datosClienteReferido == null)
                    {
                        return (false, "No se ha encontrado el usuario en la base de datos enviada");
                    }

                    var clienteReferido = new ClientesReferidos
                    {
                        IdBaseClienteBanco = datosClienteReferido.bcb.IdBaseBanco,
                        IdSupervisorReferido = 39,
                        NombreCompletoAsesor = nombres + " " + apellidos,
                        NombreCompletoCliente = nombresCliente,
                        DniAsesor = dniUsuario,
                        DniCliente = datosClienteReferido.bcb.Dni,
                        FechaReferido = DateTime.Now,
                        TraidoDe = "DBALFIN",
                        FueProcesado = false,
                        Telefono = telefono,
                        Agencia = agencia,
                        FechaVisita = fechaVisita,
                        OfertaEnviada = OfertaEnviada,
                        CelularAsesor = celular,
                        CorreoAsesor = correo,
                        CciAsesor = cci,
                        DepartamentoAsesor = departamento,
                        UbigeoAsesor = ubigeo,
                        BancoAsesor = banco,
                        EstadoReferencia = "DERIVACION PENDIENTE"
                    };
                    _context.Add(clienteReferido);
                    await _context.SaveChangesAsync();
                    return (true, "Usuario referido exitosamente");
                }
                return (false, "No se ha encontrado la fuente de la base de datos asignada");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Busca información de un cliente por DNI en las bases de datos A365 y ALFIN
        /// </summary>
        /// <param name="DNI">Número de documento de identidad del cliente a buscar</param>
        /// <returns>
        /// Una tupla que contiene:
        /// - IsSuccess: Indica si la búsqueda fue exitosa
        /// - Message: Mensaje descriptivo del resultado de la operación
        /// - data: Objeto DniReferidoData con la información del cliente. Null si no se encuentra
        /// </returns>
        /// <remarks>
        /// Este método realiza la búsqueda en dos bases de datos:
        /// 1. Primero busca en base_clientes (A365)
        /// 2. Si no encuentra, busca en base_clientes_banco (ALFIN)
        /// La búsqueda prioriza los registros de A365 sobre ALFIN
        /// </remarks>
        /// <exception cref="Exception">Se lanza cuando ocurre un error en la consulta a la base de datos</exception>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// var result = await GetDataFromDNI("12345678");
        /// if (result.IsSuccess && result.data != null)
        /// {
        ///     var clienteData = result.data;
        ///     // Procesar datos del cliente
        /// }
        /// </code>
        /// </example>
        public async Task<(bool IsSuccess, string Message, DniReferidoData? data)> GetDataFromDNI(string DNI)
        {
            try
            {
                var detalleClienteList = await _context.detalle_base
                            .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_por_DNI_A365 @DNI", 
                                new SqlParameter("@DNI", DNI)) 
                            .ToListAsync();

                if (detalleClienteList.Count != 0)
                {
                    var detalleCliente = detalleClienteList.FirstOrDefault();

                    var clientebc = await _context.base_clientes.Where(bc => bc.Dni == DNI).FirstOrDefaultAsync();

                    if (clientebc == null)
                    {
                        return (false, "El cliente no tiene Detalles en la Base de Datos de A365, este DNI no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                    }

                    var dataclientebc = new DniReferidoData
                    {
                        DNI = clientebc.Dni,
                        NombresCompletos = clientebc.XNombre + " " + clientebc.XAppaterno + " " + clientebc.XApmaterno,
                        IdBaseCliente = clientebc.IdBase,
                        TraidoDe = "DBA365",
                        OfertaMaxima = detalleCliente?.OfertaMax ?? 0
                    };
                    return (true, "El Usuario se ha encontrado en la Base de Datos de A365", dataclientebc);
                }

                var clientebcbList = await _context.detalles_clientes_dto.FromSqlRaw("SP_Consulta_Obtener_Cliente_Banco_Alfin @DNIBusqueda", new SqlParameter("@DNIBusqueda", DNI))
                                                                    .ToListAsync();

                if (clientebcbList.Count == 0)
                {
                    return (false, "No se ha encontrado el detalle del cliente en la base de datos del mes actual. La entrada fue eliminada manualmente, o el usuario esta en retiros o desembolsos en ALFIN", null);
                }

                else
                {
                    var clientebcb = clientebcbList.FirstOrDefault();

                    if (clientebcb == null)
                    {
                        return (false, "El cliente no tiene Detalles en la Base de Datos del Banco Alfin, este DNI no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                    }

                    var clientebcbDatos = await _context.base_clientes_banco.Where(bcb => bcb.Dni == DNI).FirstOrDefaultAsync();

                    var dataclientebcb = new DniReferidoData
                    {
                        DNI = clientebcb.Dni,
                        IdBaseCliente = clientebcb.IdBase,
                        TraidoDe = "DBALFIN",
                        NombresCompletos = clientebcb.Nombres + " " + clientebcb.ApellidoPaterno + " " + clientebcb.ApellidoMaterno,
                        OfertaMaxima = clientebcb.OfertaMax
                    };

                    return (true, "El Usuario se ha encontrado en la Base de Datos de ALFIN", dataclientebcb);
                }
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        /// <summary>
        /// Obtiene los datos detallados de un cliente para referirlo, buscando en las bases de datos A365 y ALFIN
        /// </summary>
        /// <param name="dni">DNI del cliente a buscar</param>
        /// <returns>
        /// Una tupla que contiene:
        /// - IsSuccess: Indica si la búsqueda fue exitosa
        /// - Message: Mensaje descriptivo del resultado
        /// - Data: Objeto dinámico con NombresCompletos y OfertaMax del cliente. Null si no se encuentra
        /// </returns>
        /// <remarks>
        /// El método busca secuencialmente en:
        /// 1. Base A365 (base_clientes join detalle_base)
        /// 2. Base ALFIN (base_clientes_banco)
        /// 
        /// Para ALFIN:
        /// - NombresCompletos siempre es "DESCONOCIDO"
        /// - OfertaMax se multiplica por 100
        /// </remarks>
        /// <exception cref="Exception">Se lanza cuando hay error en la consulta a la base de datos</exception>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// var result = await GetDataParaReferir("12345678");
        /// if (result.IsSuccess)
        /// {
        ///     var nombres = result.Data.NombresCompletos;
        ///     var oferta = result.Data.OfertaMax;
        /// }
        /// </code>
        /// </example>
        public async Task<(bool IsSuccess, string Message, dynamic? Data)> GetDataParaReferir(string dni)
        {
            try
            {
                var clienteDetallesA365 = await (from bc in _context.base_clientes
                                                 join db in _context.detalle_base on bc.IdBase equals db.IdBase
                                                 where bc.Dni == dni && bc.IdBaseBanco == null
                                                 orderby db.FechaCarga descending
                                                 select new
                                                 {
                                                     NombresCompletos = bc.XNombre + " " + bc.XAppaterno + " " + bc.XApmaterno,
                                                     OfertaMax = db.OfertaMax,
                                                 }).FirstOrDefaultAsync();
                if (clienteDetallesA365 != null)
                {
                    return (true, "Se ha encontrado el cliente en la base de datos de A365", clienteDetallesA365);
                }

                var clienteDetallesALFIN = await (from bcb in _context.base_clientes_banco
                                                  where bcb.Dni == dni
                                                  orderby bcb.FechaSubida descending
                                                  select new
                                                  {
                                                      NombresCompletos = "DESCONOCIDO",
                                                      OfertaMax = bcb.OfertaMax * 100,
                                                  }).FirstOrDefaultAsync();

                if (clienteDetallesALFIN != null)
                {
                    return (true, "Se ha encontrado el cliente en la base de datos de ALFIN", clienteDetallesALFIN);
                }
                return (false, "No se ha encontrado el cliente en ninguna de las bases de datos. Algo salio mal en la busqueda", null);
            }

            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        /// <summary>
        /// Envía un correo electrónico utilizando el procedimiento almacenado sp_send_dbmail de SQL Server
        /// </summary>
        /// <param name="destinatario">Dirección de correo electrónico del destinatario</param>
        /// <param name="mensaje">Contenido del correo electrónico en formato HTML</param>
        /// <param name="asunto">Asunto del correo electrónico</param>
        /// <returns>
        /// Una tupla que contiene:
        /// - IsSuccess: Indica si el envío fue exitoso
        /// - Message: Mensaje descriptivo del resultado de la operación
        /// </returns>
        /// <remarks>
        /// Este método utiliza el procedimiento almacenado sp_send_dbmail de SQL Server con el perfil 'WyA'
        /// para enviar correos electrónicos. El contenido del mensaje se envía en formato HTML.
        /// </remarks>
        /// <exception cref="Exception">Se lanza cuando ocurre un error al enviar el correo</exception>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// var result = await EnviarCorreoReferido(
        ///     "destinatario@ejemplo.com",
        ///     "<h1>Hola</h1><p>Contenido del mensaje</p>",
        ///     "Asunto del correo"
        /// );
        /// if (result.IsSuccess)
        /// {
        ///     // Correo enviado exitosamente
        /// }
        /// </code>
        /// </example>
        public async Task<(bool IsSuccess, string Message)> EnviarCorreoReferido(string destinatario, string mensaje, string asunto)
        {
            try
            {
                // Aquí va la lógica para enviar el correo
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC msdb.dbo.sp_send_dbmail @profile_name = {0}, @recipients = {1}, @body = {2}, @body_format = {3}, @subject = {4}",
                    "WyA", destinatario, mensaje, "HTML", asunto
                );
                return (true, "El correo ha sido enviado exitosamente");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<ClientesReferidos>? Data)> GetReferidosGeneral()
        {
            try
            {
                var referidos = await _context.clientes_referidos.ToListAsync();
                return (true, "Se han encontrado referidos", referidos);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message)> ModificarEstadoReferido(int idReferido)
        {
            try
            {
                var referido = await _context.clientes_referidos.Where(cr => cr.IdReferido == idReferido ).FirstOrDefaultAsync();
                if (referido == null)
                {
                    return (false, "No se ha encontrado el referido");
                }

                referido.FueProcesado = true;
                referido.EstadoReferencia = "DERIVACION COMPLETADA";
                _context.Update(referido);
                await _context.SaveChangesAsync();
                return (true, "Referido procesado exitosamente");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message, ClientesReferidos? Data)> GetClienteReferidoPorId(int IdReferido)
        {
            try
            {
                var referido = await _context.clientes_referidos.Where(cr => cr.IdReferido == IdReferido).FirstOrDefaultAsync();
                if (referido == null)
                {
                    return (false, "No se ha encontrado el referido", null);
                }

                return (true, "Se encontro el siguiente referido", referido);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<ClientesReferidosDTO>? Data)> GetReferidosDelDNI(string DNI)
        {
            try
            {
                var referidos = await (from cr in _context.clientes_referidos
                                where cr.DniAsesor == DNI
                                    && cr.FechaReferido.HasValue
                                    && cr.FechaReferido.Value.Year == DateTime.Now.Year
                                    && cr.FechaReferido.Value.Month == DateTime.Now.Month
                                select new ClientesReferidosDTO
                                {
                                    IdReferido = cr.IdReferido,
                                    IdBaseClienteA365 = cr.IdBaseClienteA365,
                                    IdBaseClienteBanco = cr.IdBaseClienteBanco,
                                    IdSupervisorReferido = cr.IdSupervisorReferido,
                                    NombreCompletoAsesor = cr.NombreCompletoAsesor,
                                    NombreCompletoCliente = cr.NombreCompletoCliente,
                                    DniAsesor = cr.DniAsesor,
                                    DniCliente = cr.DniCliente,
                                    FechaReferido = cr.FechaReferido,
                                    TraidoDe = cr.TraidoDe,
                                    Telefono = cr.Telefono,
                                    Agencia = cr.Agencia,
                                    FechaVisita = cr.FechaVisita,
                                    OfertaEnviada = cr.OfertaEnviada,
                                    FueProcesado = cr.FueProcesado,
                                    CelularAsesor = cr.CelularAsesor,
                                    CorreoAsesor = cr.CorreoAsesor,
                                    CciAsesor = cr.CciAsesor,
                                    DepartamentoAsesor = cr.DepartamentoAsesor,
                                    UbigeoAsesor = cr.UbigeoAsesor,
                                    BancoAsesor = cr.BancoAsesor,
                                    EstadoReferencia = cr.EstadoReferencia,
                                }).ToListAsync();

                if (referidos == null)
                {
                    return (true, "No se han encontrado referidos para el DNI especificado", new List<ClientesReferidosDTO>());
                }

                foreach (var referido in referidos)
                {
                    var desembolsos = await (from d in _context.desembolsos
                                            where d.DniDesembolso == referido.DniCliente &&
                                                d.FechaDesembolsos != null &&
                                                referido.FechaReferido.HasValue &&
                                                d.FechaDesembolsos.Value.Year == referido.FechaReferido.Value.Year &&
                                                d.FechaDesembolsos.Value.Month == referido.FechaReferido.Value.Month
                                            orderby d.FechaDesembolsos descending
                                            select d).FirstOrDefaultAsync();
                    referido.EstadoDesembolso = desembolsos == null ? "DESEMBOLSADO" : "NO DESEMBOLSADO";
                    referido.FechaReferido = desembolsos?.FechaDesembolsos?.ToLocalTime();
                }
                return (true, "Se han encontrado referidos para el DNI especificado", referidos);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}