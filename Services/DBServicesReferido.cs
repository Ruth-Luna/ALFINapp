using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Services
{
    public class DBServicesReferido
    {
        private readonly MDbContext _context;

        public DBServicesReferido(MDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Guarda un cliente referido en la base de datos especificada.
        /// </summary>
        /// <param name="dni">El DNI del cliente referido.</param>
        /// <param name="fuenteBase">La fuente de la base de datos (DBA365 o DBALFIN).</param>
        /// <param name="nombres">Los nombres del asesor que refiere al cliente.</param>
        /// <param name="apellidos">Los apellidos del asesor que refiere al cliente.</param>
        /// <param name="dniUsuario">El DNI del usuario que refiere al cliente.</param>
        /// <returns>Una tupla que indica si la operación fue exitosa y un mensaje asociado.</returns>
        public async Task<(bool IsSuccess, string Message)> GuardarClienteReferido(string dni, 
                                                                                                            string fuenteBase,
                                                                                                            string nombres,
                                                                                                            string apellidos,
                                                                                                            string dniUsuario)
        {
            try
            {
                if (fuenteBase == "DBA365")
                {
                    var datosClienteReferido = await (from bc in _context.base_clientes
                                                      where bc.Dni == dni
                                                      select new
                                                      {
                                                          bc
                                                      }).FirstOrDefaultAsync();

                    if (datosClienteReferido == null)
                    {
                        return (false, "No se ha encontrado el usuario en la base de datos enviada");
                    }

                    var clienteReferido = new ClientesReferidos
                    {
                        IdBaseClienteA365 = datosClienteReferido.bc.IdBase,
                        IdSupervisorReferido = 39,
                        NombreCompletoAsesor = nombres + " " + apellidos,
                        NombreCompletoCliente = datosClienteReferido.bc.XNombre + " " + datosClienteReferido.bc.XAppaterno + " " + datosClienteReferido.bc.XApmaterno,
                        DniAsesor = dniUsuario,
                        DniCliente = datosClienteReferido.bc.Dni,
                        FechaReferido = DateTime.Now,
                        TraidoDe = "DBA365",
                        FueProcesado = false
                    };
                    _context.Add(clienteReferido);
                    await _context.SaveChangesAsync();
                    return (true, "Usuario referido exitosamente");
                }
                else if (fuenteBase == "DBALFIN")
                {
                    var datosClienteReferido = await (from bcb in _context.base_clientes_banco
                                                      where bcb.Dni == dni
                                                      select new
                                                      {
                                                          bcb
                                                      }).FirstOrDefaultAsync();

                    if (datosClienteReferido == null)
                    {
                        return (false, "No se ha encontrado el usuario en la base de datos enviada");
                    }

                    var clienteReferido = new ClientesReferidos
                    {
                        IdBaseClienteBanco = datosClienteReferido.bcb.IdBaseBanco,
                        IdSupervisorReferido = 39,
                        NombreCompletoAsesor = nombres + " " + apellidos,
                        //NombreCompletoCliente = datosClienteReferido.bcb.XNombre + " " + datosClienteReferido.bcb.XAppaterno + " " + datosClienteReferido.bcb.XApmaterno,
                        DniAsesor = dniUsuario,
                        DniCliente = datosClienteReferido.bcb.Dni,
                        FechaReferido = DateTime.Now,
                        TraidoDe = "DBALFIN",
                        FueProcesado = false
                    };
                    _context.Add(clienteReferido);
                    await _context.SaveChangesAsync();
                    return (true, "Usuario referido exitosamente");
                }

                return (false, "No se ha encontrado la fuente de la base de datos asignada");

            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool IsSuccess, string Message, DniReferidoData? data)> GetDataFromDNI(string DNI)
        {
            try
            {
                var clientebc = await _context.base_clientes.Where(bc => bc.Dni == DNI)
                                    .FirstOrDefaultAsync();

                if (clientebc != null)
                {
                    var dataclientebc = new DniReferidoData
                    {
                        DNI = clientebc.Dni,
                        NombresCompletos = clientebc.XNombre + " " + clientebc.XAppaterno + " " + clientebc.XApmaterno,
                        IdBaseCliente = clientebc.IdBase,
                        TraidoDe = "DBA365"
                    };
                    return (true, "El Usuario se ha encontrado en la Base de Datos de A365", dataclientebc);
                }

                var clientebcb = await _context.base_clientes_banco.Where(bcb => bcb.Dni == DNI)
                                    .FirstOrDefaultAsync();

                if (clientebcb != null)
                {
                    var dataclientebcb = new DniReferidoData
                    {
                        DNI = clientebcb.Dni,
                        IdBaseCliente = clientebcb.IdBaseBanco,
                        TraidoDe = "DBALFIN"
                    };

                    return (true, "El Usuario se ha encontrado en la Base de Datos de ALFIN", dataclientebcb);
                }
                return (false, "El Usuario no se encuentra registrado en ninguna de las bases de datos", null);
            }
            catch (System.Exception ex)
            {
                return (true, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string Message, ClientesReferidos? Data)> GetClienteReferido(string dni)
        {
            try
            {
                var clienteReferido = await _context.clientes_referidos.Where(cr => cr.DniCliente == dni)
                                        .FirstOrDefaultAsync();

                if (clienteReferido != null)
                {
                    return (true, "El cliente referido ha sido encontrado en la base de datos", clienteReferido);
                }

                return (false, "El cliente referido no ha sido encontrado en la base de datos", null);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}