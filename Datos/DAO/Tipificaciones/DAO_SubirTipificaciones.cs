using ALFINapp.API.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Tipificaciones
{
    public class DAO_SubirTipificaciones
    {
        private readonly MDbContext _context;

        public DAO_SubirTipificaciones(MDbContext context)
        {
            _context = context;
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
                // Si todo sale bien, retornamos el mensaje de éxito
                return (true, "Tipificaciones subidas y estado del cliente actualizado correctamente. + " +
                               $"{result_tipificaciones.tipificaciones.Count} tipificaciones procesadas." +
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
                foreach (var tipificacion in tipificaciones)
                {
                    if (tipificacion == null || string.IsNullOrEmpty(tipificacion.Telefono))
                    {
                        continue;
                    }
                    if (tipificacion.TipificacionId == 2 && tipificacion.idderivacion == null)
                    {
                        return (false, "La tipificación DE DERIVACION requiere un ID de derivación.", new List<DtoVTipificarCliente>());
                    }
                    if (tipificacion.TipificacionId == 2)
                    {

                        var parameters = new[]
                        {
                            new SqlParameter("@idcliente", tipificacion.Telefono),
                            new SqlParameter("@tipificacion_id", tipificacion.TipificacionId),
                            new SqlParameter("@id_derivacion", tipificacion.idderivacion ?? (object)DBNull.Value),
                        };
                        var verificarderivacion = await _context.Database.ExecuteSqlRawAsync(
                            "EXEC SP_tipificacion_verificar_tipificacion_cliente @idcliente, @tipificacion_id, @id_derivacion",
                            parameters);

                        if (verificarderivacion <= 0)
                        {
                            return (false, "Error al verificar la tipificación de derivación.", new List<DtoVTipificarCliente>());
                        }
                        message = "Tipificación de derivación verificada correctamente.";
                    }
                    tipificacion.Telefono = tipificacion.Telefono.Trim();
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
        public async Task<(bool success, string message, List<(int id_tipificacion, string telefono)> tipificaciones)> subirClienteTipificado(
            List<DtoVTipificarCliente> dtos,
            int idAsignacion,
            int idUsuario)
        {
            try
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return (false, "No hay tipificaciones para subir.", new List<(int id_tipificacion, string telefono)>());
                }
                var tipificaciones = new List<(int id_tipificacion, string telefono)>();
                foreach (var dto in dtos)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@telefono", dto.Telefono),
                        new SqlParameter("@id_tipificacion", dto.TipificacionId),
                        new SqlParameter("@id_asignacion", idAsignacion),
                        new SqlParameter("@id_usuario", idUsuario)
                    };
                    var result = await _context.Database.ExecuteSqlRawAsync(
                        "EXEC SP_tipificaciones_subir_cliente_tipificado @telefono, @id_tipificacion, @id_asignacion, @id_usuario",
                        parameters);
                    if (result > 0)
                    {
                        if (!string.IsNullOrEmpty(dto.Telefono))
                        {
                            tipificaciones.Add((dto.TipificacionId, dto.Telefono));
                        }
                        continue; // Si la tipificación se subió correctamente, continuamos con la siguiente
                    }
                }
                return (true, "Todas las tipificaciones se han subido correctamente.", tipificaciones);
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al tipificar el cliente: {ex.Message}", new List<(int id_tipificacion, string telefono)>());
            }
        }

        public async Task<(bool success, string message)> actualizarEstadoTipificacion(
            List<(int id_tipificacion, string telefono)> tipificaciones,
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
                        new SqlParameter("@id_asignacion", id_asignacion)
                    };
                    var result = await _context.Database.ExecuteSqlRawAsync(
                        "EXEC SP_tipificaciones_actualizar_estado_tipificacion @telefono, @id_tipificacion, @id_asignacion",
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