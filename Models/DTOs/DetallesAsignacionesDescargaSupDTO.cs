using ALFINapp.Domain.Entities;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.API.DTOs
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
                        Dni = asignacion.Dni ?? string.Empty,
                        XAppaterno = asignacion.XAppaterno ?? string.Empty,
                        XApmaterno = asignacion.XApmaterno ?? string.Empty,
                        XNombre = asignacion.XNombre ?? string.Empty,
                        Edad = asignacion.Edad ?? 0,
                        Departamento = asignacion.Departamento ?? string.Empty,
                        Provincia = asignacion.Provincia ?? string.Empty,
                        Distrito = asignacion.Distrito ?? string.Empty,
                        Campaña = asignacion.Campaña ?? string.Empty,
                        OfertaMax = asignacion.OfertaMax ?? 0,
                        TasaMinima = asignacion.TasaMinima ?? 0,
                        SucursalComercial = asignacion.SucursalComercial ?? string.Empty,
                        AgenciaComercial = asignacion.AgenciaComercial ?? string.Empty,
                        Plazo = asignacion.Plazo ?? 0,
                        Cuota = asignacion.Cuota ?? 0,
                        Oferta12m = asignacion.Oferta12m ?? 0,
                        Tasa12m = asignacion.Tasa12m ?? 0,
                        Cuota12m = asignacion.Cuota12m ?? 0,
                        Oferta18m = asignacion.Oferta18m ?? 0,
                        Tasa18m = asignacion.Tasa18m ?? 0,
                        Cuota18m = asignacion.Cuota18m ?? 0,
                        Oferta24m = asignacion.Oferta24m ?? 0,
                        Tasa24m = asignacion.Tasa24m ?? 0,
                        Cuota24m = asignacion.Cuota24m ?? 0,
                        Oferta36m = asignacion.Oferta36m ?? 0,
                        Tasa36m = asignacion.Tasa36m ?? 0,
                        Cuota36m = asignacion.Cuota36m ?? 0,
                        GrupoTasa = asignacion.GrupoTasa ?? string.Empty,
                        GrupoMonto = asignacion.GrupoMonto ?? string.Empty,
                        Propension = asignacion.Propension ?? 0,
                        TipoCliente = asignacion.TipoCliente ?? string.Empty,
                        ClienteNuevo = asignacion.ClienteNuevo ?? string.Empty,
                        Color = asignacion.Color ?? string.Empty,
                        ColorFinal = asignacion.ColorFinal ?? string.Empty,
                        Usuario = asignacion.Usuario ?? string.Empty,
                        UserV3 = asignacion.UserV3 ?? string.Empty,
                        FlagDeudaVOferta = asignacion.FlagDeudaVOferta ?? 0,
                        PerfilRo = asignacion.PerfilRo ?? string.Empty,
                        Prioridad = asignacion.Prioridad ?? string.Empty,


                        IdAsignacion = asignacion.IdAsignacion,
                        FechaAsignacionSup = asignacion.FechaAsignacionSup ?? DateTime.MinValue,
                        IdUsuarioV = asignacion.IdUsuarioV,
                        NombreLista = asignacion.NombreLista ?? string.Empty,
                        FechaCreacion = asignacion.FechaCreacion ?? DateTime.MinValue,
                        Telefono1 = asignacion.Telefono1 ?? string.Empty,
                        Telefono2 = asignacion.Telefono2 ?? string.Empty,
                        Telefono3 = asignacion.Telefono3 ?? string.Empty,
                        Telefono4 = asignacion.Telefono4 ?? string.Empty,
                        Telefono5 = asignacion.Telefono5 ?? string.Empty,
                        Email1 = asignacion.Email1 ?? string.Empty,
                        Email2 = asignacion.Email2 ?? string.Empty,
                        ClienteNombreCompleto = asignacion.NombreCompleto ?? string.Empty,
                        TipoBase = asignacion.TipoBase ?? string.Empty
                    });
                }
            }
            var nombreLista = asignaciones?.FirstOrDefault()?.NombreLista ?? string.Empty;
            this.nombre_lista = nombreLista;
            this.dni_supervisor = nombreLista.Split('_')[0];
            this.nombres_supervisor = "INGRESAR NOMBRE DEL SUPERVISOR"; // Placeholder, should be replaced with actual logic to get supervisor name
            this.fecha_creacion_lista = asignaciones?.FirstOrDefault()?.FechaCreacion ?? DateTime.MinValue;
            this.total_asignaciones = asignaciones?.Count ?? 0;
            this.total_asignaciones_gestionadas = asignaciones?.Count(a => a.UltimaTipificacionGeneral != null) ?? 0;
            this.total_asignaciones_asignadas_a_asesores = asignaciones?.Count(a => a.IdUsuarioV.HasValue) ?? 0;
            this.total_asignaciones_sin_gestionar = this.total_asignaciones - this.total_asignaciones_gestionadas;
            this.total_asignaciones_sin_asignar = this.total_asignaciones - this.total_asignaciones_asignadas_a_asesores;
        }
    }
}