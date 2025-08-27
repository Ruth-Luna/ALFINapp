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