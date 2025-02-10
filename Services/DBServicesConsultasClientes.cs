using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
                var clienteExistenteBD = await _context.base_clientes.FirstOrDefaultAsync(c => c.Dni == DNIBusqueda && c.IdBaseBanco == null);
                if (clienteExistenteBD != null)
                {
                    var detalleclienteExistenteBD = _context.detalle_base
                        .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_por_idbase @IdBase",
                            new SqlParameter("@IdBase", clienteExistenteBD.IdBase))
                        .AsEnumerable()
                        .FirstOrDefault();

                    if (detalleclienteExistenteBD == null)
                    {
                        return (false, "El cliente no tiene Detalle Base en la Base de Datos de A365, este dato fue eliminado manualmente, o es previo a la fecha correspondiente", null);
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
                        Deuda1 = $"{detalleclienteExistenteBD.Deuda1} - {detalleclienteExistenteBD.Deuda2} - {detalleclienteExistenteBD.Deuda3}",
                        Entidad1 = $"{detalleclienteExistenteBD.Entidad1} - {detalleclienteExistenteBD.Entidad2} - {detalleclienteExistenteBD.Entidad3}",
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
                        IdBase = detalleclienteExistenteBD.IdBase,
                        ApellidoPaterno = clienteExistenteBD.XAppaterno,
                        ApellidoMaterno = clienteExistenteBD.XApmaterno,
                        Nombres = clienteExistenteBD.XNombre,
                        UserV3 = detalleclienteExistenteBD.UserV3,
                        FlagDeudaVOferta = detalleclienteExistenteBD.FlagDeudaVOferta
                    };
                    // El DNI se encuentra registrado en la Base de Datos de A365
                    return (true, "El DNI se encuentra registrado en la Base de Datos de A365. Se devolvera la entrada correspondiente", clienteA365Encontrado); // Se devuelve la entrada correspondiente
                }

                // Consulta a la base de datos del banco de clientes
                var testingDBBank = await _context.base_clientes_banco.FirstOrDefaultAsync(c => c.Dni == DNIBusqueda);
                if (testingDBBank == null)
                {
                    return (false, "El cliente no tiene Detalles en la Base de Datos del Banco Alfin, este DNI no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                }
                var clienteExistenteBank = _context.detalles_clientes_dto
                    .FromSqlRaw("EXEC SP_Consulta_Obtener_Cliente_Banco_Alfin @DNIBusqueda",
                        new SqlParameter("@DNIBusqueda", DNIBusqueda))
                    .AsEnumerable()
                    .FirstOrDefault();

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

        public async Task<(bool IsSuccess, string message, DetallesClienteDTO? Data)> GetClienteByTelefono(string telefono)
        {
            try
            {
                var clienteExistenteBD = await _context.clientes_enriquecidos.FirstOrDefaultAsync(c => c.Telefono1 == telefono
                    || c.Telefono2 == telefono
                    || c.Telefono3 == telefono
                    || c.Telefono4 == telefono
                    || c.Telefono5 == telefono);

                if (clienteExistenteBD == null)
                {
                    return (false, "El cliente no tiene Detalles en la Base de Datos de A365, este TELEFONO no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                }

                var detalleclienteExistenteBD = _context.detalle_base
                    .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_por_id_enriquecido @idEnriquecido",
                        new SqlParameter("@idEnriquecido", clienteExistenteBD.IdCliente))
                    .AsEnumerable()
                    .FirstOrDefault();

                if (detalleclienteExistenteBD == null)
                {
                    return (false, "El cliente no tiene Detalle Base en la Base de Datos de A365, este dato fue eliminado manualmente", null);
                }

                var baseClienteExistenteBD = await _context.base_clientes.FirstOrDefaultAsync(c => c.IdBase == clienteExistenteBD.IdBase);

                if (baseClienteExistenteBD == null)
                {
                    return (false, "El cliente no tiene Detalles en la Base de Datos de A365, este TELEFONO no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                }

                var clienteA365Encontrado = new DetallesClienteDTO
                {
                    Dni = baseClienteExistenteBD.Dni,
                    ColorFinal = detalleclienteExistenteBD.ColorFinal,
                    Color = detalleclienteExistenteBD.Color,
                    Campaña = detalleclienteExistenteBD.Campaña,
                    OfertaMax = detalleclienteExistenteBD.OfertaMax,
                    Plazo = detalleclienteExistenteBD.Plazo,
                    CapacidadMax = detalleclienteExistenteBD.CapacidadMax,
                    SaldoDiferencialReeng = detalleclienteExistenteBD.SaldoDiferencialReeng,
                    ClienteNuevo = detalleclienteExistenteBD.ClienteNuevo,
                    Deuda1 = $"{detalleclienteExistenteBD.Deuda1} - {detalleclienteExistenteBD.Deuda2} - {detalleclienteExistenteBD.Deuda3}",
                    Entidad1 = $"{detalleclienteExistenteBD.Entidad1} - {detalleclienteExistenteBD.Entidad2} - {detalleclienteExistenteBD.Entidad3}",
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
                    IdBase = detalleclienteExistenteBD.IdBase,
                    ApellidoPaterno = baseClienteExistenteBD.XAppaterno,
                    ApellidoMaterno = baseClienteExistenteBD.XApmaterno,
                    Nombres = baseClienteExistenteBD.XNombre
                };
                return (true, "El cliente fue encontrado en la base de Datos de A365", clienteA365Encontrado); // Se devuelve la entrada correspondiente
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}