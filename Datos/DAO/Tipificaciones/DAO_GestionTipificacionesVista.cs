namespace ALFINapp.Datos.DAO.Tipificaciones
{
    public class DAO_GestionTipificacionesVista
    {
        private readonly MDbContext _context;
        private readonly DAO_TipificacionesConsultas _dao_tipificacionesConsultas;

        public DAO_GestionTipificacionesVista(
            MDbContext context,
            DAO_TipificacionesConsultas dao_tipificacionesConsultas)
        {
            _context = context;
            _dao_tipificacionesConsultas = dao_tipificacionesConsultas;
        }
        public async Task<(bool IsSuccess, string Message, object? lista)> GetClienteTipificacion(
            int id_cliente,
            int id_usuario_v,
            string traido_de = "A365"
        )
        {
            try
            {
                if (traido_de == "A365")
                {
                    var cliente = await _dao_tipificacionesConsultas.GetClienteA365(id_cliente, id_usuario_v);
                    
                    return (true, "Cliente encontrado en A365", cliente);
                }
                return (true, "Clientes asignados obtenidos correctamente", "en proceso");
            }
            catch (System.Exception)
            {
                return (false, "Error al obtener los clientes asignados", null);
            }
            
        }
    }
}