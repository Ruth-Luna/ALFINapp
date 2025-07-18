using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Services
{
    /// <summary>
    /// Service class that provides database operations for client query operations in the ALFINapp system.
    /// Handles retrieval of client details for typification from different data sources (A365 and ALFIN).
    /// </summary>
    public class DBServicesConsultasClientes
    {
        private readonly MDbContext _context;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DBServicesConsultasClientes"/> class.
        /// </summary>
        /// <param name="context">The database context used for database operations.</param>
        public DBServicesConsultasClientes(MDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Retrieves detailed client information from the A365 database for typification purposes.
        /// </summary>
        /// <param name="IdBase">Base ID of the client record to retrieve.</param>
        /// <param name="IdUsuarioV">ID of the user (advisor) retrieving the information.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the operation was successful
        /// - message: Descriptive message about the result
        /// - Data: DetalleTipificarClienteDTO object with client details if successful, otherwise null
        /// </returns>
        /// <remarks>
        /// Uses the stored procedure SP_Consulta_Obtener_detalle_cliente_para_tipificar_A365 to retrieve client data.
        /// Maps all client information including personal data, contact details, and financial information to a DTO.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string message, DetalleTipificarClienteDTO? Data)> GetDataParaTipificarClienteA365(int IdBase, int IdUsuarioV)
        {
            try
            {
                var detalleClienteConsulta = await _context.detalle_cliente_a365_tipificar_dto
                    .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_para_tipificar_A365_refactorizado @IdBase, @IdUsuarioV",
                        new SqlParameter("@IdBase", IdBase),
                        new SqlParameter("@IdUsuarioV", IdUsuarioV))
                    .ToListAsync();

                if (detalleClienteConsulta.Count == 0)
                {
                    return (false, "No se encontraron datos para tipificar el cliente", null);
                }

                var data = detalleClienteConsulta.FirstOrDefault();

                if (data == null)
                {
                    return (false, "No se encontraron datos para tipificar el cliente", null);
                }

                var detalleTipificar = new DetalleTipificarClienteDTO
                {
                    // Propiedades de BaseCliente
                    Dni = data.Dni,
                    XAppaterno = data.XAppaterno,
                    XApmaterno = data.XApmaterno,
                    XNombre = data.XNombre,
                    Edad = data.Edad,
                    Departamento = data.Departamento,
                    Provincia = data.Provincia,
                    Distrito = data.Distrito,
                    IdBase = data.IdBase,

                    // Propiedades de DetalleBase
                    Campaña = data.Campaña,
                    OfertaMax = data.OfertaMax,
                    TasaMinima = data.TasaMinima,
                    Sucursal = data.Sucursal,
                    AgenciaComercial = data.AgenciaComercial,
                    Plazo = data.Plazo,
                    Cuota = data.Cuota,
                    GrupoTasa = data.GrupoTasa,
                    GrupoMonto = data.GrupoMonto,
                    Propension = data.Propension,
                    TipoCliente = data.TipoCliente,
                    ClienteNuevo = data.ClienteNuevo,
                    Color = data.Color,
                    ColorFinal = data.ColorFinal,
                    PerfilRo = data.PerfilRo,

                    // Propiedades de ClientesEnriquecido
                    IdCliente = data.IdCliente,

                    Telefonos = new List<TelefonoDTO>
                    {
                        new TelefonoDTO
                        {
                            Numero = data.Telefono1,
                            Comentario = data.ComentarioTelefono1,
                            DescripcionTipificacion = data.UltimaTipificacionTelefono1,
                            FechaTipificacion = data.FechaUltimaTipificacionTelefono1
                        },
                        new TelefonoDTO
                        {
                            Numero = data.Telefono2,
                            Comentario = data.ComentarioTelefono2,
                            DescripcionTipificacion = data.UltimaTipificacionTelefono2,
                            FechaTipificacion = data.FechaUltimaTipificacionTelefono2
                        },
                        new TelefonoDTO
                        {
                            Numero = data.Telefono3,
                            Comentario = data.ComentarioTelefono3,
                            DescripcionTipificacion = data.UltimaTipificacionTelefono3,
                            FechaTipificacion = data.FechaUltimaTipificacionTelefono3
                        },
                        new TelefonoDTO
                        {
                            Numero = data.Telefono4,
                            Comentario = data.ComentarioTelefono4,
                            DescripcionTipificacion = data.UltimaTipificacionTelefono4,
                            FechaTipificacion = data.FechaUltimaTipificacionTelefono4
                        },
                        new TelefonoDTO
                        {
                            Numero = data.Telefono5,
                            Comentario = data.ComentarioTelefono5,
                            DescripcionTipificacion = data.UltimaTipificacionTelefono5,
                            FechaTipificacion = data.FechaUltimaTipificacionTelefono5
                        },
                    },
                    //Propiedades Tasas y Detalles
                    Oferta12m = data.Oferta12m,
                    Tasa12m = data.Tasa12m,
                    Cuota12m = data.Cuota12m,
                    Oferta18m = data.Oferta18m,
                    Tasa18m = data.Tasa18m,
                    Cuota18m = data.Cuota18m,
                    Oferta24m = data.Oferta24m,
                    Tasa24m = data.Tasa24m,
                    Cuota24m = data.Cuota24m,
                    Oferta36m = data.Oferta36m,
                    Tasa36m = data.Tasa36m,
                    Cuota36m = data.Cuota36m,
                    Usuario = data.Usuario,
                    SegmentoUser = data.SegmentoUser,

                    //Propiedades de ClientesAsignados
                    IdAsignacion = data.IdAsignacion,
                    FuenteBase = data.FuenteBase
                };

                return (true, "Datos para tipificar el cliente encontrados", detalleTipificar);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        /// <summary>
        /// Retrieves manually added phone numbers for a specific client.
        /// </summary>
        /// <param name="IdCliente">ID of the client to retrieve phone numbers for.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the operation was successful
        /// - message: Descriptive message about the result
        /// - Data: List of TelefonosAgregadosDTO objects with phone details if found, otherwise null
        /// </returns>
        /// <remarks>
        /// Queries the telefonos_agregados table to find records associated with the specified client.
        /// Each phone record includes the number, comments, last typification, and typification date.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string message, List<TelefonosAgregadosDTO>? Data)> GetTelefonosTraidosManualmente(int IdCliente)
        {
            try
            {
                var telefonos = await (from ta in _context.telefonos_agregados
                                       where ta.IdCliente == IdCliente
                                       select new TelefonosAgregadosDTO
                                       {
                                           TelefonoTipificado = ta.Telefono,
                                           ComentarioTelefono = ta.Comentario,
                                           DescripcionTipificacion = ta.UltimaTipificacion,
                                           FechaTipificacionSup = ta.FechaUltimaTipificacion
                                       }).ToListAsync();
                if (telefonos.Count == 0)
                {
                    return (true, "No se encontraron telefonos agregados manualmente", null);
                }
                return (true, "Telefonos agregados manualmente encontrados", telefonos);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves detailed client information from the ALFIN database for typification purposes.
        /// </summary>
        /// <param name="IdBase">Base ID of the client record to retrieve.</param>
        /// <param name="usuarioId">ID of the user (advisor) retrieving the information.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the operation was successful
        /// - message: Descriptive message about the result
        /// - Data: DetalleTipificarClienteDTO object with client details if successful, otherwise null
        /// </returns>
        /// <remarks>
        /// Performs a complex join operation across multiple tables in the ALFIN database:
        /// - base_clientes
        /// - base_clientes_banco
        /// - base_clientes_banco_campana_grupo
        /// - base_clientes_banco_color
        /// - base_clientes_banco_plazo
        /// - base_clientes_banco_rango_deuda
        /// - base_clientes_banco_usuario
        /// - clientes_enriquecidos
        /// - clientes_asignados
        /// 
        /// Maps financial offers with different terms (12m, 18m, 24m, 36m) and rates.
        /// Multiplies the offer amount by 100 to standardize with A365 data.
        /// Uses default "DESCONOCIDO" values for fields not available in the ALFIN database.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string message, DetalleTipificarClienteDTO? Data)> GetDataParaTipificarClienteAlfin(int IdBase, int usuarioId)
        {
            try
            {
                var detallesClientes = await (from bc in _context.base_clientes
                                              join bcb in _context.base_clientes_banco on bc.IdBaseBanco equals bcb.IdBaseBanco
                                              join bcg in _context.base_clientes_banco_campana_grupo on bcb.IdCampanaGrupoBanco equals bcg.IdCampanaGrupo into bcgGroup
                                              from bcg in bcgGroup.DefaultIfEmpty()
                                              join bcc in _context.base_clientes_banco_color on bcb.IdColorBanco equals bcc.IdColor into bccGroup
                                              from bcc in bccGroup.DefaultIfEmpty()
                                              join bcp in _context.base_clientes_banco_plazo on bcb.IdPlazoBanco equals bcp.IdPlazo into bcpGroup
                                              from bcp in bcpGroup.DefaultIfEmpty()
                                              join bcrd in _context.base_clientes_banco_rango_deuda on bcb.IdRangoDeuda equals bcrd.IdRangoDeuda into bcrdGroup
                                              from bcrd in bcrdGroup.DefaultIfEmpty()
                                              join bcu in _context.base_clientes_banco_usuario on bcb.IdUsuarioBanco equals bcu.IdUsuario into bcuGroup
                                              from bcu in bcuGroup.DefaultIfEmpty()
                                              join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                              join ca in _context.clientes_asignados on ce.IdCliente equals ca.IdCliente
                                              where bc.IdBase == IdBase
                                                  && bc.IdBaseBanco != null
                                                  && ca.FechaAsignacionVendedor.HasValue
                                                  && ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                  && ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month
                                                  && ca.IdUsuarioV == usuarioId
                                              select new DetalleClienteA365TipificarDTO
                                              {
                                                  // Propiedades de BaseCliente
                                                  Dni = bc.Dni,
                                                  XAppaterno = bc.XAppaterno,
                                                  XApmaterno = bc.XApmaterno,
                                                  XNombre = bc.XNombre,
                                                  Edad = bc.Edad,
                                                  Departamento = bc.Departamento,
                                                  Provincia = bc.Provincia,
                                                  Distrito = bc.Distrito,
                                                  IdBase = bc.IdBase,

                                                  // Propiedades de DetalleBase
                                                  Campaña = bcg.NombreCampana ?? "DESCONOCIDO",
                                                  Sucursal = "DESCONOCIDO",
                                                  AgenciaComercial = bcb != null ? $"Numero Entidades: {bcb.NumEntidades}" : "Numero Entidades: Desconocido",
                                                  Plazo = bcp.NumMeses,
                                                  Cuota = 0,
                                                  GrupoTasa = bcb.TasasEspeciales,
                                                  GrupoMonto = "DESCONOCIDO",
                                                  Propension = 0,
                                                  TipoCliente = bcu.TipoUsuario,
                                                  ClienteNuevo = "ANTIGUO",
                                                  Color = bcc.NombreColor ?? "DESCONOCIDO",
                                                  ColorFinal = bcc.NombreColor ?? "DESCONOCIDO",
                                                  Usuario = bcu.NombreUsuario,
                                                  SegmentoUser = bcu.TipoUsuario,
                                                  PerfilRo = "DESCONOCIDO",

                                                  // Propiedades de ClientesEnriquecido
                                                  IdCliente = ce.IdCliente,
                                                  Telefono1 = ce.Telefono1,
                                                  Telefono2 = ce.Telefono2,
                                                  Telefono3 = ce.Telefono3,
                                                  Telefono4 = ce.Telefono4,
                                                  Telefono5 = ce.Telefono5,
                                                  ComentarioTelefono1 = ce.ComentarioTelefono1,
                                                  ComentarioTelefono2 = ce.ComentarioTelefono2,
                                                  ComentarioTelefono3 = ce.ComentarioTelefono3,
                                                  ComentarioTelefono4 = ce.ComentarioTelefono4,
                                                  ComentarioTelefono5 = ce.ComentarioTelefono5,
                                                  UltimaTipificacionTelefono1 = ce.UltimaTipificacionTelefono1,
                                                  UltimaTipificacionTelefono2 = ce.UltimaTipificacionTelefono2,
                                                  UltimaTipificacionTelefono3 = ce.UltimaTipificacionTelefono3,
                                                  UltimaTipificacionTelefono4 = ce.UltimaTipificacionTelefono4,
                                                  UltimaTipificacionTelefono5 = ce.UltimaTipificacionTelefono5,
                                                  FechaUltimaTipificacionTelefono1 = ce.FechaUltimaTipificacionTelefono1,
                                                  FechaUltimaTipificacionTelefono2 = ce.FechaUltimaTipificacionTelefono2,
                                                  FechaUltimaTipificacionTelefono3 = ce.FechaUltimaTipificacionTelefono3,
                                                  FechaUltimaTipificacionTelefono4 = ce.FechaUltimaTipificacionTelefono4,
                                                  FechaUltimaTipificacionTelefono5 = ce.FechaUltimaTipificacionTelefono5,
                                                  // Propiedades de Tasas y Detalles
                                                  OfertaMax = bcb.OfertaMax,
                                                  Tasa12m = bcb.Tasa1,
                                                  Tasa18m = bcb.Tasa2,
                                                  Tasa24m = bcb.Tasa3,
                                                  Tasa36m = bcb.Tasa4,
                                                  // Propiedades de ClientesAsignados
                                                  IdAsignacion = ca.IdAsignacion,
                                                  FuenteBase = ca.FuenteBase
                                              }).FirstOrDefaultAsync();

                if (detallesClientes == null)
                {
                    return (false, "No se encontraron datos para tipificar el cliente", null);
                }

                var detalleTipificarCliente = new DetalleTipificarClienteDTO
                {
                    // Propiedades de BaseCliente
                    Dni = detallesClientes.Dni,
                    XAppaterno = detallesClientes.XAppaterno,
                    XApmaterno = detallesClientes.XApmaterno,
                    XNombre = detallesClientes.XNombre,
                    Edad = 0,
                    Departamento = "DESCONOCIDO",
                    Provincia = "DESCONOCIDO",
                    Distrito = "DESCONOCIDO",
                    IdBase = detallesClientes.IdBase,

                    // Propiedades de DetalleBase
                    Campaña = detallesClientes.Campaña ?? "DESCONOCIDO",
                    OfertaMax = detallesClientes.OfertaMax * 100 ?? 0,
                    TasaMinima = 0,
                    Sucursal = "DESCONOCIDO",
                    AgenciaComercial = detallesClientes.AgenciaComercial,
                    Plazo = detallesClientes.Plazo,
                    Cuota = detallesClientes.Cuota ?? 0,
                    GrupoTasa = detallesClientes.GrupoTasa ?? "DESCONOCIDO",
                    GrupoMonto = detallesClientes.GrupoMonto ?? "DESCONOCIDO",
                    Propension = detallesClientes.Propension ?? 0,
                    TipoCliente = detallesClientes.TipoCliente ?? "DESCONOCIDO",
                    ClienteNuevo = detallesClientes.ClienteNuevo ?? "DESCONOCIDO",
                    Color = detallesClientes.Color ?? "DESCONOCIDO",
                    ColorFinal = detallesClientes.ColorFinal ?? "DESCONOCIDO",
                    PerfilRo = detallesClientes.PerfilRo ?? "DESCONOCIDO",

                    // Propiedades de ClientesEnriquecido
                    Telefonos = new List<TelefonoDTO>
                        {
                            new TelefonoDTO
                            {
                                Numero = detallesClientes.Telefono1 ?? "0",
                                Comentario = detallesClientes.ComentarioTelefono1,
                                DescripcionTipificacion = detallesClientes.UltimaTipificacionTelefono1,
                                FechaTipificacion = detallesClientes.FechaUltimaTipificacionTelefono1
                            },
                            new TelefonoDTO
                            {
                                Numero = detallesClientes.Telefono2 ?? "0",
                                Comentario = detallesClientes.ComentarioTelefono2,
                                DescripcionTipificacion = detallesClientes.UltimaTipificacionTelefono2,
                                FechaTipificacion = detallesClientes.FechaUltimaTipificacionTelefono2
                            },
                            new TelefonoDTO
                            {
                                Numero = detallesClientes.Telefono3 ?? "0",
                                Comentario = detallesClientes.ComentarioTelefono3,
                                DescripcionTipificacion = detallesClientes.UltimaTipificacionTelefono3,
                                FechaTipificacion = detallesClientes.FechaUltimaTipificacionTelefono3
                            },
                            new TelefonoDTO
                            {
                                Numero = detallesClientes.Telefono4 ?? "0",
                                Comentario = detallesClientes.ComentarioTelefono4,
                                DescripcionTipificacion = detallesClientes.UltimaTipificacionTelefono4,
                                FechaTipificacion = detallesClientes.FechaUltimaTipificacionTelefono4
                            },
                            new TelefonoDTO
                            {
                                Numero = detallesClientes.Telefono5 ?? "0",
                                Comentario = detallesClientes.ComentarioTelefono5,
                                DescripcionTipificacion = detallesClientes.UltimaTipificacionTelefono5,
                                FechaTipificacion = detallesClientes.FechaUltimaTipificacionTelefono5
                            },
                        },

                    //Propiedades Tasas y Detalles
                    Tasa12m = detallesClientes.Tasa12m ?? 0,
                    Tasa18m = detallesClientes.Tasa18m ?? 0,
                    Tasa24m = detallesClientes.Tasa24m ?? 0,
                    Tasa36m = detallesClientes.Tasa36m ?? 0,
                    Usuario = detallesClientes.Usuario ?? "DESCONOCIDO",
                    SegmentoUser = detallesClientes.SegmentoUser ?? "DESCONOCIDO",
                    IdCliente = detallesClientes.IdCliente,
                    IdAsignacion = detallesClientes.IdAsignacion,
                    FuenteBase = detallesClientes.FuenteBase
                };

                return (true, "Datos para tipificar el cliente encontrados", detalleTipificarCliente);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}