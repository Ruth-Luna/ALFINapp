using ALFINapp.Datos.DAO.Miscelaneos;
using ALFINapp.Models;

namespace ALFINapp.Datos.DAO.Tipificaciones
{
    public class DAO_GestionTipificacionesVista
    {
        private readonly MDbContext _context;
        private readonly DAO_TipificacionesConsultas _dao_tipificacionesConsultas;
        private readonly DAO_ConsultasMiscelaneas _dao_consultasMiscelaneas;

        public DAO_GestionTipificacionesVista(
            MDbContext context,
            DAO_TipificacionesConsultas dao_tipificacionesConsultas,
            DAO_ConsultasMiscelaneas dao_consultasMiscelaneas)
        {
            _context = context;
            _dao_tipificacionesConsultas = dao_tipificacionesConsultas;
            _dao_consultasMiscelaneas = dao_consultasMiscelaneas;
        }
        public async Task<(bool IsSuccess, string Message, ViewClienteDetallado lista)> GetClienteTipificacion(
            int id_cliente,
            int id_usuario_v,
            string traido_de = "A365"
        )
        {
            try
            {
                var agencias = await _dao_consultasMiscelaneas.GetDestinos(id_usuario_v);
                if (traido_de == "A365")
                {
                    var cliente = await _dao_tipificacionesConsultas.GetClienteA365(id_cliente, id_usuario_v);

                    return (true, "Cliente encontrado en A365", cliente.lista);
                }
                else if (traido_de == "ALFIN")
                {
                    var cliente = await _dao_tipificacionesConsultas.GetClienteAlfin(id_cliente, id_usuario_v);

                    return (true, "Cliente encontrado en ALFIN", cliente.lista);
                }
                return (true, "Clientes asignados obtenidos correctamente", new ViewClienteDetallado());
            }
            catch (System.Exception)
            {
                return (false, "Error al obtener los clientes asignados", new ViewClienteDetallado());
            }
            
        }
    }
}