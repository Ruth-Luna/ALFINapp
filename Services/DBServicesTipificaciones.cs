using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using System.Data;

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
                        return (false, "No se encontro la derivacion del cliente, recuerde que debe mandar la derivacion antes de guardar la tipificacion");
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
                    new SqlParameter("@HoraGestion", Tipificacion.FechaTipificacion != null ? 
                        (object)Tipificacion.FechaTipificacion.Value.TimeOfDay : DBNull.Value) 
                    { 
                        SqlDbType = SqlDbType.Time 
                    },
                    new SqlParameter("@Telefono", Tipificacion.TelefonoTipificado),
                    new SqlParameter("@OrigenTelefono", "E"),
                    new SqlParameter("@CodCampaña", detalle_base.Campaña),
                    new SqlParameter("@CodTip", Tipificacion.IdTipificacion),
                    new SqlParameter("@Oferta", detalle_base.OfertaMax),
                    new SqlParameter("@DocAsesor", asesorDatos.Dni),
                    new SqlParameter("@Origen", Tipificacion.Origen),
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
                    return (false, "No se pudo guardar la gestion en la base de datos");
                }
                return (true, "La Gestion se ha guardado correctamente");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message, Tipificaciones? Data)> ObtenerTipificacion(int idTipificacion)
        {
            try
            {
                var tipificacion = await _context.tipificaciones
                    .Where(t => t.IdTipificacion == idTipificacion)
                    .FirstOrDefaultAsync();
                if (tipificacion == null)
                {
                    return (false, "No se encontro la tipificacion en la base de datos", null);
                }
                return (true, "Se encontro la tipificacion en la base de datos", tipificacion);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message)> GuardarNuevaTipificacion(ClientesTipificado clienteTipificado)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@id_asignacion", clienteTipificado.IdAsignacion),
                    new SqlParameter("@id_tipificacion", clienteTipificado.IdTipificacion),
                    new SqlParameter("@fecha_tipificacion", clienteTipificado.FechaTipificacion ?? (object)DBNull.Value)
                    { 
                        SqlDbType = SqlDbType.DateTime 
                    },
                    new SqlParameter("@origen", clienteTipificado.Origen!=null ? clienteTipificado.Origen.Trim(): "nuevo")
                    {
                        SqlDbType = SqlDbType.VarChar,
                        Size = 40
                    },
                    new SqlParameter("@telefono_tipificado", clienteTipificado.TelefonoTipificado),
                    new SqlParameter("@derivacion_fecha", clienteTipificado.DerivacionFecha ?? (object)DBNull.Value)
                    { 
                        SqlDbType = SqlDbType.DateTime 
                    },
                };
                var result = await _context.Database.ExecuteSqlRawAsync("EXECUTE SP_tipificacion_insertar_nueva_tipificacion @id_asignacion, @id_tipificacion, @fecha_tipificacion, @origen, @telefono_tipificado, @derivacion_fecha"
                ,parameters);
                if (result == 0)
                {
                    return (false, "No se pudo guardar la tipificacion en la base de datos");
                }
                return (true, "La tipificacion se ha guardado correctamente");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
        // Other DB services can be added here
    }
}