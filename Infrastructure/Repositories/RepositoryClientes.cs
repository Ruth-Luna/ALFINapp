using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryClientes : IRepositoryClientes
    {
        private readonly MDbContext _context;
        public RepositoryClientes(MDbContext context)
        {
            _context = context;
        }
        public async Task<List<DetallesAsignacionesDTO>?> GetAllAsignaciones(int idCliente)
        {
            try
            {
                return null;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<DetallesAsignacionesDTO>?> GetAllAsignaciones()
        {
            try
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                var firstDay = new DateTime(year, month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                var getAsignaciones = await _context.clientes_asignados
                    .AsNoTracking()
                    .Where(x => x.FechaAsignacionSup >= firstDay
                        && x.FechaAsignacionSup <= lastDay
                        && x.IdUsuarioV != null)
                    .ToListAsync();
                var asignaciones = new List<DetallesAsignacionesDTO>();
                foreach (var asignacion in getAsignaciones)
                {
                    asignaciones.Add(new DetallesAsignacionesDTO(asignacion));
                }
                return asignaciones;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<DetallesAsignacionesDTO>?> GetAllAsignacionesTrabajadas()
        {
            try
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                var firstDay = new DateTime(year, month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                var getAsig = await _context.clientes_asignados
                    .AsNoTracking()
                    .Where(x => x.PesoTipificacionMayor != null
                        && x.FechaAsignacionSup >= firstDay
                        && x.FechaAsignacionSup <= lastDay)
                    .ToListAsync();
                var asignaciones = new List<DetallesAsignacionesDTO>();
                foreach (var asignacion in getAsig)
                {
                    asignaciones.Add(new DetallesAsignacionesDTO(asignacion));
                }
                return asignaciones;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<DetallesAsignacionesDTO?> GetAsignacion(int idAsignacion)
        {
            try
            {
                var getAsig = await _context.clientes_asignados
                    .AsNoTracking()
                    .Where(x => x.IdAsignacion == idAsignacion)
                    .FirstOrDefaultAsync();
                if (getAsig != null)
                {
                    return new DetallesAsignacionesDTO(getAsig);
                }
                return null;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<BaseCliente?> getBase(int idBase)
        {
            try
            {
                var basecliente = await _context.base_clientes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdBase == idBase);
                return basecliente;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<BaseCliente?> getBase(string dni)
        {
            try
            {
                var basecliente = await _context.base_clientes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Dni == dni);
                return basecliente;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<(bool IsSuccess, string Message, Application.DTOs.DetallesClienteDTO? Data)> getClientesFromDBandBank(string dni)
        {
            try
            {
                var checkDesembolso = await (from d in _context.desembolsos
                                             where d.DniDesembolso == dni
                                              && d.FechaDesembolsos.HasValue
                                             && d.FechaDesembolsos.Value.Year == DateTime.Now.Year
                                             && d.FechaDesembolsos.Value.Month == DateTime.Now.Month
                                             select d).FirstOrDefaultAsync();
                if (checkDesembolso != null)
                {
                    return (false, "El cliente no puede ser tipificado porque ya tiene un desembolso en este mes", null);
                }

                var checkRetiros = await (from r in _context.retiros
                                          where r.DniRetiros == dni
                                          && r.FechaRetiro.HasValue
                                          && r.FechaRetiro.Value.Year == DateTime.Now.Year
                                          && r.FechaRetiro.Value.Month == DateTime.Now.Month
                                          select r).FirstOrDefaultAsync();

                if (checkRetiros != null)
                {
                    return (false, "El cliente no puede ser tipificado porque ya tiene un retiro en este mes", null);
                }
                var checkEntradaA365 = (from bc in _context.base_clientes
                                        join db in _context.detalle_base on bc.IdBase equals db.IdBase
                                        where bc.Dni == dni
                                        select new { bc, db }).ToList().OrderByDescending(c => c.db.FechaCarga).FirstOrDefault();
                var entradaA365 = (entradaDB: false, mensaje: string.Empty);
                if (checkEntradaA365 != null)
                {
                    if (checkEntradaA365.db.FechaCarga.HasValue && checkEntradaA365.db.FechaCarga.Value.Year == DateTime.Now.Year && checkEntradaA365.db.FechaCarga.Value.Month == DateTime.Now.Month)
                    {
                        var detalleclienteExistenteBD = _context.detalle_base
                        .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_por_DNI_A365 @DNI",
                            new SqlParameter("@DNI", dni))
                        .AsEnumerable()
                        .FirstOrDefault();
                        if (detalleclienteExistenteBD != null)
                        {
                            var clienteExistenteBD = await _context.base_clientes.FirstOrDefaultAsync(c => c.Dni == dni);
                            if (clienteExistenteBD == null)
                            {
                                return (false, "El cliente no tiene Detalles en la Base de Datos de A365. Este DNI no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                            }
                            var clienteA365Encontrado = new Application.DTOs.DetallesClienteDTO(detalleclienteExistenteBD, clienteExistenteBD);
                            return (true, "Entrada en la Base de Datos de A365 encontrado", clienteA365Encontrado);
                        }
                        else
                        {
                            entradaA365.entradaDB = false;
                            entradaA365.mensaje = "El cliente no tiene Detalles en la Base de Datos de A365. Se buscara en la base de datos interna del banco. ";
                        }
                    }
                    else
                    {
                        entradaA365.entradaDB = false;
                        entradaA365.mensaje = "El cliente no fue enviado por el banco este mes a la base de datos de A365. Se buscara en la base de datos interna del banco. ";
                    }
                }
                else
                {
                    entradaA365.entradaDB = false;
                    entradaA365.mensaje = "El cliente no tiene Detalles en la Base de Datos de A365. Se buscara en la base de datos interna del banco. ";
                }
                // Consulta a la base de datos del banco de clientes
                var checkEntradaBankAlfin = await (from bcb in _context.base_clientes_banco
                                                   where bcb.Dni == dni
                                                   orderby bcb.FechaSubida descending
                                                   select bcb).FirstOrDefaultAsync();

                if (checkEntradaBankAlfin == null || !checkEntradaBankAlfin.FechaSubida.HasValue)
                {
                    return (false, entradaA365.mensaje + "El cliente no tiene Detalles en la Base de Datos del Banco Alfin. Al cliente no se le permitira ser tipificado", null);
                }

                if (checkEntradaBankAlfin.FechaSubida.Value.Year != DateTime.Now.Year && checkEntradaBankAlfin.FechaSubida.Value.Month != DateTime.Now.Month)
                {
                    return (false, entradaA365.mensaje + "El cliente no fue enviado por el banco este mes a la base de datos interna de ALFIN. Al cliente no se le permitira ser tipificado ", null);
                }

                var clienteExistenteBank = _context.consulta_obtener_cliente
                    .FromSqlRaw("EXEC SP_Consulta_Obtener_Cliente_Banco_Alfin @dni",
                        new SqlParameter("@dni", dni))
                    .AsEnumerable()
                    .FirstOrDefault();

                if (clienteExistenteBank == null)
                {
                    return (false, entradaA365.mensaje + $"El cliente no tiene Detalles en la Base de Datos del Banco Alfin. Este DNI no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                }
                var clienteExistenteDto = new Application.DTOs.DetallesClienteDTO(clienteExistenteBank);
                return (true, entradaA365.mensaje + "El DNI se encuentra registrado en la Base de Datos de Alfin durante este mes. Al cliente se le permite ser tipificado", clienteExistenteDto);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error al consultar el cliente", null);
            }
        }

        public async Task<(bool IsSuccess, string Message, DetallesClienteDTO? Data)> getClientesFromTelefono(string telefono)
        {
            try
            {
                var detalleclienteExistenteBD = _context.detalle_base
                    .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_por_telefono @Telefono",
                        new SqlParameter("@Telefono", telefono))
                    .AsEnumerable()
                    .FirstOrDefault();

                if (detalleclienteExistenteBD == null)
                {
                    var detalleclienteExistenteBDALFIN = _context.consulta_obtener_cliente
                    .FromSqlRaw("EXEC SP_Consulta_Obtener_detalle_cliente_por_telefono_ALFIN_banco @Telefono",
                        new SqlParameter("@Telefono", telefono))
                    .AsEnumerable()
                    .FirstOrDefault();
                    if (detalleclienteExistenteBDALFIN == null)
                    {
                        return (false, "El cliente no tiene Detalles en la Base de Datos de A365, este TELEFONO no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                    }
                    var detalleclienteDto = new Application.DTOs.DetallesClienteDTO(detalleclienteExistenteBDALFIN);
                    return (true, "El cliente fue encontrado en la base de Datos de ALFIN", detalleclienteDto);
                }

                var baseClienteExistenteBD = await _context.base_clientes.FirstOrDefaultAsync(c => c.IdBase == detalleclienteExistenteBD.IdBase);

                if (baseClienteExistenteBD == null)
                {
                    return (false, "El cliente no tiene Detalles en la Base de Datos de A365, este TELEFONO no se encuentra en ninguna de nuestras bases de datos conocidas", null);
                }

                var clienteA365EncontradoDto = new Application.DTOs.DetallesClienteDTO(detalleclienteExistenteBD, baseClienteExistenteBD);
                return (true, "El cliente fue encontrado en la base de Datos de A365", clienteA365EncontradoDto);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<ClientesEnriquecido?> GetEnriquecido(int idCliente)
        {
            try
            {
                var enriquecido = await _context.clientes_enriquecidos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdCliente == idCliente);
                return enriquecido;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<ClientesEnriquecido?> GetEnriquecidoxBase(int idBase)
        {
            try
            {
                var enriquecido = await _context.clientes_enriquecidos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdBase == idBase);
                return enriquecido;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> UpdateAsignacion(ClientesAsignado asignacion)
        {
            try
            {
                var ClienteAsignado = await _context.clientes_asignados
                    .FirstOrDefaultAsync(x => x.IdAsignacion == asignacion.IdAsignacion);
                if (ClienteAsignado != null)
                {
                    ClienteAsignado.TipificacionMayorPeso = asignacion.TipificacionMayorPeso;
                    ClienteAsignado.PesoTipificacionMayor = asignacion.PesoTipificacionMayor;
                    ClienteAsignado.FechaTipificacionMayorPeso = asignacion.FechaTipificacionMayorPeso;
                    await _context.SaveChangesAsync();
                    return true;
                }
                Console.WriteLine("No se encontró la asignación");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateEnriquecido(ClientesEnriquecido enriquecido)
        {
            try
            {
                var ClienteEnriquecido = await _context.clientes_enriquecidos
                    .FirstOrDefaultAsync(x => x.IdCliente == enriquecido.IdCliente);
                if (ClienteEnriquecido != null)
                {
                    ClienteEnriquecido.ComentarioTelefono1 = enriquecido.ComentarioTelefono1;
                    ClienteEnriquecido.ComentarioTelefono2 = enriquecido.ComentarioTelefono2;
                    ClienteEnriquecido.ComentarioTelefono3 = enriquecido.ComentarioTelefono3;
                    ClienteEnriquecido.ComentarioTelefono4 = enriquecido.ComentarioTelefono4;
                    ClienteEnriquecido.ComentarioTelefono5 = enriquecido.ComentarioTelefono5;
                    ClienteEnriquecido.UltimaTipificacionTelefono1 = enriquecido.UltimaTipificacionTelefono1;
                    ClienteEnriquecido.UltimaTipificacionTelefono2 = enriquecido.UltimaTipificacionTelefono2;
                    ClienteEnriquecido.UltimaTipificacionTelefono3 = enriquecido.UltimaTipificacionTelefono3;
                    ClienteEnriquecido.UltimaTipificacionTelefono4 = enriquecido.UltimaTipificacionTelefono4;
                    ClienteEnriquecido.UltimaTipificacionTelefono5 = enriquecido.UltimaTipificacionTelefono5;
                    ClienteEnriquecido.FechaUltimaTipificacionTelefono1 = enriquecido.FechaUltimaTipificacionTelefono1;
                    ClienteEnriquecido.FechaUltimaTipificacionTelefono2 = enriquecido.FechaUltimaTipificacionTelefono2;
                    ClienteEnriquecido.FechaUltimaTipificacionTelefono3 = enriquecido.FechaUltimaTipificacionTelefono3;
                    ClienteEnriquecido.FechaUltimaTipificacionTelefono4 = enriquecido.FechaUltimaTipificacionTelefono4;
                    ClienteEnriquecido.FechaUltimaTipificacionTelefono5 = enriquecido.FechaUltimaTipificacionTelefono5;
                    ClienteEnriquecido.IdClientetipTelefono1 = enriquecido.IdClientetipTelefono1;
                    ClienteEnriquecido.IdClientetipTelefono2 = enriquecido.IdClientetipTelefono2;
                    ClienteEnriquecido.IdClientetipTelefono3 = enriquecido.IdClientetipTelefono3;
                    ClienteEnriquecido.IdClientetipTelefono4 = enriquecido.IdClientetipTelefono4;
                    ClienteEnriquecido.IdClientetipTelefono5 = enriquecido.IdClientetipTelefono5;
                    await _context.SaveChangesAsync();
                    return true;
                }
                Console.WriteLine("No se encontró el cliente enriquecido");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}