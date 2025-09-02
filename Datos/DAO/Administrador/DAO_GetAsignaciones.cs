using ALFINapp.API.DTOs;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Administrador
{
    public class DAO_GetAsignaciones
    {
        private readonly MDbContext _context;
        private DA_Usuario _dausuario = new DA_Usuario();
        public DAO_GetAsignaciones(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, string Message, ViewVerAsignacionesDelSupervisor asignaciones)> GetAllAssignmentsFromSupervisor()
        {
            try
            {
                var result = await _context.gestion_conseguir_todas_las_asignaciones_por_listas
                    .FromSqlRaw("EXEC dbo.SP_GESTION_CONSEGUIR_TODAS_LAS_ASIGNACIONES_POR_LISTAS")
                    .ToListAsync();
                if (result == null || result.Count == 0)
                {
                    return (false, "No se encontraron asignaciones para el supervisor. O es probable que no hayan listas existentes durante este mes", new ViewVerAsignacionesDelSupervisor());
                }
                var detalles = result.Select(dto => new ViewListasAsignacionesPorSupervisor(dto)).ToList();
                return (true, "Asignaciones obtenidas correctamente.", new ViewVerAsignacionesDelSupervisor { asignaciones = detalles });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al obtener las asignaciones: {ex.Message}");
                return (false, $"Error en la base de datos al obtener las listas de asignacion", new ViewVerAsignacionesDelSupervisor());
            }
        }
        public async Task<(bool success, string message, DetallesAsignacionesDescargaSupDTO data)> getAsiggnmentsForDownload(string? nombre_lista, int page = -1)
        {
            try
            {
                if (string.IsNullOrEmpty(nombre_lista))
                {
                    return (false, "El nombre de la lista no puede estar vacío.", new DetallesAsignacionesDescargaSupDTO());
                }
                var dni_supervisor = nombre_lista.Split('_')[0];

                var usuario = _dausuario.getUsuarioPorDni(dni_supervisor);
                if (usuario == null)
                {
                    return (false, "El supervisor no existe o no tiene asignaciones.", new DetallesAsignacionesDescargaSupDTO());
                }
                
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@NombreLista", nombre_lista),
                    new SqlParameter("@Pagina", page)
                };
                _context.Database.SetCommandTimeout(600);
                var result = await _context.gestion_conseguir_o_descargar_asignacion_de_leads_de_sup
                    .FromSqlRaw("EXEC dbo.SP_GESTION_CONSEGUIR_O_DESCARGAR_ASIGNACION_DE_LEADS_DE_SUPERVISORES @NombreLista, @Pagina", parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (result == null || result.Count == 0)
                {
                    return (false, "No se encontraron asignaciones detalladas para el supervisor.", new DetallesAsignacionesDescargaSupDTO());
                }
                var detallesAsignaciones = new DetallesAsignacionesDescargaSupDTO(result);
                detallesAsignaciones.dni_supervisor = dni_supervisor;
                detallesAsignaciones.nombres_supervisor = usuario.NombresCompletos ?? "INGRESAR NOMBRE DEL SUPERVISOR";

                return (true, "Asignaciones obtenidas correctamente.", detallesAsignaciones);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, new DetallesAsignacionesDescargaSupDTO());
            }
        }
    }
}