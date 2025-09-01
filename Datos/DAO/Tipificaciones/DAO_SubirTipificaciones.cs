using System.Data;
using ALFINapp.API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Tipificaciones
{
    public class DAO_SubirTipificaciones
    {
        private readonly MDbContext _context;
        private readonly DAO_ActualizarComentarios _dao_ActualizarComentarios;

        public DAO_SubirTipificaciones(
            MDbContext context,
            DAO_ActualizarComentarios dAO_ActualizarComentarios)
        {
            _context = context;
            _dao_ActualizarComentarios = dAO_ActualizarComentarios;
        }

        public async Task<(bool success, string message)> SubirTipificacionesAsync(
            List<DtoVTipificarCliente> tipificaciones,
            int id_asignacion,
            int id_usuario)
        {
            try
            {
                var message = "No hay tipificaciones para subir.";
                if (tipificaciones == null || !tipificaciones.Any())
                {
                    return (false, message);
                }
                var mappedTipificaciones = await MapearYVerificarTipificaciones(tipificaciones);
                if (!mappedTipificaciones.success)
                {
                    return (false, mappedTipificaciones.message);
                }
                var tipificacionesListas = mappedTipificaciones.mappedTipificaciones;

                if (tipificacionesListas.Count == 0)
                {
                    return (false, "No se encontraron tipificaciones válidas para procesar.");
                }

                var result_tipificaciones = await subirClienteTipificado(
                    tipificacionesListas,
                    id_asignacion,
                    id_usuario);

                if (!result_tipificaciones.success)
                {
                    return (false, result_tipificaciones.message);
                }

                var estadoActualizado = await actualizarEstadoTipificacion(
                    result_tipificaciones.tipificaciones,
                    id_asignacion
                    );
                if (!estadoActualizado.success)
                {
                    return (false, estadoActualizado.message);
                }
                foreach (var tipificacion in tipificacionesListas)
                {
                    if (!string.IsNullOrEmpty(tipificacion.Comentario))
                    {
                        var comentarioActualizado = await _dao_ActualizarComentarios.ActualizarComentarioAsync(
                            tipificacion.Telefono,
                            id_asignacion,
                            tipificacion.Comentario);
                            
                        if (!comentarioActualizado.success)
                        {
                            return (false, comentarioActualizado.message);
                        }
                    }
                }

                return (true, "Tipificaciones subidas y estado del cliente actualizado correctamente. " +
                               $"Se han procesado {result_tipificaciones.tipificaciones.Count} tipificaciones. " +
                               $"{mappedTipificaciones.message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error al subir las tipificaciones: {ex.Message}");
            }
        }
        public async Task<(bool success, string message, List<DtoVTipificarCliente> mappedTipificaciones)> MapearYVerificarTipificaciones(
            List<DtoVTipificarCliente> tipificaciones)
        {
            try
            {
                var message = "";
                var mappedTipificaciones = new List<DtoVTipificarCliente>();
                if (tipificaciones == null || !tipificaciones.Any())
                {
                    return (false, "No hay tipificaciones para procesar.", mappedTipificaciones);
                }
                foreach (var tipificacion in tipificaciones)
                {
                    if (
                        tipificacion == null
                        || tipificacion.TipificacionId <= 0
                        || tipificacion.Telefono == "0"
                        || tipificacion.Telefono == " "
                        || string.IsNullOrEmpty(tipificacion.Telefono))
                    {
                        continue;
                    }
                    if (tipificacion.TipificacionId == 2 && tipificacion.idderivacion == null)
                    {
                        return (false, "La tipificación DE DERIVACIÓN requiere un ID de derivación.", new List<DtoVTipificarCliente>());
                    }
                    if (tipificacion.TipificacionId == 2)
                    {
                        var outputParam = new SqlParameter
                        {
                            ParameterName = "@resultado",
                            SqlDbType = SqlDbType.Int,
                            Direction = ParameterDirection.Output
                        };

                        var parameters = new[]
                        {
                            new SqlParameter("@idcliente", tipificacion.Telefono),
                            new SqlParameter("@tipificacion_id", tipificacion.TipificacionId),
                            new SqlParameter("@id_derivacion", tipificacion.idderivacion ?? (object)DBNull.Value),
                            outputParam
                        };
                        await _context.Database.ExecuteSqlRawAsync(
                            "EXEC SP_tipificacion_verificar_tipificacion_cliente @idcliente, @tipificacion_id, @id_derivacion, @resultado OUTPUT",
                            parameters);

                        var result = (int)(outputParam.Value ?? 0);
                        if (result <= 0)
                        {
                            return (false, "Error al verificar la tipificación de derivación.", new List<DtoVTipificarCliente>());
                        }
                        message = "Tipificación de derivación verificada correctamente.";
                    }
                    tipificacion.Telefono = tipificacion.Telefono.Trim();
                    tipificacion.Comentario = tipificacion.Comentario?.Trim() ?? string.Empty;
                    mappedTipificaciones.Add(tipificacion);
                }

                if (mappedTipificaciones.Count == 0)
                {
                    return (false, "No se encontraron tipificaciones válidas para procesar.", new List<DtoVTipificarCliente>());
                }
                message += "Todas las tipificaciones han sido mapeadas y verificadas correctamente.";
                return (true, message, mappedTipificaciones);
            }
            catch (Exception ex)
            {
                return (false, $"Error al mapear y verificar las tipificaciones: {ex.Message}", new List<DtoVTipificarCliente>());
            }
        }
        public async Task<(bool success, string message, List<(int id_tipificacion, string telefono, int id_cliente_tip)> tipificaciones)> subirClienteTipificado(
            List<DtoVTipificarCliente> dtos,
            int idAsignacion,
            int idUsuario)
        {
            try
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return (false, "No hay tipificaciones para subir.", new List<(int id_tipificacion, string telefono, int id_cliente_tip)>());
                }
                var tipificaciones = new List<(int id_tipificacion, string telefono, int id_cliente_tip)>();
                foreach (var dto in dtos)
                {
                    var outputParam = new SqlParameter("@id_cliente_tip", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var parameters = new[]
                    {
                        new SqlParameter("@telefono", dto.Telefono),
                        new SqlParameter("@id_tipificacion", dto.TipificacionId),
                        new SqlParameter("@id_asignacion", idAsignacion),
                        new SqlParameter("@id_usuario", idUsuario),
                        outputParam
                    };
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC SP_tipificaciones_subir_cliente_tipificado @telefono, @id_tipificacion, @id_asignacion, @id_usuario, @id_cliente_tip OUTPUT",
                        parameters);
                    int insertedId = (int)outputParam.Value;
                    if (!string.IsNullOrEmpty(dto.Telefono) && insertedId > 0)
                    {
                        tipificaciones.Add((dto.TipificacionId, dto.Telefono, insertedId));
                    }
                }
                return (true, "Todas las tipificaciones se han subido correctamente.", tipificaciones);
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al tipificar el cliente: {ex.Message}", new List<(int id_tipificacion, string telefono, int id_cliente_tip)>());
            }
        }

        public async Task<(bool success, string message)> actualizarEstadoTipificacion(
            List<(int id_tipificacion, string telefono, int id_cliente_tip)> tipificaciones,
            int id_asignacion
            )
        {
            try
            {
                foreach (var tipificacion in tipificaciones)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@telefono", tipificacion.telefono),
                        new SqlParameter("@id_tipificacion", tipificacion.id_tipificacion),
                        new SqlParameter("@id_asignacion", id_asignacion),
                        new SqlParameter("@id_cliente_tip", tipificacion.id_cliente_tip)
                    };
                    var result = await _context.Database.ExecuteSqlRawAsync(
                        "EXEC SP_tipificaciones_actualizar_estado_tipificacion @telefono, @id_tipificacion, @id_asignacion, @id_cliente_tip",
                        parameters);
                    if (result <= 0)
                    {
                        return (false, $"Error al actualizar el estado de la tipificación para el teléfono {tipificacion.telefono}.");
                    }
                }
                return (true, "Estado de las tipificaciones actualizado correctamente.");
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al actualizar el estado de la tipificación: {ex.Message}");
            }
        }
    }
}