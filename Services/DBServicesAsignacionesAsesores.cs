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
        public async Task<Tuple<bool, string>> GuardarReAsignacionCliente(string DNIBusqueda, string BaseTipo, int IdUsuarioVAsignar)
        {
            try
            {
                var getCliente = await (from bc in _context.base_clientes
                                        where bc.Dni == DNIBusqueda
                                        select new { bc }).FirstOrDefaultAsync();

                if (getCliente == null || getCliente.bc == null)
                {
                    return new Tuple<bool, string>(false, $"El cliente con DNI {DNIBusqueda} no ha podido ser encontrado, problamente fue eliminado");
                }

                var getEnriquecidoCliente = await (from ce in _context.clientes_enriquecidos
                                                   where ce.IdBase == getCliente.bc.IdBase
                                                   select new { ce }).FirstOrDefaultAsync();

                if (getEnriquecidoCliente == null || getEnriquecidoCliente.ce == null)
                {
                    Console.WriteLine($"El cliente con DNI {DNIBusqueda} no tiene datos enriquecidos");
                    var datosEnriquecidos = new ClientesEnriquecido
                    {
                        IdBase = getCliente.bc.IdBase,
                        FechaEnriquecimiento = DateTime.Now,
                    };

                    _context.clientes_enriquecidos.Add(datosEnriquecidos);
                    var result1 = await _context.SaveChangesAsync();

                    if (result1 <= 0)
                    {
                        return new Tuple<bool, string>(false, "Error al guardar la reasignación del cliente.");
                    }

                }

                var getBaseCliente = await (from bc in _context.base_clientes
                                            where bc.Dni == DNIBusqueda
                                            join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                            join ca in _context.clientes_asignados on ce.IdCliente equals ca.IdCliente into clientesAsignadosGroup
                                            from ca in clientesAsignadosGroup.DefaultIfEmpty() // Left join con clientes_asignados
                                            select new { bc, ce, ca }).ToListAsync();

                var IdUsuarioSupervisor = await (from u in _context.usuarios
                                                 where
                                                        u.IdUsuario == IdUsuarioVAsignar
                                                 select u.IDUSUARIOSUP
                                                        ).FirstOrDefaultAsync();
                var clienteId = 0;
                foreach (var clientesBase in getBaseCliente)
                {
                    clienteId = clientesBase.ce.IdCliente;
                    if (clientesBase.ca.IdUsuarioV == IdUsuarioVAsignar)
                    {
                        return new Tuple<bool, string>(false, $"El cliente con DNI {DNIBusqueda} esta asignado a usted");
                    }
                }

                var reasignarCliente = new ClientesAsignado
                {
                    IdUsuarioV = IdUsuarioVAsignar,
                    FechaAsignacionVendedor = DateTime.Now,
                    IdCliente = clienteId,
                    FuenteBase = BaseTipo,
                    FinalizarTipificacion = false,
                    IdUsuarioS = IdUsuarioSupervisor,
                    FechaAsignacionSup = DateTime.Now,
                    ClienteDesembolso = false,
                    ClienteRetirado = false,
                };

                _context.clientes_asignados.Add(reasignarCliente);
                var result2 = await _context.SaveChangesAsync();
                if (result2 <= 0)
                {
                    return new Tuple<bool, string>(false, "Error al guardar la reasignación del cliente.");
                }

                return new Tuple<bool, string>(true, "La reasignacion del cliente se produjo con exito");
            }
            catch (System.Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
                throw;
            }
            
        }
        
    }
}