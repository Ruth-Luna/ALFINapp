namespace ALFINapp.Datos.DAO.Tipificaciones
{
    public class DAO_GestionTipificacionesVista
    {
        private readonly MDbContext _context;
        public DAO_GestionTipificacionesVista(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, string Message, object? lista)> GetClienteTipificacion(
            int id_cliente,
            string traido_de = "A365"
        )
        {
            try
            {
                return (true, "Clientes asignados obtenidos correctamente", "en proceso");
            }
            catch (System.Exception)
            {
                return (false, "Error al obtener los clientes asignados", null);
            }
            
        }
    }
}