using System.Data;
using ALFINapp.API.Models;
using ALFINapp.Datos.DAO.Miscelaneos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Supervisores
{
    public class DAO_AsignacionSupervisorView
    {
        private readonly DA_Usuario _usuario = new DA_Usuario();
        private readonly DAO_ConsultasMiscelaneas _consultasMiscelaneas;
        private readonly MDbContext _context;
        public DAO_AsignacionSupervisorView(
            MDbContext context,
            DAO_ConsultasMiscelaneas consultasMiscelaneas)
        {
            _context = context;
            _consultasMiscelaneas = consultasMiscelaneas;
        }
        public async Task<(bool IsSuccess, string Message, ViewAsignacionSupervisor Data)> getViewAsignacion(int idUsuario)
        {
            try
            {
                var GetAsesoresAsignados = _usuario.ListarAsesores(idUsuario);
                if (GetAsesoresAsignados == null)
                {
                    return (true, "No se encontraron asesores asignados.", new ViewAsignacionSupervisor());
                }
                var detalles = await GetDetallesAsignacionesPorAsesor(
                    GetAsesoresAsignados.Select(x => x.IdUsuario).ToList(), idUsuario);
                if (!detalles.IsSuccess)
                {
                    return (false, detalles.Message, new ViewAsignacionSupervisor());
                }
                var cantidades = await GetCantidadClientesTotalFromSupervisor(
                    idUsuario);

                int totalClientes = cantidades.total;
                int clientesPendientesSupervisor = cantidades.totalPendientes;
                int clientesAsignadosSupervisor = cantidades.totalAsignados;
                var supervisorData = await GetDetallesAsignacionesPorAsesor(
                    GetAsesoresAsignados.Select(x => x.IdUsuario).ToList(), idUsuario);

                var listas = await _consultasMiscelaneas.GetListas(idUsuario);
                if (!listas.IsSuccess)
                {
                    return (false, listas.Message, new ViewAsignacionSupervisor());
                }
                var destinos = await _consultasMiscelaneas.GetDestinos(idUsuario);
                if (!destinos.IsSuccess)
                {
                    return (false, destinos.Message, new ViewAsignacionSupervisor());
                }
                var bases = await _consultasMiscelaneas.getBases(idUsuario);
                var asignacionSupervisor = new ViewAsignacionSupervisor
                {
                    Asesores = supervisorData.Data,
                    TotalClientes = totalClientes,
                    TotalClientesAsignados = clientesAsignadosSupervisor,
                    TotalClientesPendientes = clientesPendientesSupervisor,
                    Destinos = destinos.Destinos != null ? destinos.Destinos.Where(d => d != null).Cast<string>().ToList() : new List<string>(),
                    ListasAsignacion = listas.Listas != null ? listas.Listas : new List<string>(),
                    BasesAsignacion = bases != null ? bases : new List<string>()
                };
                return (true, "Consulta correcta", asignacionSupervisor);
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al obtener la asignación: {ex.Message}", new ViewAsignacionSupervisor());
            }
        }
        public async Task<(bool IsSuccess, string Message, List<ViewAsignacionAsesor> Data)> GetDetallesAsignacionesPorAsesor(
            List<int> idAsesores,
            int idUsuarioS)
        {
            try
            {
                var table = new DataTable();
                table.Columns.Add("ids", typeof(int));
                foreach (var id in idAsesores)
                    table.Rows.Add(id);
                var tvpParam = new SqlParameter("@Asesores", table)
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.IdLists"
                };
                var result = await _context.supervisores_get_detalles_asignaciones_por_asesores
                    .FromSqlRaw("EXEC SP_SUPERVISORES_GET_DETALLES_ASIGNACIONES_POR_ASESORES @Asesores", tvpParam)
                    .ToListAsync();
                if (result == null || result.Count == 0)
                {
                    return (true, "No se encontraron detalles de asignaciones por asesores.", new List<ViewAsignacionAsesor>());
                }
                var detalles = result
                    .Select(x => new ViewAsignacionAsesor(x))
                    .ToList();
                return (true, "Consulta correcta", detalles);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener la asignación: {ex.Message}", new List<ViewAsignacionAsesor>());
            }
        }

        public async Task<(int total, int totalAsignados, int totalPendientes)> GetCantidadClientesTotalFromSupervisor(
            int idUsuario,
            string filter = "",
            string search = ""
            )
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", idUsuario),
                    new SqlParameter("@Filter", string.IsNullOrEmpty(filter) ? (object)DBNull.Value : filter),
                    new SqlParameter("@Search", string.IsNullOrEmpty(search) ? (object)DBNull.Value : search)
                };
                var result = await _context.supervisor_get_number_of_leads
                    .FromSqlRaw("EXEC sp_supervisor_get_number_of_leads @IdUsuario = @IdUsuario, @Filter = @Filter, @Search = @Search", parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (result == null || !result.Any())
                {
                    Console.WriteLine("No se encontraron datos de clientes.");
                    return (0, 0, 0);
                }
                var resultado = result.FirstOrDefault();
                if (resultado == null)
                {
                    Console.WriteLine("No se encontraron datos de clientes.");
                    return (0, 0, 0);
                }
                return (resultado.TotalClientes, resultado.TotalClientesAsignados, resultado.TotalClientesPendientes);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return (0, 0, 0);
            }
        }
    }
}