using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace ALFINapp.Infrastructure.Services
{
    /// <summary>
    /// Service class that provides database operations for advisor assignment management in the ALFINapp system.
    /// Handles client reassignment processes between advisors and databases.
    /// </summary>
    public class DBServicesAsignacionesAsesores
    {
        private readonly MDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="DBServicesAsignacionesAsesores"/> class.
        /// </summary>
        /// <param name="context">The database context used for database operations.</param>
        public DBServicesAsignacionesAsesores(MDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Reassigns a client to a specific advisor in the system.
        /// </summary>
        /// <param name="DNIBusqueda">DNI (identification number) of the client to reassign.</param>
        /// <param name="BaseTipo">Source database type ("BDA365" or "BDALFIN").</param>
        /// <param name="IdUsuarioVAsignar">ID of the advisor to assign the client to.</param>
        /// <returns>
        /// A tuple containing:
        /// - IsSuccess: Indicates if the reassignment was successful
        /// - message: Descriptive message about the result
        /// </returns>
        /// <remarks>
        /// This method handles the reassignment process differently based on the database source:
        /// 
        /// For BDA365:
        /// 1. Verifies the client exists in the A365 database
        /// 2. Checks if the client is already enriched in the system
        /// 3. If assigned to supervisor but not advisor, updates the assignment
        /// 4. If not assigned at all, creates new enriched record and assignment
        /// 
        /// For BDALFIN:
        /// 1. Verifies the client exists in the ALFIN bank database
        /// 2. Creates or updates base_clientes record if needed
        /// 3. Creates or retrieves the enriched client record
        /// 4. Creates a new assignment record with ALFIN source
        /// 
        /// In both cases, the method prevents duplicate assignments within the same month.
        /// </remarks>
        /// <exception cref="Exception">Thrown when there is an error during the database operation.</exception>
        public async Task<(bool IsSuccess, string message)> GuardarReAsignacionCliente(string DNIBusqueda, string BaseTipo, int IdUsuarioVAsignar)
        {
            try
            {
                var supervisorAsignado = await _context.usuarios
                    .Where(u => u.IdUsuario == IdUsuarioVAsignar)
                    .Select(u => u.IDUSUARIOSUP)
                    .FirstOrDefaultAsync();

                if (supervisorAsignado == null)
                {
                    return (false, "No se encontró un supervisor asignado para el usuario vendedor.");
                }

                var clientePreviamenteAsignadoAUsted = await (
                    from bc in _context.base_clientes
                    join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase into ceGroup
                    from ce in ceGroup.DefaultIfEmpty()
                    join ca in _context.clientes_asignados on ce.IdCliente equals ca.IdCliente into caGroup
                    from ca in caGroup.DefaultIfEmpty()
                    where ca.IdUsuarioV == IdUsuarioVAsignar &&
                          bc.Dni == DNIBusqueda &&
                        ca.FechaAsignacionVendedor.HasValue &&
                        ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year &&
                        ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month
                    select ca
                ).FirstOrDefaultAsync();
                if (clientePreviamenteAsignadoAUsted != null)
                {
                    return (false, "El cliente a buscar ya se encuentra asignado a usted");
                }

                if (BaseTipo == "BDA365")
                {
                    var ClienteDBA365 = await (
                        from bc in _context.base_clientes
                        join db in _context.detalle_base on bc.IdBase equals db.IdBase
                        where bc.Dni == DNIBusqueda
                        orderby db.FechaCarga descending
                        select new { bc, db }
                    ).FirstOrDefaultAsync();

                    if (ClienteDBA365 == null)
                    {
                        return (false, "El cliente no tiene Detalle Base en la Base de Datos de A365, si el cliente tiene Datos en la base de Datos del banco Alfin puede hacer la consulta con los datos correspondientes");
                    }

                    var EnriquecidoClienteA365 = await _context.clientes_enriquecidos.FirstOrDefaultAsync(ce => ce.IdBase == ClienteDBA365.bc.IdBase);
                    if (EnriquecidoClienteA365 != null)
                    {
                        var checkAsignacion = await _context.clientes_asignados
                            .Where(c => c.IdCliente == EnriquecidoClienteA365.IdCliente &&
                                        c.IdUsuarioS == supervisorAsignado &&
                                        c.IdUsuarioV == null &&
                                        c.FechaAsignacionVendedor.HasValue &&
                                        c.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year &&
                                        c.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month)
                            .FirstOrDefaultAsync();
                        if (checkAsignacion != null)
                        {
                            //El supervisor tiene base creada unicamente mover la asignacion al vendedor
                            checkAsignacion.IdUsuarioV = IdUsuarioVAsignar;
                            checkAsignacion.FechaAsignacionVendedor = DateTime.Now;
                            checkAsignacion.ClienteDesembolso = false;
                            checkAsignacion.ClienteRetirado = false;
                            _context.clientes_asignados.Update(checkAsignacion);
                            await _context.SaveChangesAsync();
                            return (true, "El cliente fue asignado correctamente a la Base A365.");
                        }

                        var nuevoClienteAsignado = new ClientesAsignado
                        {
                            IdUsuarioV = IdUsuarioVAsignar,
                            FechaAsignacionVendedor = DateTime.Now,
                            IdCliente = EnriquecidoClienteA365.IdCliente,
                            FuenteBase = ClienteDBA365.db.TipoBase,
                            FinalizarTipificacion = false,
                            IdUsuarioS = supervisorAsignado.Value,
                            FechaAsignacionSup = DateTime.Now,
                            ClienteDesembolso = false,
                            ClienteRetirado = false,
                            Destino = "A365_AGREGADO_MANUALMENTE",
                        };
                        _context.clientes_asignados.Add(nuevoClienteAsignado);
                        await _context.SaveChangesAsync();
                        return (true, "El cliente fue asignado correctamente a la Base A365");
                    }
                    else
                    {
                        var nuevoClienteEnriquecidos = new ClientesEnriquecido
                        {
                            IdBase = ClienteDBA365.bc.IdBase,
                            FechaEnriquecimiento = DateTime.Now,
                        };
                        _context.clientes_enriquecidos.Add(nuevoClienteEnriquecidos);
                        await _context.SaveChangesAsync();

                        var nuevoClienteAsignado = new ClientesAsignado
                        {
                            IdCliente = nuevoClienteEnriquecidos.IdCliente,
                            IdUsuarioS = supervisorAsignado.Value,
                            IdUsuarioV = IdUsuarioVAsignar,
                            FechaAsignacionSup = DateTime.Now,
                            FechaAsignacionVendedor = DateTime.Now,
                            FuenteBase = ClienteDBA365.db.TipoBase,
                            FinalizarTipificacion = false,
                            ClienteDesembolso = false,
                            ClienteRetirado = false,
                            Destino = "A365_AGREGADO_MANUALMENTE",
                        };

                        _context.clientes_asignados.Add(nuevoClienteAsignado);
                        await _context.SaveChangesAsync();
                        return (true, "El cliente fue asignado correctamente a la Base A365");
                    }
                }
                else if (BaseTipo == "BDALFIN")
                {
                    var ClienteDBAlfinBanco = await _context.base_clientes_banco
                        .Where(c => c.Dni == DNIBusqueda)
                        .OrderByDescending(c => c.FechaSubida)
                        .FirstOrDefaultAsync();
                    if (ClienteDBAlfinBanco == null)
                    {
                        return (false, "El cliente no tiene entrada en la Base de Datos de ALFIN, si el cliente tiene Datos en la base de Datos del banco A365 puede hacer la consulta con los datos correspondientes");
                    }

                    var BaseClienteBanco = await _context.base_clientes.FirstOrDefaultAsync(c => c.IdBaseBanco == ClienteDBAlfinBanco.IdBaseBanco || c.Dni == ClienteDBAlfinBanco.Dni);

                    var nuevoBaseClienteDelBanco = new BaseCliente();
                    if (BaseClienteBanco == null)
                    {
                        //El Cliente No tiene Detalle Base de la Base de Datos de ALFIN
                        nuevoBaseClienteDelBanco = new BaseCliente
                        {
                            IdBaseBanco = ClienteDBAlfinBanco.IdBaseBanco,
                            Dni = ClienteDBAlfinBanco.Dni,
                            XNombre = ClienteDBAlfinBanco.NOMBRES,
                            XAppaterno = ClienteDBAlfinBanco.PATERNO,
                            XApmaterno = ClienteDBAlfinBanco.MATERNO,
                        };
                        _context.base_clientes.Add(nuevoBaseClienteDelBanco);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        nuevoBaseClienteDelBanco = BaseClienteBanco;
                        nuevoBaseClienteDelBanco.IdBaseBanco = ClienteDBAlfinBanco.IdBaseBanco;
                    }

                    var EnriquecidoClienteAlfin = await _context.clientes_enriquecidos.FirstOrDefaultAsync(ce => ce.IdBase == nuevoBaseClienteDelBanco.IdBase);

                    var nuevoClienteEnriquecidos = new ClientesEnriquecido();

                    if (EnriquecidoClienteAlfin == null)
                    {
                        //NO HAY ENTRADA DE CLIENTES ENRIQUECIDO SE CREARA LA ENTRADA
                        nuevoClienteEnriquecidos = new ClientesEnriquecido
                        {
                            IdBase = nuevoBaseClienteDelBanco.IdBase,
                            FechaEnriquecimiento = DateTime.Now,
                            Telefono1 = ClienteDBAlfinBanco.Numero1,
                            Telefono2 = ClienteDBAlfinBanco.Numero2,
                            Telefono3 = ClienteDBAlfinBanco.Numero3,
                            Telefono4 = ClienteDBAlfinBanco.Numero4,
                            Telefono5 = ClienteDBAlfinBanco.Numero5,
                        };
                        _context.clientes_enriquecidos.Add(nuevoClienteEnriquecidos);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        nuevoClienteEnriquecidos = EnriquecidoClienteAlfin;
                    }

                    var nuevoClienteAsignado = new ClientesAsignado
                    {
                        IdUsuarioV = IdUsuarioVAsignar,
                        FechaAsignacionVendedor = DateTime.Now,
                        IdCliente = nuevoClienteEnriquecidos.IdCliente,
                        FuenteBase = "ALFINBANCO",
                        FinalizarTipificacion = false,
                        IdUsuarioS = supervisorAsignado.Value,
                        FechaAsignacionSup = DateTime.Now,
                        ClienteDesembolso = false,
                        ClienteRetirado = false,
                        Destino = "ALFIN_AGREGADO_MANUALMENTE",
                    };
                    _context.clientes_asignados.Add(nuevoClienteAsignado);
                    await _context.SaveChangesAsync();
                    return (true, "El cliente fue asignado correctamente de la Base ALFIN");
                }
                else
                {
                    return (false, "Se mando un Tipo de Base distinto de los esperados el sistema no ha realizado ninguna insercion");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}