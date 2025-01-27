using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Services
{
    public class DBServicesConsultasClientes
    {
        private readonly MDbContext _context;

        public DBServicesConsultasClientes(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string message, DetallesClienteDTO? Data)> GetClientsFromDBandBank(string DNIBusqueda)
        {
            try
            {
                var clienteExistenteBD = await _context.base_clientes.FirstOrDefaultAsync(c => c.Dni == DNIBusqueda);
                if (clienteExistenteBD != null)
                {
                    var detalleclienteExistenteBD = await _context.detalle_base.FirstOrDefaultAsync(c => c.IdBase == clienteExistenteBD.IdBase);
                    if (detalleclienteExistenteBD == null)
                    {
                        return (false, "El cliente no tiene Detalle Base en la Base de Datos de A365, este dato fue eliminado manualmente, este error debe de ser reportado", null);
                    }

                    // Consulta a la base de datos del A365
                    var clienteA365Encontrado = new DetallesClienteDTO
                    {
                        Dni = clienteExistenteBD.Dni,
                        ColorFinal = detalleclienteExistenteBD.ColorFinal,
                        Color = detalleclienteExistenteBD.Color,
                        Campaña = detalleclienteExistenteBD.Campaña,
                        OfertaMax = detalleclienteExistenteBD.OfertaMax,
                        Plazo = detalleclienteExistenteBD.Plazo,
                        CapacidadMax = detalleclienteExistenteBD.CapacidadMax,
                        SaldoDiferencialReeng = detalleclienteExistenteBD.SaldoDiferencialReeng,
                        ClienteNuevo = detalleclienteExistenteBD.ClienteNuevo,
                        Deuda1 = $"{detalleclienteExistenteBD.Deuda1}{detalleclienteExistenteBD.Deuda2}{detalleclienteExistenteBD.Deuda3}",
                        Entidad1 = detalleclienteExistenteBD.Entidad1,
                        Tasa1 = detalleclienteExistenteBD.Tasa1,
                        Tasa2 = detalleclienteExistenteBD.Tasa2,
                        Tasa3 = detalleclienteExistenteBD.Tasa3,
                        Tasa4 = detalleclienteExistenteBD.Tasa4,
                        Tasa5 = detalleclienteExistenteBD.Tasa5,
                        Tasa6 = detalleclienteExistenteBD.Tasa6,
                        Tasa7 = detalleclienteExistenteBD.Tasa7,
                        GrupoTasa = detalleclienteExistenteBD.GrupoTasa,
                        Usuario = detalleclienteExistenteBD.Usuario,
                        SegmentoUser = detalleclienteExistenteBD.SegmentoUser,
                        TraidoDe = "BDA365",
                        IdBase = detalleclienteExistenteBD.IdBase
                    };
                    // El DNI se encuentra registrado en la Base de Datos de A365
                    return (true, "El DNI se encuentra registrado en la Base de Datos de A365. Se devolvera la entrada correspondiente", clienteA365Encontrado); // Se devuelve la entrada correspondiente
                }

                // Consulta a la base de datos del banco de clientes
                var testingDBBank = await _context.base_clientes_banco.FirstOrDefaultAsync(c => c.Dni == DNIBusqueda);
                var clienteExistenteBank = await(
                                                    from bcb in _context.base_clientes_banco
                                                    join pb in _context.base_clientes_banco_plazo on bcb.IdPlazoBanco equals pb.IdPlazo into PlazoGrupo
                                                    from pb in PlazoGrupo.DefaultIfEmpty() // Left Join con base_clientes_banco_plazo
                                                    join cg in _context.base_clientes_banco_campana_grupo on bcb.IdCampanaGrupoBanco equals cg.IdCampanaGrupo into CampanaGrupo
                                                    from cg in CampanaGrupo.DefaultIfEmpty() // Left Join con base_clientes_banco_campana_grupo
                                                    join c in _context.base_clientes_banco_color on bcb.IdColorBanco equals c.IdColor into ColorGrupo
                                                    from c in ColorGrupo.DefaultIfEmpty() // Left Join con base_clientes_banco_color
                                                    join u in _context.base_clientes_banco_usuario on bcb.IdUsuarioBanco equals u.IdUsuario into UsuarioGrupo
                                                    from u in UsuarioGrupo.DefaultIfEmpty() // Left Join con base_clientes_banco_usuario
                                                    join rd in _context.base_clientes_banco_rango_deuda on bcb.IdRangoDeuda equals rd.IdRangoDeuda into RangoDeudaGrupo
                                                    from rd in RangoDeudaGrupo.DefaultIfEmpty() 
                                                    where bcb.Dni == DNIBusqueda
                                                    select new DetallesClienteDTO
                                                    {
                                                        Dni = bcb.Dni,
                                                        ColorFinal = c.NombreColor,
                                                        Campaña = cg.NombreCampana,
                                                        OfertaMax = bcb.OfertaMax,
                                                        Plazo = pb.NumMeses,
                                                        CapacidadMax = bcb.CapacidadPagoMen,
                                                        SaldoDiferencialReeng = bcb.Reenganche,
                                                        ClienteNuevo = bcb.Frescura == true ? "SI" : "NO",
                                                        Deuda1 = rd.RangoDeDeuda, //
                                                        Entidad1 = bcb.NumEntidades.ToString(),
                                                        Tasa1 = bcb.Tasa1,
                                                        Tasa2 = bcb.Tasa2,
                                                        Tasa3 = bcb.Tasa3,
                                                        Tasa4 = bcb.Tasa4,
                                                        Tasa5 = bcb.Tasa5,
                                                        Tasa6 = bcb.Tasa6,
                                                        Tasa7 = bcb.Tasa7,
                                                        GrupoTasa = null, //
                                                        Usuario = u.NombreUsuario,
                                                        SegmentoUser = null,
                                                        TraidoDe = "BDALFIN",
                                                        IdBase = bcb.IdBaseBanco
                                                    }).FirstOrDefaultAsync();

                if (clienteExistenteBank == null)
                {
                    return (false, "El cliente no tiene Detalles en la Base de Datos del Banco Alfin, este DNI no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                }
                return (true, "El cliente fue encontrado en la base de Datos del Banco Alfin", clienteExistenteBank); // Se devuelve la entrada correspondiente
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}