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
        public async Task<(bool IsSuccess, string message)> GuardarReAsignacionCliente(string DNIBusqueda, string BaseTipo, int IdUsuarioVAsignar)
        {
            try
            {
                if (BaseTipo == "A365")
                {
                    var ClienteDBA365 = await (
                                                    from c in _context.base_clientes
                                                    join d in _context.detalle_base on c.IdBase equals d.IdBase
                                                    where c.Dni == DNIBusqueda
                                                    select new
                                                    {
                                                        Cliente = c,
                                                        Detalle = d
                                                    }
                                                ).FirstOrDefaultAsync();
                    if (ClienteDBA365 == null)
                    {
                        return (false, "El cliente no tiene Detalle Base en la Base de Datos de A365, si el cliente tiene Datos en la base de Datos del banco Alfin puede hacer la consulta con los datos correspondientes");
                    }
                    if (ClienteDBA365 != null)
                    {
                        //FUNCIONALIDAD: El cliente FUE encontrado en A365 
                        var EnriquecidoClienteA365 = await _context.clientes_enriquecidos.FirstOrDefaultAsync(ce => ce.IdBase == ClienteDBA365.Cliente.IdBase);
                        if (EnriquecidoClienteA365 != null)
                        {
                            //FUNCIONALIDAD: El cliente FUE encontrado en A365 Y TIENE UNA ENTRADA EN LA TABLA DE CLIENTES ENRIQUECIDOS
                            var clientePreviamenteAsignadoAUsted = await _context.clientes_asignados.FirstOrDefaultAsync(ca => ca.IdUsuarioV == IdUsuarioVAsignar);
                            if (clientePreviamenteAsignadoAUsted != null)
                            {
                                //FUNCIONALIDAD: El cliente a buscar ya se encuentra asignado a AL USUARIO VENDEDOR
                                return (false, "El cliente a buscar ya se encuentra asignado a usted");
                            }

                            var nuevoClienteAsignado = new ClientesAsignado
                            {
                                IdUsuarioV = IdUsuarioVAsignar,
                                FechaAsignacionVendedor = DateTime.Now,
                                IdCliente = EnriquecidoClienteA365.IdCliente,
                                FuenteBase = ClienteDBA365.Detalle.TipoBase,
                                FinalizarTipificacion = false,
                                IdUsuarioS = 0,
                                FechaAsignacionSup = DateTime.Now,
                                ClienteDesembolso = false,
                                ClienteRetirado = false,
                            };
                            _context.clientes_asignados.Add(nuevoClienteAsignado);
                            await _context.SaveChangesAsync();
                            return (true, "El cliente fue asignado correctamente a la Base A365");
                        }
                        else
                        {
                            //FUNCIONALIDAD: El cliente NO tiene una entrada en la tabla de clientes enriquecidos: Se debe crear Manualmente
                            /*var nuevoClienteEnriquecidos = new ClientesEnriquecido
                            {
                                IdBase = ClienteDBA365.IdBase,
                                FechaEnriquecimiento = DateTime.Now,
                                IdCliente = ClienteDBA365.IdBase,
                            };
                            _context.clientes_enriquecidos.Add(nuevoClienteEnriquecidos);
                            //FUNCIONALIDAD: El cliente SERA ASIGNADO MANUALMENTE A LA TABLA CLIENTES ASIGNADOS*/
                            return (false, "El cliente NO tiene una entrada en la tabla de clientes enriquecidos: Se debe crear Manualmente");
                        }
                    }
                }

                if (BaseTipo == "ALFIN")
                {
                    var ClienteDBAlfinBanco = await _context.base_clientes_banco.FirstOrDefaultAsync(c => c.Dni == DNIBusqueda);
                }
                return (false, "Se mando un BaseTipo distinto de los esperados el sistema ha sido comprometido");
                // Buscar al cliente en las bases de datos Alfin y A365
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
                throw;
            }

        }

    }
}