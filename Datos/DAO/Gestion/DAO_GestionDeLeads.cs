using ALFINapp.API.Models;
using ALFINapp.Datos.DAO.Miscelaneos;

namespace ALFINapp.Datos.DAO.Gestion
{
    public class DAO_GestionDeLeads
    {
        private readonly MDbContext _context;
        private readonly DAO_ConsultasMiscelaneas _dao_consultasMiscelaneas;
        private readonly DAO_GestionDeLeadsAsesor _dao_gestionDeLeadsAsesor;
        private readonly DAO_GestionDeLeadsSupervisor _dao_gestionDeLeadsSupervisor;
        public DAO_GestionDeLeads(
            MDbContext context,
            DAO_ConsultasMiscelaneas dao_consultasMiscelaneas,
            DAO_GestionDeLeadsAsesor daoGestionDeLeadsAsesor,
            DAO_GestionDeLeadsSupervisor daoGestionDeLeadsSupervisor)
        {
            _context = context;
            _dao_consultasMiscelaneas = dao_consultasMiscelaneas;
            _dao_gestionDeLeadsAsesor = daoGestionDeLeadsAsesor;
            _dao_gestionDeLeadsSupervisor = daoGestionDeLeadsSupervisor;
        }
        public async Task<(bool IsSuccess, string Message, ViewGestionLeads Data)> GetLeadsAsignados(
            int usuarioId,
            int intervaloInicio = 0,
            int intervaloFin = 1,
            string filter = "",
            string search = "",
            string order = "tipificacion",
            bool orderAsc = true)
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                var usuario = await _dao_consultasMiscelaneas.getuser(usuarioId);
                if (usuario == null)
                {
                    return (false, "No se encontró el usuario", new ViewGestionLeads());
                }
                var clientes = new List<ViewCliente>();
                if (usuario.IdRol == 3)
                {
                    var clientesResult = await _dao_gestionDeLeadsAsesor.GetLeadsAsignadosAsesorPaginado(
                        usuarioId,
                        intervaloInicio,
                        intervaloFin,
                        filter,
                        search,
                        order,
                        orderAsc);
                    if (!clientesResult.IsSuccess || clientesResult.Data == null)
                    {
                        return (false, clientesResult.Message, new ViewGestionLeads());
                    }
                    clientes = clientesResult.Data;
                    var cantidades = await _dao_gestionDeLeadsAsesor.GetCantidadesAsignadosAsesor(usuarioId);
                    if (!cantidades.IsSuccess)
                    {
                        return (false, cantidades.Message, new ViewGestionLeads());
                    }
                    var convertView = new ViewGestionLeads
                    {
                        ClientesA365 = clientes,
                        Vendedor = new ViewUsuario(usuario),
                        Supervisor = new ViewUsuario(),
                        ClientesAlfin = new List<ViewCliente>(), // Asignar lista vacía si no hay clientes Alfin
                        clientesPendientes = cantidades.Pendientes,
                        clientesTipificados = cantidades.Tipificados,
                        clientesTotal = cantidades.Total,
                        PaginaActual = intervaloInicio + 1,
                        filtro = filter,
                        searchfield = search,
                        order = order,
                        orderAsc = orderAsc
                    };
                    return (true, "Leads obtenidos correctamente", convertView);
                }
                else if (usuario.IdRol == 2)
                {
                    var clientesResult = await _dao_gestionDeLeadsSupervisor.GetLeadsAsignadosSupervisorPaginado(
                        usuarioId,
                        filter,
                        search,
                        order,
                        orderAsc,
                        intervaloInicio,
                        intervaloFin);
                    if (!clientesResult.IsSuccess || clientesResult.Data == null)
                    {
                        return (false, clientesResult.Message, new ViewGestionLeads());
                    }
                    var destinos = await _dao_consultasMiscelaneas.GetDestinos(usuarioId);
                    var listas = await _dao_consultasMiscelaneas.GetListas(usuarioId);
                    var cantidades = await _dao_gestionDeLeadsSupervisor.GetLeadsAsignadosSupervisorCantidades(
                        usuarioId,
                        filter,
                        search);
                    var convertView = new ViewGestionLeads
                    {
                        ClientesA365 = clientesResult.Data,
                        Supervisor = new ViewUsuario(usuario),
                        ClientesAlfin = new List<ViewCliente>(), // Asignar lista vacía si no hay clientes Alfin
                        clientesPendientes = cantidades.totalPendientes,
                        clientesTipificados = cantidades.totalAsignados,
                        clientesTotal = cantidades.total,
                        PaginaActual = intervaloInicio + 1,
                        filtro = filter,
                        searchfield = search,
                        order = order,
                        orderAsc = orderAsc,
                        destinoBases = destinos.Destinos.Where(d => d != null).Cast<string>().ToList(),
                        listasAsignacion = listas.Listas.Where(l => l != null).Cast<string>().ToList()
                    };
                    return (true, "Leads obtenidos correctamente", convertView);
                }
                else
                {
                    return (false, "Rol de usuario no reconocido", new ViewGestionLeads());
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener los leads: {ex.Message}", new ViewGestionLeads());
            }
        }
    }
}