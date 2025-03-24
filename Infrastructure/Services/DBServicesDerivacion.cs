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

        public async Task<(bool IsSuccess, string Message)> ObservarDerivacion(int id, string observacion)
        {
            try
            {
                var derivacion = await _context.derivaciones_asesores.FirstOrDefaultAsync(x => x.IdDerivacion == id);
                if (derivacion == null)
                {
                    return (false, "No se encontró la derivación");
                }
                derivacion.ObservacionDerivacion = observacion;
                _context.derivaciones_asesores.Update(derivacion);
                await _context.SaveChangesAsync();
                return (true, "Observación guardada correctamente");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message, GestionDetalleDTO? data)> GetDerivacionInformation(DerivacionesAsesores derivacion)
        {
            try
            {
                var asesorInfo = await _context.usuarios.FirstOrDefaultAsync(x => x.Dni == derivacion.DniAsesor);
                var desembolsoInfo = await _context.desembolsos
                    .Where(x => x.DniDesembolso == derivacion.DniCliente
                        && x.FechaProporcion.HasValue
                        && x.FechaProporcion.Value.Month == derivacion.FechaDerivacion.Month
                        && x.FechaProporcion.Value.Year == derivacion.FechaDerivacion.Year)
                    .OrderByDescending(x => x.FechaSol)
                    .FirstOrDefaultAsync();
                if (derivacion.IdAsignacion == null)
                {
                    var derivacionInfoA365 = await (from bc in _context.base_clientes
                                                    join db in _context.detalle_base on bc.IdBase equals db.IdBase
                                                    where bc.Dni == derivacion.DniCliente
                                                        && bc.IdBaseBanco == null
                                                        && db.FechaCarga.HasValue
                                                        && db.FechaCarga.Value.Month == derivacion.FechaDerivacion.Month
                                                        && db.FechaCarga.Value.Year == derivacion.FechaDerivacion.Year
                                                    orderby db.FechaCarga descending
                                                    select new
                                                    {
                                                        bc,
                                                        db
                                                    }).FirstOrDefaultAsync();
                    if (derivacionInfoA365 != null)
                    {
                        var nuevaGestionDetalle = new GestionDetalleDTO
                        {
                            CodCampaña = derivacionInfoA365.db.Campaña,
                            Oferta = derivacionInfoA365.db.OfertaMax != null ? derivacionInfoA365.db.OfertaMax.Value : 0,
                            CodCanal = derivacionInfoA365.db.Canal,
                            FechaCarga = derivacionInfoA365.db.FechaCarga != null ? derivacionInfoA365.db.FechaCarga.Value : DateTime.MinValue,
                            IdSupervisor = asesorInfo != null ? asesorInfo.IDUSUARIOSUP : 0,
                            Supervisor = asesorInfo != null ? asesorInfo.RESPONSABLESUP : "NO SE ENCONTRO SUPERVISOR ENCARGADO",
                            IdDesembolso = desembolsoInfo != null ? desembolsoInfo.IdDesembolsos : 0,
                            FechaDesembolso = desembolsoInfo != null ? desembolsoInfo.FechaDesembolsos : null,
                            FueDesembolsado = desembolsoInfo != null ? true : false,
                            EstadoDesembolso = desembolsoInfo != null ? "DESEMBOLSADO" : "NO DESEMBOLSADO",
                            Observacion = "LA INFORMACION TRAIDA ES DE A365/SISTEMA INTERNO Y NO DEL BANCO",
                        };
                        return (true, "Informacion de la derivacion encontrada mas actual traida de A365", nuevaGestionDetalle);
                    }
                    var derivacionAlfin = await (
                        from bcb in _context.base_clientes_banco
                        join bcg in _context.base_clientes_banco_campana_grupo on bcb.IdCampanaGrupoBanco equals bcg.IdCampanaGrupo into bcgGroup
                        from bcg in bcgGroup.DefaultIfEmpty()
                        join bcc in _context.base_clientes_banco_color on bcb.IdColorBanco equals bcc.IdColor into bccGroup
                        from bcc in bccGroup.DefaultIfEmpty()
                        join bcp in _context.base_clientes_banco_plazo on bcb.IdPlazoBanco equals bcp.IdPlazo into bcpGroup
                        from bcp in bcpGroup.DefaultIfEmpty()
                        join bcr in _context.base_clientes_banco_rango_deuda on bcb.IdRangoDeuda equals bcr.IdRangoDeuda into bcrGroup
                        from bcr in bcrGroup.DefaultIfEmpty()
                        join bcu in _context.base_clientes_banco_usuario on bcb.IdUsuarioBanco equals bcu.IdUsuario into bcuGroup
                        from bcu in bcuGroup.DefaultIfEmpty()
                        where bcb.Dni == derivacion.DniCliente
                            && bcb.FechaSubida.HasValue
                            && bcb.FechaSubida.Value.Month == derivacion.FechaDerivacion.Month
                            && bcb.FechaSubida.Value.Year == derivacion.FechaDerivacion.Year
                        orderby bcb.FechaSubida descending
                        select new
                        {
                            bcb,
                            bcg,
                            bcc,
                            bcp,
                            bcr,
                            bcu
                        }).FirstOrDefaultAsync();
                    if (derivacionAlfin != null)
                    {
                        var nuevaGestionDetalle = new GestionDetalleDTO
                        {
                            CodCampaña = derivacionAlfin.bcg != null ? derivacionAlfin.bcg.NombreCampana : "NO SE ENCONTRO CAMPAÑA",
                            Oferta = derivacionAlfin.bcb.OfertaMax != null ? derivacionAlfin.bcb.OfertaMax.Value : 0,
                            CodCanal = " ",
                            FechaCarga = derivacionAlfin.bcb.FechaSubida != null ? derivacionAlfin.bcb.FechaSubida.Value : DateTime.MinValue,
                            IdSupervisor = asesorInfo != null ? asesorInfo.IDUSUARIOSUP : 0,
                            Supervisor = asesorInfo != null ? asesorInfo.RESPONSABLESUP : "NO SE ENCONTRO SUPERVISOR ENCARGADO",
                            IdDesembolso = desembolsoInfo != null ? desembolsoInfo.IdDesembolsos : 0,
                            FechaDesembolso = desembolsoInfo != null ? desembolsoInfo.FechaDesembolsos : null,
                            FueDesembolsado = desembolsoInfo != null ? true : false,
                            EstadoDesembolso = desembolsoInfo != null ? "DESEMBOLSADO" : "NO DESEMBOLSADO",
                            Observacion = "LA INFORMACION TRAIDA ES DE ALFIN/SISTEMA INTERNO Y NO DEL BANCO",
                        };
                        return (true, "Información de la derivación encontrada mas actual traida de ALFIN", nuevaGestionDetalle);
                    }
                    return (true, "No se encontró información de la derivación", new GestionDetalleDTO());
                }

                var derivacionInfo = await (
                    from ca in _context.clientes_asignados
                    join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                    join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                    join db in _context.detalle_base on bc.IdBase equals db.IdBase
                    where ca.IdAsignacion == derivacion.IdAsignacion
                        && ca.FuenteBase == db.TipoBase
                    select new GestionDetalleDTO
                    {
                        CodCampaña = db.Campaña,
                        Oferta = db.OfertaMax != null ? db.OfertaMax.Value : 0,
                        CodCanal = db.Canal,
                        FechaCarga = db.FechaCarga != null ? db.FechaCarga.Value : DateTime.MinValue,
                        IdSupervisor = asesorInfo != null ? asesorInfo.IDUSUARIOSUP : 0,
                        Supervisor = asesorInfo != null ? asesorInfo.RESPONSABLESUP : "NO SE ENCONTRO SUPERVISOR ENCARGADO",
                        IdDesembolso = desembolsoInfo != null ? desembolsoInfo.IdDesembolsos : 0,
                        FechaDesembolso = desembolsoInfo != null ? desembolsoInfo.FechaDesembolsos : null,
                        FueDesembolsado = desembolsoInfo != null ? true : false,
                        EstadoDesembolso = desembolsoInfo != null ? "DESEMBOLSADO" : "NO DESEMBOLSADO",
                        Observacion = "LA INFORMACION TRAIDA ES DE A365/SISTEMA INTERNO Y NO DEL BANCO",
                    }).FirstOrDefaultAsync();

                if (derivacionInfo != null)
                {
                    return (true, "Informacion de la derivacion encontrada mas actual traida de A365", derivacionInfo);
                }

                var derivacionInfoAlfin = await (
                    from ca in _context.clientes_asignados
                    join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                    join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                    join bcb in _context.base_clientes_banco on bc.IdBaseBanco equals bcb.IdBaseBanco into bcbGroup
                    from bcb in bcbGroup.DefaultIfEmpty()
                    join bcg in _context.base_clientes_banco_campana_grupo on bcb.IdCampanaGrupoBanco equals bcg.IdCampanaGrupo into bcgGroup
                    from bcg in bcgGroup.DefaultIfEmpty()
                    join bcc in _context.base_clientes_banco_color on bcb.IdColorBanco equals bcc.IdColor into bccGroup
                    from bcc in bccGroup.DefaultIfEmpty()
                    join bcp in _context.base_clientes_banco_plazo on bcb.IdPlazoBanco equals bcp.IdPlazo into bcpGroup
                    from bcp in bcpGroup.DefaultIfEmpty()
                    join bcr in _context.base_clientes_banco_rango_deuda on bcb.IdRangoDeuda equals bcr.IdRangoDeuda into bcrGroup
                    from bcr in bcrGroup.DefaultIfEmpty()
                    join bcu in _context.base_clientes_banco_usuario on bcb.IdUsuarioBanco equals bcu.IdUsuario into bcuGroup
                    from bcu in bcuGroup.DefaultIfEmpty()
                    where ca.IdAsignacion == derivacion.IdAsignacion
                        && bcb.FechaSubida.HasValue
                        && bcb.FechaSubida.Value.Month == derivacion.FechaDerivacion.Month
                        && bcb.FechaSubida.Value.Year == derivacion.FechaDerivacion.Year
                    orderby bcb.FechaSubida descending
                    select new GestionDetalleDTO
                    {
                        CodCampaña = bcg != null ? bcg.NombreCampana : "NO SE ENCONTRO CAMPAÑA",
                        Oferta = bcb.OfertaMax != null ? bcb.OfertaMax.Value : 0,
                        CodCanal = " ",
                        FechaCarga = bcb.FechaSubida != null ? bcb.FechaSubida.Value : DateTime.MinValue,
                        IdSupervisor = asesorInfo != null ? asesorInfo.IDUSUARIOSUP : 0,
                        Supervisor = asesorInfo != null ? asesorInfo.RESPONSABLESUP : "NO SE ENCONTRO SUPERVISOR ENCARGADO",
                        IdDesembolso = desembolsoInfo != null ? desembolsoInfo.IdDesembolsos : 0,
                        FechaDesembolso = desembolsoInfo != null ? desembolsoInfo.FechaDesembolsos : null,
                        FueDesembolsado = desembolsoInfo != null ? true : false,
                        EstadoDesembolso = desembolsoInfo != null ? "DESEMBOLSADO" : "NO DESEMBOLSADO",
                        Observacion = "LA INFORMACION TRAIDA ES DE ALFIN/SISTEMA INTERNO Y NO DEL BANCO",
                    }).FirstOrDefaultAsync();
                if (derivacionInfoAlfin != null)
                {
                    return (true, "Informacion de la derivacion encontrada mas actual traida de A365", derivacionInfoAlfin);
                }
                return (true, "No se encontró información de la derivación, se pasaran entradas vacias", new GestionDetalleDTO());
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string message)> EnviarEmailDeDerivacion(string destinatario_agencia_codigo, string mensaje, string asunto)
        {
            try
            {
                string destinatario_codigo = Regex.Match(destinatario_agencia_codigo, @"^\d+").Value;
                destinatario_codigo = "73" + destinatario_codigo;
                var destinatario_principal = await _context.directorio_comercial
                    .Where(x => x.Ceco == destinatario_codigo)
                    .Select(x => new { x.CorreoAgencia, x.CorreoGerente })
                    .FirstOrDefaultAsync();

                if (destinatario_principal == null)
                {
                    return (false, "No se encontró el destinatario principal, seleccione una agencia valida");
                }

                var correos = await _context.correos.Where(x => x.Envio == "derivaciones" && !x.EsCorreoPropio).ToListAsync();
                var destinatarios = string.Join("; ", correos.Select(x => x.Correo));

                destinatarios = string.Join("; ", destinatarios, destinatario_principal.CorreoGerente);

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC msdb.dbo.sp_send_dbmail @profile_name = {0}, @recipients = {1}, @copy_recipients = {2}, @body = {3}, @body_format = {4}, @subject = {5}",
                    new SqlParameter("@profile_name", "RoxanaAlfin"),
                    new SqlParameter("@recipients", destinatario_principal.CorreoAgencia),
                    new SqlParameter("@copy_recipients", destinatarios),
                    new SqlParameter("@body", mensaje),
                    new SqlParameter("@body_format", "HTML"),
                    new SqlParameter("@subject", asunto)
                );
                return (true, "El correo ha sido enviado exitosamente");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string message, GestionDetalleDTO? data)> GetDerivacionXDNI(string dni)
        {
            try
            {
                var getDerivacion = await (from da in _context.derivaciones_asesores
                                           join ce in _context.clientes_enriquecidos on da.IdCliente equals ce.IdCliente into ceGroup
                                           from ce in ceGroup.DefaultIfEmpty()
                                           join bc in _context.base_clientes on ce.IdBase equals bc.IdBase into bcGroup
                                           from bc in bcGroup.DefaultIfEmpty()
                                           join db in _context.detalle_base on bc.IdBase equals db.IdBase into dbGroup
                                           from db in dbGroup.DefaultIfEmpty()
                                           where da.DniCliente == dni
                                           orderby da.FechaDerivacion descending, db.FechaCarga descending
                                           select new GestionDetalleDTO
                                           {
                                               CodCampaña = db.Campaña,
                                               Oferta = db.OfertaMax != null ? db.OfertaMax.Value : 0,
                                               CodCanal = db.Canal,
                                           }).FirstOrDefaultAsync();
                if (getDerivacion == null)
                {
                    return (false, "No se encontró la derivación", null);
                }
                return (true, "Derivación encontrada", getDerivacion);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}