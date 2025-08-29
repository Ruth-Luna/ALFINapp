using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Tipificaciones
{
    public class DAO_ActualizarComentarios
    {
        private readonly MDbContext _context;

        public DAO_ActualizarComentarios(MDbContext context)
        {
            _context = context;
        }
        
        public async Task<(bool success, string message)> ActualizarComentarioAsync(
            string telefono,
            int idCliente,
            string comentario)
        {
            try
            {
                if (string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(comentario))
                {
                    return (false, "Los Campos enviados estan Vacios.");
                }

                var registro = await _context.telefonos_agregados
                    .FirstOrDefaultAsync(ta => ta.Telefono == telefono && ta.IdCliente == idCliente);

                if (registro != null)
                {
                    registro.Comentario = comentario;
                    await _context.SaveChangesAsync();
                    return (true, "Comentario guardado correctamente.");
                }
                else
                {
                    var cliente_telefono = await _context.clientes_enriquecidos
                        .FirstOrDefaultAsync(ct => ct.Telefono1 == telefono);
                    if (cliente_telefono != null)
                    {
                        cliente_telefono.ComentarioTelefono1 = comentario;
                        await _context.SaveChangesAsync();
                        return (true, "Comentario guardado correctamente.");
                    }
                    var cliente_telefono2 = await _context.clientes_enriquecidos
                        .FirstOrDefaultAsync(ct => ct.Telefono2 == telefono);
                    if (cliente_telefono2 != null)
                    {
                        cliente_telefono2.ComentarioTelefono2 = comentario;
                        await _context.SaveChangesAsync();
                        return (true, "Comentario guardado correctamente.");
                    }
                    var cliente_telefono3 = await _context.clientes_enriquecidos
                        .FirstOrDefaultAsync(ct => ct.Telefono3 == telefono);
                    if (cliente_telefono3 != null)
                    {
                        cliente_telefono3.ComentarioTelefono3 = comentario;
                        await _context.SaveChangesAsync();
                        return (true, "Comentario guardado correctamente.");
                    }
                    var cliente_telefono4 = await _context.clientes_enriquecidos
                        .FirstOrDefaultAsync(ct => ct.Telefono4 == telefono);
                    if (cliente_telefono4 != null)
                    {
                        cliente_telefono4.ComentarioTelefono4 = comentario;
                        await _context.SaveChangesAsync();
                        return (true, "Comentario guardado correctamente.");
                    }
                    var cliente_telefono5 = await _context.clientes_enriquecidos
                        .FirstOrDefaultAsync(ct => ct.Telefono5 == telefono);
                    if (cliente_telefono5 != null)
                    {
                        cliente_telefono5.ComentarioTelefono5 = comentario;
                        await _context.SaveChangesAsync();
                        return (true, "Comentario guardado correctamente.");
                    }
                    return (false, "Número no encontrado.");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}