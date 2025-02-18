using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;

namespace ALFINapp.Services
{
    public class DBServicesTipificaciones
    {
        private readonly MDbContext _context;

        public DBServicesTipificaciones(MDbContext context)
        {
            _context = context;
        }


        public async Task<(bool IsSuccess, string Message)> EnviarFomularioDerivacion(DerivacionesAsesores derivarCliente)
        {
            try
            {
                if (derivarCliente == null)
                {
                    return (false, "El objeto derivarCliente no puede ser nulo");
                }

                // Agregar el objeto a la base de datos
                _context.derivaciones_asesores.Add(derivarCliente);
                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
                return (true, "El Formulario de Derivacion se ha enviado correctamente");
            }
            catch (System.Exception ex)
            {
                return (true, ex.Message);
            }
        }
        public async Task<(bool IsSuccess, string Message)> GuardarGestionDetalle(
            ClientesAsignado ClienteAsignado,
            ClientesTipificado Tipificacion,
            ClientesEnriquecido Enriquecido)
        {
            try
            {
                var clienteDatos = await (from bc in _context.base_clientes
                                  where bc.IdBase == Enriquecido.IdBase
                                  select bc)
                                .FirstOrDefaultAsync();
                if (clienteDatos == null)
                {
                    return (false, "El cliente Tipificado no se encuentro en la base de datos");
                }
                var asesorDatos = await (from u in _context.usuarios
                                where u.IdUsuario == ClienteAsignado.IdUsuarioV
                                select u)
                                .FirstOrDefaultAsync();
                if (asesorDatos == null)
                {
                    return (false, "El asesor asignado no se encuentro en la base de datos");
                }
                var detalle_base = new DetalleBase();
                if (clienteDatos.IdBaseBanco == null)
                {
                    detalle_base = (from db in _context.detalle_base
                                    where db.IdBase == clienteDatos.IdBase
                                    orderby db.FechaCarga descending
                                    select db)
                                    .FirstOrDefault();
                }
                else
                {
                    detalle_base = (from bcb in _context.base_clientes_banco
                                    join cg in _context.base_clientes_banco_campana_grupo on bcb.IdCampanaGrupoBanco equals cg.IdCampanaGrupo
                                    where bcb.IdBaseBanco == clienteDatos.IdBaseBanco
                                    orderby bcb.FechaSubida descending
                                    select new DetalleBase
                                    {
                                        OfertaMax = bcb.OfertaMax,
                                        Campaña = cg.NombreCampana,
                                    }).FirstOrDefault();
                }
                if (detalle_base == null)
                {
                    return (false, "No se encontro detalle de base para el cliente tipificado");
                }
                var derivacionBusqueda = new DerivacionesAsesores();
                if (Tipificacion.IdTipificacion == 2)
                {
                    derivacionBusqueda = await (from da in _context.derivaciones_asesores
                                        where da.IdCliente == clienteDatos.IdBase
                                        select da)
                                        .FirstOrDefaultAsync();
                    if (derivacionBusqueda == null)
                    {
                        return (false, "No se encontro la derivacion del cliente");
                    }
                }

                var parameters = new[]
                {
                    new SqlParameter("@IdTipificacion", Tipificacion.IdClientetip),
                    new SqlParameter("@IdAsignacion", ClienteAsignado.IdAsignacion),
                    new SqlParameter("@CodCanal", "SYSTEMA365"),
                    new SqlParameter("@Canal", "A365"),
                    new SqlParameter("@DocCliente", clienteDatos.Dni),
                    new SqlParameter("@FechaEnvio", DateTime.Now),
                    new SqlParameter("@FechaGestion", Tipificacion.FechaTipificacion),
                    new SqlParameter("@HoraGestion", Tipificacion.FechaTipificacion != null ? Tipificacion.FechaTipificacion.Value.Date : DBNull.Value),
                    new SqlParameter("@Telefono", Tipificacion.TelefonoTipificado),
                    new SqlParameter("@OrigenTelefono", "E"),
                    new SqlParameter("@CodCampaña", detalle_base.Campaña),
                    new SqlParameter("@CodTip", Tipificacion.IdTipificacion),
                    new SqlParameter("@Oferta", detalle_base.OfertaMax),
                    new SqlParameter("@DocAsesor", asesorDatos.Dni),
                    new SqlParameter("@Origen", "SISTEMA A365"),
                    new SqlParameter("@FechaCarga", DateTime.Now),
                    new SqlParameter("@IdDerivacion", derivacionBusqueda!=null?derivacionBusqueda.IdDerivacion:null),
                    new SqlParameter("@IdSupervisor", asesorDatos.IDUSUARIOSUP),
                    new SqlParameter("@Supervisor", asesorDatos.RESPONSABLESUP),
                };
                var result = await _context.Database.ExecuteSqlRawAsync("EXECUTE SP_Tipificaciones_guardar_gestion_detalle  @IdTipificacion,@IdAsignacion,@CodCanal,@Canal,@DocCliente,@FechaEnvio,@FechaGestion,@HoraGestion,@Telefono,@OrigenTelefono,@CodCampaña,@CodTip,@Oferta,@DocAsesor,@Origen,@FechaCarga,@IdDerivacion,@IdSupervisor,@Supervisor"
                    ,parameters);
                if (result == 0)
                {
                    return (false, "No se pudo guardar la gestion en la base de datos");
                }
                return (true, "La Gestion se ha guardado correctamente");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
        // Other DB services can be added here
    }
}