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
                int tiempoMaximoEspera = 40000; // 30 segundos
                int intervaloEspera = 1000; // 1 segundo
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

                return (false, "Tiempo de espera agotado. La entrada no fue procesada. Pero fue guardada correctamente en nuestro sistema no sera necesario que envie mas derivaciones de este cliente en caso su rol sea Asesor. Su derivacion sera procesada muy pronto. Para conocer el estado de su derivacion puede dirigirse a la pestaña de Derivaciones");
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
    }
}