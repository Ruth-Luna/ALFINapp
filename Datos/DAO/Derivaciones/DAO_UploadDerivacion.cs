using System.Data;
using ALFINapp.API.DTOs;
using ALFINapp.Datos.DAO.Miscelaneos;
using ALFINapp.Datos.DAO.Tipificaciones;
using ALFINapp.DTOs;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Derivaciones
{
    public class DAO_UploadDerivacion
    {
        private readonly MDbContext _context;
        private readonly DAO_ConsultasMiscelaneas _daoConsultasMiscelaneas;
        private readonly DAO_SubirTipificaciones _daoSubirTipificaciones;
        public DAO_UploadDerivacion(
            MDbContext context,
            DAO_ConsultasMiscelaneas daoConsultasMiscelaneas,
            DAO_SubirTipificaciones daoSubirTipificaciones)
        {
            _context = context;
            _daoConsultasMiscelaneas = daoConsultasMiscelaneas;
            _daoSubirTipificaciones = daoSubirTipificaciones;
        }
        public async Task<(bool success, string message)> UploadDerivacion(
            DtoVUploadDerivacion dto)
        {
            try
            {
                if (dto.id_usuario == null)
                {
                    return (false, "El id de usuario no puede estar vacio.");
                }
                var verificacion = await verDisponibilidadDerivacion(dto.id_base);

                if (!verificacion.success)
                {
                    return (false, verificacion.message);
                }
                
                var getCliente = await _daoConsultasMiscelaneas.getBase(dto.id_base);
                if (getCliente == null)
                {
                    return (false, "No se encontró el cliente");
                }

                var derivacion = new DerivacionesAsesores
                {
                    FechaDerivacion = DateTime.Now,
                    FechaVisita = dto.fecha_visita,
                    TelefonoCliente = dto.telefono,
                    NombreAgencia = dto.agencia_comercial
                };

                if (!string.IsNullOrEmpty(dto.nombres_completos))
                {
                    derivacion.NombreCliente = dto.nombres_completos;
                }

                var upload = await uploadNuevaDerivacion(
                    derivacion,
                    dto.id_base,
                    dto.id_usuario.Value);
                if (!upload.success)
                {
                    return (false, upload.message);
                }

                var estado_tipificacion = await _daoSubirTipificaciones.SubirTipificacionesAsync(
                    new List<DtoVTipificarCliente>
                    {
                        new DtoVTipificarCliente
                        {
                            Telefono = dto.telefono,
                            TipificacionId = dto.type,
                            idderivacion = upload.idderivacion
                        }
                    }, dto.id_asignacion, dto.id_usuario.Value);

                if (!estado_tipificacion.success)
                {
                    return (false, estado_tipificacion.message);
                }

                return (true, "Carga de derivación exitosa. Tipificación subida correctamente.");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error al cargar la derivación: " + ex.Message);
            }
        }

        public async Task<(bool success, string message)> verDisponibilidadDerivacion(int idBase)
        {
            try
            {
                var verDerivacion = await _context.resultado_verificacion
                    .FromSqlRaw("EXEC SP_derivaciones_verificar_disponibilidad_para_derivacion_id_base @id_base"
                        , new SqlParameter("@id_base", idBase))
                    .AsNoTracking()
                    .ToListAsync();
                var resultadoVerificacion = verDerivacion.FirstOrDefault();
                if (resultadoVerificacion == null)
                {
                    return (false, "Ha ocurrido un error en la base de datos, intentelo nuevamente");
                }
                if (resultadoVerificacion.Resultado == 0)
                {
                    return (false, resultadoVerificacion.Mensaje);
                }
                else
                {
                    return (true, resultadoVerificacion.Mensaje);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error en la base de datos");
            }
        }
        public async Task<(bool success, string message, int idderivacion)> uploadNuevaDerivacion(
            DerivacionesAsesores derivacion,
            int idBase,
            int idUsuario)
        {
            try
            {
                var parametros = new[]
                {
                    new SqlParameter("@agencia_derivacion", derivacion.NombreAgencia),
                    new SqlParameter("@fecha_visita", derivacion.FechaVisita) { SqlDbType = SqlDbType.DateTime },
                    new SqlParameter("@telefono", derivacion.TelefonoCliente),
                    new SqlParameter("@id_base", idBase),
                    new SqlParameter("@id_usuario", idUsuario),
                    new SqlParameter("@nombre_completos", derivacion.NombreCliente ?? string.Empty)
                };
                var generarDerivacion = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_derivacion_insertar_derivacion_test_N @agencia_derivacion, @fecha_visita, @telefono, @id_base, @id_usuario, @nombre_completos",
                    parametros);
                if (generarDerivacion <= 0)
                {
                    return (false, "Error al subir la derivación", 0);
                }
                return (true, "Derivacion subida correctamente", generarDerivacion);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error en la base de datos al subir la derivacion" + ex.Message, 0);
            }
        }
    }
}