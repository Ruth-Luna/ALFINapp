using ALFINapp.Domain.Entities;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Application.DTOs
{
    public class DetallesAsignacionesDescargaSupDTO
    {
        public string nombre_lista { get; set; } = string.Empty;
        public string dni_supervisor { get; set; } = string.Empty;
        public string nombres_supervisor { get; set; } = string.Empty;
        public DateTime fecha_creacion_lista { get; set; } = DateTime.MinValue;
        public int total_asignaciones { get; set; } = 0;
        public int total_asignaciones_gestionadas { get; set; } = 0;
        public int total_asignaciones_asignadas_a_asesores { get; set; } = 0;
        public int total_asignaciones_sin_gestionar { get; set; } = 0;
        public int total_asignaciones_sin_asignar { get; set; } = 0;
        public List<Asignacion> asignaciones_detalladas { get; set; } = new List<Asignacion>();
        public DetallesAsignacionesDescargaSupDTO() { }
        public DetallesAsignacionesDescargaSupDTO(List<GestionConseguirODescargarAsignacionDeLeadsDeSup> asignaciones)
        {
            if (asignaciones != null && asignaciones.Count > 0)
            {
                foreach (var asignacion in asignaciones)
                {
                    this.asignaciones_detalladas.Add(new Asignacion
                    {
                        IdAsignacion = asignacion.id_asignacion,
                        FechaAsignacionSup = asignacion.fecha_asignacion_sup ?? DateTime.MinValue,
                        IdUsuarioV = asignacion.id_usuarioV,
                        NombreLista = asignacion.nombre_lista ?? string.Empty,
                        FechaCreacion = asignacion.fecha_creacion ?? DateTime.MinValue,
                        Telefono1 = asignacion.telefono_1 ?? string.Empty,
                        Telefono2 = asignacion.telefono_2 ?? string.Empty,
                        Telefono3 = asignacion.telefono_3 ?? string.Empty,
                        Telefono4 = asignacion.telefono_4 ?? string.Empty,
                        Telefono5 = asignacion.telefono_5 ?? string.Empty,
                        Email1 = asignacion.email_1 ?? string.Empty,
                        Email2 = asignacion.email_2 ?? string.Empty,
                        ClienteNombre = asignacion.nombre_completo ?? string.Empty,
                        OfertaMax = asignacion.oferta_max?.ToString() ?? string.Empty,
                        TipoBase = asignacion.tipo_base ?? string.Empty
                    });
                }
            }
            var nombreLista = asignaciones?.FirstOrDefault()?.nombre_lista ?? string.Empty;
            this.nombre_lista = nombreLista;
            this.dni_supervisor = nombreLista.Split('_')[0];
            this.nombres_supervisor = "INGRESAR NOMBRE DEL SUPERVISOR"; // Placeholder, should be replaced with actual logic to get supervisor name
            this.fecha_creacion_lista = asignaciones?.FirstOrDefault()?.fecha_creacion ?? DateTime.MinValue;
            this.total_asignaciones = asignaciones?.Count ?? 0;
            this.total_asignaciones_gestionadas = asignaciones?.Count(a => a.ultima_tipificacion_general != null) ?? 0;
            this.total_asignaciones_asignadas_a_asesores = asignaciones?.Count(a => a.id_usuarioV.HasValue) ?? 0;
            this.total_asignaciones_sin_gestionar = this.total_asignaciones - this.total_asignaciones_gestionadas;
            this.total_asignaciones_sin_asignar = this.total_asignaciones - this.total_asignaciones_asignadas_a_asesores;
        }
    }
}