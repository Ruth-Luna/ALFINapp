using System.Data;
using System.Text.RegularExpressions;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Services
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
                    "EXEC SP_derivacion_insertar_derivacion_2 @fecha_visita_derivacion, @dni_asesor_derivacion, @DNI_cliente_derivacion, @id_cliente, @nombre_cliente_derivacion, @telefono_derivacion, @agencia_derivacion, @num_agencia",
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
        public async Task<(bool IsSuccess, string Message, List<DerivacionesAsesores>? Data)> GetClientesDerivadosGenerales(List<Usuario> asesores)
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
                var result = await _context.derivaciones_asesores
                    .FromSqlRaw("EXEC sp_Derivacion_consulta_derivaciones_x_asesor_por_dni_REFACTORIZADO @Dni = {0}", 
                        parameter)
                    .ToListAsync();
                return (true, "Derivaciones obtenidas correctamente", result);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<GestionDetalleDTO>? Data)> GetDerivacionInformationAll(List<DerivacionesAsesores> clientes)
        {
            try
            {
                var getAllDnisClientes = clientes.Select(x => x.DniCliente).ToHashSet();
                var getInformationDesem = await _context.desembolsos
                    .Where(x => getAllDnisClientes.Contains(x.DniDesembolso)
                        && x.FechaDesembolsos.HasValue
                        && x.FechaDesembolsos.Value.Year == DateTime.Now.Year
                        && x.FechaDesembolsos.Value.Month == DateTime.Now.Month
                        && x.Sucursal != null
                        && x.DocAsesor != null)
                    .OrderByDescending(x => x.FechaDesembolsos)
                    .ToListAsync();
                var getInformationA365 = await _context.base_clientes
                    .Join(_context.detalle_base, bc => bc.IdBase, db => db.IdBase, (bc, db) => new { bc, db })
                    .Where(x => getAllDnisClientes.Contains(x.bc.Dni)
                        && x.db.FechaCarga.HasValue
                        && x.db.FechaCarga.Value.Year == DateTime.Now.Year
                        && x.db.FechaCarga.Value.Month == DateTime.Now.Month)
                    .OrderByDescending(x => x.db.FechaCarga)
                    .AsNoTracking()
                    .ToListAsync();
                var getInformationAlfin = await _context.base_clientes_banco
                    .Join(_context.base_clientes_banco_campana_grupo,
                        bcb => bcb.IdCampanaGrupoBanco,
                        bcg => bcg.IdCampanaGrupo,
                        (bcb, bcg) => new { bcb, bcg })
                    .Join(_context.base_clientes_banco_color,
                        b => b.bcb.IdColorBanco,
                        bcc => bcc.IdColor,
                        (b, bcc) => new { b.bcb, b.bcg, bcc })
                    .Join(_context.base_clientes_banco_plazo,
                        b => b.bcb.IdPlazoBanco,
                        bcp => bcp.IdPlazo,
                        (b, bcp) => new { b.bcb, b.bcg, b.bcc, bcp })
                    .Join(_context.base_clientes_banco_rango_deuda,
                        b => b.bcb.IdRangoDeuda,
                        bcr => bcr.IdRangoDeuda,
                        (b, bcr) => new { b.bcb, b.bcg, b.bcc, b.bcp, bcr })
                    .Join(_context.base_clientes_banco_usuario,
                        b => b.bcb.IdUsuarioBanco,
                        bcu => bcu.IdUsuario,
                        (b, bcu) => new { b.bcb, b.bcg, b.bcc, b.bcp, b.bcr, bcu })
                    .Where(b => getAllDnisClientes.Contains(b.bcb.Dni)
                        && b.bcb.FechaSubida.HasValue
                        && b.bcb.FechaSubida.Value.Year == DateTime.Now.Year
                        && b.bcb.FechaSubida.Value.Month == DateTime.Now.Month)
                    .OrderByDescending(b => b.bcb.FechaSubida)
                    .AsNoTracking()
                    .ToListAsync();

                var joinInformation = (from cliente in clientes
                                        join desem in getInformationDesem on cliente.DniCliente equals desem.DniDesembolso into desemGroup
                                        from desem in desemGroup.DefaultIfEmpty()
                                        join a365 in getInformationA365 on cliente.DniCliente equals a365.bc.Dni into a365Group
                                        from a365 in a365Group.DefaultIfEmpty()
                                        join alfin in getInformationAlfin on cliente.DniCliente equals alfin.bcb.Dni into alfinGroup
                                        from alfin in alfinGroup.DefaultIfEmpty()
                                        let ofertaA365 = a365?.db.OfertaMax ?? 0
                                        let ofertaAlfin = alfin?.bcb.OfertaMax ?? 0
                                        select new GestionDetalleDTO
                                        {
                                            ArchivoOrigen = a365 != null ? "A365" : alfin != null ? "ALFIN" : "NO SE ENCONTRO",
                                            Canal = a365 != null ? a365.db.Canal : " ",
                                            CodCampaña = a365 != null ? a365.db.Campaña : alfin?.bcg.NombreCampana ?? "NO SE ENCONTRO CAMPAÑA",
                                            Oferta = ofertaA365 > 0 ? ofertaA365 : ofertaAlfin,
                                            CodCanal = a365?.db.Canal ?? " ",
                                            DocAsesor = cliente.DniAsesor,
                                            DocCliente = cliente.DniCliente,
                                            EstadoDerivacion = cliente.EstadoDerivacion,
                                            FechaDerivacion = cliente.FechaDerivacion,
                                            FechaEnvio = cliente.FechaVisita ?? DateTime.MinValue,
                                            FechaGestion = cliente.FechaDerivacion,
                                            FueProcesadaLaDerivacion = cliente.FueProcesado,
                                            IdAsignacion = cliente.IdAsignacion,
                                            IdDerivacion = cliente.IdDerivacion,
                                            FechaCarga = a365?.db.FechaCarga ?? alfin?.bcb.FechaSubida ?? DateTime.MinValue,
                                            IdSupervisor = 0,
                                            Supervisor = "NO SE ENCONTRO SUPERVISOR ENCARGADO",
                                            IdDesembolso = desem?.IdDesembolsos ?? 0,
                                            FechaDesembolso = desem?.FechaDesembolsos,
                                            FueDesembolsado = desem != null,
                                            EstadoDesembolso = desem != null ? "DESEMBOLSADO" : "NO DESEMBOLSADO",
                                            Observacion = "LA INFORMACION TRAIDA ES DE SISTEMA INTERNO",
                                            NombreCompletoCliente = cliente.NombreCliente,
                                            Telefono = cliente.TelefonoCliente,
                                            OrigenTelefono = "A365",
                                            DocAsesorDesembolso = desem != null ? desem.DocAsesor != null ? desem.DocAsesor : "NO SE ENCONTRO": "NO DESEMBOLSADO",
                                        }).ToList();
                foreach (var item in joinInformation)
                {
                    if (item.DocAsesor != item.DocAsesorDesembolso && item.FueDesembolsado == true)
                    {
                        item.IdDesembolso = 0;
                        item.FechaDesembolso = null;
                        item.FueDesembolsado = false;
                        item.EstadoDesembolso = $"NO DESEMBOLSADO";
                    }
                }

                joinInformation = joinInformation.OrderBy(x => x.FechaDerivacion).ToList();
                joinInformation = joinInformation
                    .GroupBy(x => x.DocCliente)
                    .Select(g => g.First())
                    .ToList();

                return (true, "Información de las derivaciones obtenida correctamente", joinInformation);
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