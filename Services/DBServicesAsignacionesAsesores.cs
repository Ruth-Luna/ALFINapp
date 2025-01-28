using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.EntityFrameworkCore;


namespace ALFINapp.Services
{
    public class DBServicesAsignacionesAsesores
    {
        private readonly MDbContext _context;

        public DBServicesAsignacionesAsesores(MDbContext context)
        {
            _context = context;
        }

        //ACA ESTAN LAS INSERCIONES A LA DB
        /// <summary>
        /// Guarda la reasignación de un cliente a un usuario vendedor en la base de datos especificada.
        /// </summary>
        /// <param name="DNIBusqueda">El DNI del cliente a buscar.</param>
        /// <param name="BaseTipo">El tipo de base de datos donde se realizará la búsqueda ("BDA365" o "BDALFIN").</param>
        /// <param name="IdUsuarioVAsignar">El ID del usuario vendedor al que se asignará el cliente.</param>
        /// <returns>Una tupla que indica si la operación fue exitosa y un mensaje descriptivo.</returns>
        /// <remarks>
        /// Este método realiza las siguientes operaciones:
        /// - Busca el supervisor asignado al usuario vendedor.
        /// - Dependiendo del tipo de base, busca al cliente en la base de datos correspondiente.
        /// - Si el cliente ya está asignado al usuario vendedor en el mes y año actual, retorna un mensaje indicándolo.
        /// - Si el cliente no está asignado, lo asigna al usuario vendedor y guarda los cambios en la base de datos.
        /// </remarks>
        /// <exception cref="Exception">Retorna el mensaje de la excepción en caso de error.</exception>
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

                if (BaseTipo == "BDA365")
                {
                    var ClienteDBA365 = await (
                        from bc in _context.base_clientes
                        join db in _context.detalle_base on bc.IdBase equals db.IdBase
                        where bc.Dni == DNIBusqueda
                        orderby db.FechaCarga
                        select new { bc, db }
                    ).FirstOrDefaultAsync();

                    if (ClienteDBA365 == null)
                    {
                        return (false, "El cliente no tiene Detalle Base en la Base de Datos de A365, si el cliente tiene Datos en la base de Datos del banco Alfin puede hacer la consulta con los datos correspondientes");
                    }

                    var EnriquecidoClienteA365 = await _context.clientes_enriquecidos.FirstOrDefaultAsync(ce => ce.IdBase == ClienteDBA365.bc.IdBase);
                    if (EnriquecidoClienteA365 != null)
                    {
                        var clientePreviamenteAsignadoAUsted =
                            await _context.clientes_asignados
                                .Where(ca => ca.IdUsuarioV == IdUsuarioVAsignar &&
                                             ca.IdCliente == EnriquecidoClienteA365.IdCliente &&
                                             ca.FechaAsignacionSup.HasValue &&
                                             ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month &&
                                             ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year)
                                .FirstOrDefaultAsync();
                        if (clientePreviamenteAsignadoAUsted != null)
                        {
                            return (false, "El cliente a buscar ya se encuentra asignado a usted");
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
                            Destino = "ALFIN_AGREGADO_MANUALMENTE",
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
                    var ClienteDBAlfinBanco = await _context.base_clientes_banco.FirstOrDefaultAsync(c => c.Dni == DNIBusqueda);
                    if (ClienteDBAlfinBanco == null)
                    {
                        return (false, "El cliente no tiene entrada en la Base de Datos de ALFIN, si el cliente tiene Datos en la base de Datos del banco A365 puede hacer la consulta con los datos correspondientes");
                    }

                    var EnriquecidoClienteAlfin = await _context.clientes_enriquecidos.FirstOrDefaultAsync(ce => ce.IdBaseBanco == ClienteDBAlfinBanco.IdBaseBanco);
                    if (EnriquecidoClienteAlfin != null)
                    {
                        var clientePreviamenteAsignadoAUsted =
                            await _context.clientes_asignados
                                .Where(ca => ca.IdUsuarioV == IdUsuarioVAsignar &&
                                             ca.IdCliente == EnriquecidoClienteAlfin.IdCliente &&
                                             ca.FechaAsignacionSup.HasValue &&
                                             ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month &&
                                             ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year)
                                .FirstOrDefaultAsync();
                        if (clientePreviamenteAsignadoAUsted != null)
                        {
                            return (false, "El cliente a buscar ya se encuentra asignado a usted");
                        }

                        var nuevoClienteAsignado = new ClientesAsignado
                        {
                            IdUsuarioV = IdUsuarioVAsignar,
                            FechaAsignacionVendedor = DateTime.Now,
                            IdCliente = EnriquecidoClienteAlfin.IdCliente,
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
                        var nuevoClienteEnriquecidos = new ClientesEnriquecido
                        {
                            IdBaseBanco = ClienteDBAlfinBanco.IdBaseBanco,
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
                            FuenteBase = "ALFINBANCO",
                            FinalizarTipificacion = false,
                            ClienteDesembolso = false,
                            ClienteRetirado = false,
                            Destino = "ALFIN_AGREGADO_MANUALMENTE",
                        };
                        _context.clientes_asignados.Add(nuevoClienteAsignado);
                        await _context.SaveChangesAsync();
                        return (true, "El cliente fue asignado correctamente de la Base ALFIN");
                    }
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