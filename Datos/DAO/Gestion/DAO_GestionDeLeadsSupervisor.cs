using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Datos.DAO.Miscelaneos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Gestion
{
    public class DAO_GestionDeLeadsSupervisor
    {
        private readonly MDbContext _context;
        private readonly DAO_ConsultasMiscelaneas _dao_consultasMiscelaneas;
        public DAO_GestionDeLeadsSupervisor(
            MDbContext context,
            DAO_ConsultasMiscelaneas dao_consultasMiscelaneas)
        {
            _context = context;
            _dao_consultasMiscelaneas = dao_consultasMiscelaneas;
        }
        public async Task<(bool IsSuccess, string Message, List<ViewCliente> Data)> GetLeadsAsignadosSupervisorPaginado(
            int idUsuario,
            string filter = "",
            string search = "",
            string order = "tipificacion",
            bool orderAsc = true,
            int intervaloInicio = 0,
            int intervaloFin = 1)
        {
            try
            {
                var hoy = DateTime.Now;
                var añoActual = hoy.Year;
                var mesActual = hoy.Month;

                var supervisorData = await _context.supervisor_get_asignacion_leads.FromSqlRaw(
                    "EXEC dbo.sp_supervisor_get_asignacion_de_leads @IdUsuario = {0}", new SqlParameter("@IdUsuario", idUsuario))
                    .ToListAsync();
                if (!supervisorData.Any())
                {
                    Console.WriteLine("No hay clientes asignados al supervisor.");
                    return (true, "No se encontraron clientes asignados al supervisor", new List<ViewCliente>());
                }
                var detallesClientes = supervisorData.Select(cliente => new ViewCliente(cliente)).ToList();
                return (true, "Clientes asignados al supervisor obtenidos correctamente", detallesClientes);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener los clientes asignados al supervisor: {ex.Message}", new List<ViewCliente>());
            }
        }

        public async Task<(int total, int totalAsignados, int totalPendientes)> GetLeadsAsignadosSupervisorCantidades(
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