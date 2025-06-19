namespace ALFINapp.Models
{
    public class ViewVerAsignacionesDelSupervisor
    {
        public List<ViewListasAsignacionesPorSupervisor> asignaciones { get; set; } = new List<ViewListasAsignacionesPorSupervisor>();
    }

    public class ViewListasAsignacionesPorSupervisor
    {
        public string? dni { get; set; }
        public string? nombres_supervisor { get; set; }
        public string? nombre_lista { get; set; }
        public DateTime? fecha_creacion_lista { get; set; }
        public int total_asignaciones { get; set; } = 0;
        public int total_asignaciones_gestionadas { get; set; } = 0;
        public int total_asignaciones_asignadas_a_asesores { get; set; } = 0;
    }
}