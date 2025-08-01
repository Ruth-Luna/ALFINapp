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
    public class DAO_GestionDeLeadsAsesor
    {
        private readonly MDbContext _context;
        private readonly DAO_ConsultasMiscelaneas _dao_consultasMiscelaneas;
        public DAO_GestionDeLeadsAsesor(
            MDbContext context,
            DAO_ConsultasMiscelaneas dao_consultasMiscelaneas)
        {
            _context = context;
            _dao_consultasMiscelaneas = dao_consultasMiscelaneas;
        }
        public async Task<(bool IsSuccess, string Message, List<ViewCliente> Data)> GetLeadsAsignadosAsesorPaginado(
            int IdUsuarioVendedor,
            int IntervaloInicio,
            int IntervaloFin,
            string? filter,
            string? searchfield,
            string? order,
            bool orderAsc)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuarioVendedor", IdUsuarioVendedor),
                    new SqlParameter("@Search", searchfield),
                    new SqlParameter("@Filter", filter),
                    new SqlParameter("@IntervaloInicio", IntervaloInicio),
                    new SqlParameter("@IntervaloFinal", IntervaloFin),
                    new SqlParameter("@Ordenamiento", order),
                    new SqlParameter("@OrdenamientoAsc", orderAsc)
                };

                var getAllBase = await _context
                    .leads_get_clientes_asignados_gestion_leads
                    .FromSqlRaw("EXECUTE sp_leads_get_clientes_asignados_for_gestion_de_leads_filtro_y_ordenamiento_general @IdUsuarioVendedor, @Search, @Filter, @IntervaloInicio, @IntervaloFinal, @Ordenamiento, @OrdenamientoAsc",
                    parameters)
                    .ToListAsync();
                if (getAllBase.Count == 0)
                {
                    return (true, "No se encontraron clientes asignados al asesor", new List<ViewCliente>());
                }
                return (true, "No se encontraron clientes asignados al asesor",
                    getAllBase.Select(c => new ViewCliente(c)).ToList());
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener los leads asignados al asesor: {ex.Message}", new List<ViewCliente>());
            }
        }
        
        public async Task<(bool IsSuccess, string Message, int Total, int Tipificados, int Pendientes)> GetCantidadesAsignadosAsesor(int IdUsuarioVendedor)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuarioVendedor", IdUsuarioVendedor)
                };
                var getCantidad = await _context
                    .leads_get_clientes_asignados_cantidades
                    .FromSqlRaw("EXECUTE sp_leads_get_clientes_asignados_cantidades @IdUsuarioVendedor",
                    parameters)
                    .ToListAsync();
                var cantidades = getCantidad.FirstOrDefault();
                if (cantidades == null)
                {
                    return (true, "No se encontraron clientes", 0, 0, 0);
                }
                return (true, "Non Implemented", cantidades.Total, cantidades.Gestionados, cantidades.Pendientes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los clientes asignados al asesor: {ex.Message}");
                return (false, $"Error al obtener los clientes asignados al asesor: {ex.Message}", 0, 0, 0);
            }
        }
    }
}