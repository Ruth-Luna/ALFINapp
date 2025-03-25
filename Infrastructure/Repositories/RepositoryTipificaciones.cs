using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryTipificaciones : IRepositoryTipificaciones
    {
        MDbContext _context;
        public RepositoryTipificaciones(MDbContext context)
        {
            _context = context;
        }

        public async Task<Tipificaciones?> GetTipificacion(int id)
        {
            try
            {
                var tipificacion = await _context.tipificaciones.FirstOrDefaultAsync(x => x.IdTipificacion == id);
                return tipificacion;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Task<IEnumerable<ClientesTipificado>> GetTipificaciones()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UploadGestionTip(ClientesAsignado clienteA, ClientesTipificado clienteT, ClientesEnriquecido clienteE, int idUsuario)
        {
            try
            {
                var clienteDatos = await (from bc in _context.base_clientes
                                  where bc.IdBase == clienteE.IdBase
                                  select bc)
                                .FirstOrDefaultAsync();
                if (clienteDatos == null)
                {
                    return (false);
                }
                var asesorDatos = await (from u in _context.usuarios
                                where u.IdUsuario == idUsuario
                                select u)
                                .FirstOrDefaultAsync();
                if (asesorDatos == null)
                {
                    return (false);
                }
                var detalle_base = new DetalleBase();
                detalle_base = (from db in _context.detalle_base
                                    where db.IdBase == clienteDatos.IdBase
                                    orderby db.FechaCarga descending
                                    select db)
                                    .FirstOrDefault();
                if (detalle_base == null)
                {
                    detalle_base = (from bcb in _context.base_clientes_banco
                                    join cg in _context.base_clientes_banco_campana_grupo on bcb.IdCampanaGrupoBanco equals cg.IdCampanaGrupo
                                    where bcb.IdBaseBanco == clienteDatos.IdBaseBanco
                                    orderby bcb.FechaSubida descending
                                    select new DetalleBase
                                    {
                                        OfertaMax = bcb.OfertaMax*100,
                                        Campaña = cg.NombreCampana,
                                    }).FirstOrDefault();
                }
                if (detalle_base == null)
                {
                    return (false);
                }
                var derivacionBusqueda = new DerivacionesAsesores();
                if (clienteT.IdTipificacion == 2)
                {
                    derivacionBusqueda = await (from da in _context.derivaciones_asesores
                                        where da.IdCliente == clienteE.IdCliente
                                        select da)
                                        .FirstOrDefaultAsync();
                    if (derivacionBusqueda == null)
                    {
                        return false;
                    }
                }
                string codigoTelefono = "TA";
                if (clienteT.TelefonoTipificado != null)
                {
                    if (clienteT.TelefonoTipificado == clienteE.Telefono1)
                    {
                        codigoTelefono = "T1";
                    }
                    else if (clienteT.TelefonoTipificado == clienteE.Telefono2)
                    {
                        codigoTelefono = "T2";
                    }
                    else if (clienteT.TelefonoTipificado == clienteE.Telefono3)
                    {
                        codigoTelefono = "T3";
                    }
                    else if (clienteT.TelefonoTipificado == clienteE.Telefono4)
                    {
                        codigoTelefono = "T4";
                    }
                    else if (clienteT.TelefonoTipificado == clienteE.Telefono5)
                    {
                        codigoTelefono = "T5";
                    }
                }

                var parameters = new[]
                {
                    new SqlParameter("@IdTipificacion", clienteT.IdClientetip),
                    new SqlParameter("@IdAsignacion", clienteA.IdAsignacion),
                    new SqlParameter("@CodCanal", "SYSTEMA365"),
                    new SqlParameter("@Canal", "A365"),
                    new SqlParameter("@DocCliente", clienteDatos.Dni),
                    new SqlParameter("@FechaEnvio", clienteA.FechaAsignacionVendedor),
                    new SqlParameter("@FechaGestion", clienteT.FechaTipificacion),
                    new SqlParameter("@HoraGestion", clienteT.FechaTipificacion != null ? 
                        (object)clienteT.FechaTipificacion.Value.TimeOfDay : DBNull.Value) 
                    { 
                        SqlDbType = SqlDbType.Time 
                    },
                    new SqlParameter("@Telefono", clienteT.TelefonoTipificado),
                    new SqlParameter("@OrigenTelefono", codigoTelefono),
                    new SqlParameter("@CodCampaña", detalle_base.Campaña),
                    new SqlParameter("@CodTip", clienteT.IdTipificacion),
                    new SqlParameter("@Oferta", detalle_base.OfertaMax),
                    new SqlParameter("@DocAsesor", asesorDatos.Dni),
                    new SqlParameter("@Origen", clienteT.Origen),
                    new SqlParameter("@ArchivoOrigen", "BD PROPIA"),
                    new SqlParameter("@FechaCarga", DateTime.Now),
                    new SqlParameter("@IdDerivacion", derivacionBusqueda.DniCliente != null ? (int)derivacionBusqueda.IdDerivacion : DBNull.Value),
                    new SqlParameter("@IdSupervisor", asesorDatos.IDUSUARIOSUP.HasValue ? (object)asesorDatos.IDUSUARIOSUP.Value : DBNull.Value),
                    new SqlParameter("@Supervisor", asesorDatos.RESPONSABLESUP),
                    new SqlParameter("@IdDesembolso", DBNull.Value),
                };
                var result = await _context.Database.ExecuteSqlRawAsync("EXECUTE SP_Tipificaciones_guardar_gestion_detalle  @IdTipificacion,@IdAsignacion,@CodCanal,@Canal,@DocCliente,@FechaEnvio,@FechaGestion,@HoraGestion,@Telefono,@OrigenTelefono,@CodCampaña,@CodTip,@Oferta,@DocAsesor,@Origen,@ArchivoOrigen,@FechaCarga,@IdDerivacion,@IdSupervisor,@Supervisor,@IdDesembolso"
                    ,parameters);
                if (result == 0)
                {
                    return false;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> UploadTipificacion(ClientesTipificado clientesTipificado)
        {
            try
            {
                _context.clientes_tipificados.Add(clientesTipificado);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}