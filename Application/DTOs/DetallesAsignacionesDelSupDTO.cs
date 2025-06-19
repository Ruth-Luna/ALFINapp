using ALFINapp.Infrastructure.Persistence.Procedures;
using ALFINapp.Models;

namespace ALFINapp.Application.DTOs
{
    public class DetallesAsignacionesDelSupDTO
    {
        public string? Dni { get; set; }
        public string? XAppaterno { get; set; }
        public string? XApmaterno { get; set; }
        public string? XNombre { get; set; }
        public DetallesListaDTO? DetallesLista { get; set; }
        public DetallesAsignacionesDelSupDTO()
        {
            DetallesLista = new DetallesListaDTO();
        }
        public DetallesAsignacionesDelSupDTO(GestionConseguirTodasLasAsignacionesPorListas asignaciones)
        {
            Dni = asignaciones.dni_supervisor?.ToString();
            XNombre = asignaciones.nombre_supervisor;
            DetallesLista = new DetallesListaDTO
            {
                NombreLista = asignaciones.nombre_lista,
                FechaCreacionLista = asignaciones.fecha_creacion_lista,
                TotalAsignaciones = asignaciones.total_asignaciones,
                TotalAsignacionesGestionadas = asignaciones.asignaciones_gestionadas,
                TotalAsignacionesAsignadasAAsesores = asignaciones.asignadas_a_asesores
            };
        }
        public ViewListasAsignacionesPorSupervisor toViewListas()
        {
            return new ViewListasAsignacionesPorSupervisor
            {
                dni = Dni,
                nombres_supervisor = XNombre,
                nombre_lista = DetallesLista?.NombreLista,
                fecha_creacion_lista = DetallesLista?.FechaCreacionLista,
                total_asignaciones = DetallesLista?.TotalAsignaciones ?? 0,
                total_asignaciones_gestionadas = DetallesLista?.TotalAsignacionesGestionadas ?? 0,
                total_asignaciones_asignadas_a_asesores = DetallesLista?.TotalAsignacionesAsignadasAAsesores ?? 0
            };
        }
    }

    public class DetallesListaDTO
    {
        public string? NombreLista { get; set; }
        public DateTime? FechaCreacionLista { get; set; }
        public int TotalAsignaciones { get; set; } = 0;
        public int TotalAsignacionesGestionadas { get; set; } = 0;
        public int TotalAsignacionesAsignadasAAsesores { get; set; } = 0;
    }
}