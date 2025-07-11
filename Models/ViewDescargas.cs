using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Models
{
    public class ViewDescargas
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
        public int total_asignaciones_derivadas { get; set; } = 0;
        public List<DetallesDescarga> asignaciones_detalladas { get; set; } = new List<DetallesDescarga>();
        public ViewDescargas() { }
        public ViewDescargas(List<DerivacionConseguirODescargarAsignacionConDerivacionesDeSup> model)
        {
            if (model != null && model.Count > 0)
            {
                foreach (var item in model)
                {
                    this.asignaciones_detalladas.Add(new DetallesDescarga
                    {
                        Dni = item.Dni ?? string.Empty,
                        XAppaterno = item.XAppaterno ?? string.Empty,
                        XApmaterno = item.XApmaterno ?? string.Empty,
                        XNombre = item.XNombre ?? string.Empty,
                        Edad = item.Edad ?? 0,
                        Departamento = item.Departamento ?? string.Empty,
                        Provincia = item.Provincia ?? string.Empty,
                        Distrito = item.Distrito ?? string.Empty,
                        Campaña = item.Campaña ?? string.Empty,
                        OfertaMax = item.OfertaMax ?? 0,
                        TasaMinima = item.TasaMinima ?? 0,
                        SucursalComercial = item.SucursalComercial ?? string.Empty,
                        AgenciaComercial = item.AgenciaComercial ?? string.Empty,
                        Plazo = item.Plazo ?? 0,
                        Cuota = item.Cuota ?? 0,
                        Oferta12m = item.Oferta12m ?? 0,
                        Tasa12m = item.Tasa12m ?? 0,
                        Cuota12m = item.Cuota12m ?? 0,
                        Oferta18m = item.Oferta18m ?? 0,
                        Tasa18m = item.Tasa18m ?? 0,
                        Cuota18m = item.Cuota18m ?? 0,
                        Oferta24m = item.Oferta24m ?? 0,
                        Tasa24m = item.Tasa24m ?? 0,
                        Cuota24m = item.Cuota24m ?? 0,
                        Oferta36m = item.Oferta36m ?? 0,
                        Tasa36m = item.Tasa36m ?? 0,
                        Cuota36m = item.Cuota36m ?? 0,
                        GrupoTasa = item.GrupoTasa ?? string.Empty,
                        GrupoMonto = item.GrupoMonto ?? string.Empty,
                        Propension = item.Propension ?? 0,
                        TipoCliente = item.TipoCliente ?? string.Empty,
                        ClienteNuevo = item.ClienteNuevo ?? string.Empty,
                        Color = item.Color ?? string.Empty,
                        ColorFinal = item.ColorFinal ?? string.Empty,
                        Usuario = item.Usuario ?? string.Empty,
                        UserV3 = item.UserV3 ?? string.Empty,
                        FlagDeudaVOferta = item.FlagDeudaVOferta ?? 0,
                        PerfilRo = item.PerfilRo ?? string.Empty,
                        Prioridad = item.Prioridad ?? string.Empty,
                        Telefono1 = item.Telefono1 ?? string.Empty,
                        Telefono2 = item.Telefono2 ?? string.Empty,
                        Telefono3 = item.Telefono3 ?? string.Empty,
                        Telefono4 = item.Telefono4 ?? string.Empty,
                        Telefono5 = item.Telefono5 ?? string.Empty,
                        Email1 = item.Email1 ?? string.Empty,
                        Email2 = item.Email2 ?? string.Empty,
                        NombreCompleto = $"{item.XAppaterno} {item.XApmaterno} {item.XNombre}".Trim(),
                        TipoBase = item.TipoBase ?? string.Empty,
                        IdAsignacion = item.IdAsignacion,
                        NombreLista = item.NombreLista ?? string.Empty,

                        FechaCreacion = item.FechaCreacion ?? DateTime.MinValue,
                        FechaAsignacionSup = item.FechaAsignacionSup ?? DateTime.MinValue,
                        IdUsuarioV = item.IdUsuarioV,
                        UltimaTipificacionGeneral = item.UltimaTipificacionGeneral ?? string.Empty,
                        ClienteNombreCompleto = $"{item.XAppaterno} {item.XApmaterno} {item.XNombre}".Trim(),

                        // COLUMAS DE DERIVACIONES
                        TelefonoDerivado = item.TelefonoDerivado ?? String.Empty,
                        FechaDerivacion = item.FechaDerivacion ?? DateTime.MinValue,
                        NombreAgenciaDerivada = item.NombreAgenciaDerivada ?? String.Empty,
                        FechaVisita = item.FechaVisita ?? DateTime.MinValue,
                        OfertaMaxDerivada = item.OfertaMaxDerivada ?? 0,
                        DocAsesor = item.DocAsesor ?? string.Empty,
                        NombreAsesor = item.NombreAsesor ?? string.Empty
                    });
                }
                this.nombre_lista = string.Join("\n", model
                    .Select(x => x.NombreLista)
                    .Distinct());
                this.dni_supervisor = string.Join("\n", model
                    .Select(x => x.DocAsesor)
                    .Distinct());
                this.nombres_supervisor = string.Join("\n", model
                    .Select(x => x.NombreAsesor)
                    .Distinct());
                this.fecha_creacion_lista = DateTime.Now; // Asignar la fecha actual como fecha de creación de la lista
                this.total_asignaciones = model.Count;
                this.total_asignaciones_gestionadas = model.Count(x => x.FechaAsignacionSup.HasValue);
                this.total_asignaciones_asignadas_a_asesores = model.Count(x => x.IdUsuarioV.HasValue && x.IdUsuarioV > 0);
                this.total_asignaciones_sin_gestionar = model.Count(x => !x.FechaAsignacionSup.HasValue);
                this.total_asignaciones_sin_asignar = model.Count(x => !x.IdUsuarioV.HasValue || x.IdUsuarioV <= 0);
                this.total_asignaciones_derivadas = model.Count(x => x.FechaAsignacionSup.HasValue && x.IdUsuarioV.HasValue && x.IdUsuarioV > 0);
            }
        }
    }
    public class DetallesDescarga
    {
        public string? Dni { get; set; } = string.Empty;
        public string? XAppaterno { get; set; } = string.Empty;
        public string? XApmaterno { get; set; } = string.Empty;
        public string? XNombre { get; set; } = string.Empty;
        public int? Edad { get; set; } = 0;
        public string? Departamento { get; set; } = string.Empty;
        public string? Provincia { get; set; } = string.Empty;
        public string? Distrito { get; set; } = string.Empty;
        public string? Campaña { get; set; } = string.Empty;
        public decimal? OfertaMax { get; set; } = 0;
        public decimal? TasaMinima { get; set; } = 0;
        public string? SucursalComercial { get; set; } = string.Empty;
        public string? AgenciaComercial { get; set; } = string.Empty;
        public int? Plazo { get; set; } = 0;
        public decimal? Cuota { get; set; } = 0;
        public decimal? Oferta12m { get; set; } = 0;
        public decimal? Tasa12m { get; set; } = 0;
        public decimal? Cuota12m { get; set; } = 0;
        public decimal? Oferta18m { get; set; } = 0;
        public decimal? Tasa18m { get; set; } = 0;
        public decimal? Cuota18m { get; set; } = 0;
        public decimal? Oferta24m { get; set; } = 0;
        public decimal? Tasa24m { get; set; } = 0;
        public decimal? Cuota24m { get; set; } = 0;
        public decimal? Oferta36m { get; set; } = 0;
        public decimal? Tasa36m { get; set; } = 0;
        public decimal? Cuota36m { get; set; } = 0;
        public string? GrupoTasa { get; set; } = string.Empty;
        public string? GrupoMonto { get; set; } = string.Empty;
        public int? Propension { get; set; } = 0;
        public string? TipoCliente { get; set; } = string.Empty;
        public string? ClienteNuevo { get; set; } = string.Empty;
        public string? Color { get; set; } = string.Empty;
        public string? ColorFinal { get; set; } = string.Empty;
        public string? Usuario { get; set; } = string.Empty;
        public string? UserV3 { get; set; } = string.Empty;
        public int? FlagDeudaVOferta { get; set; } = 0;
        public string? PerfilRo { get; set; } = string.Empty;
        public string? Prioridad { get; set; } = string.Empty;
        public string? Telefono1 { get; set; } = string.Empty;
        public string? Telefono2 { get; set; } = string.Empty;
        public string? Telefono3 { get; set; } = string.Empty;
        public string? Telefono4 { get; set; } = string.Empty;
        public string? Telefono5 { get; set; } = string.Empty;
        public string? Email1 { get; set; } = string.Empty;
        public string? Email2 { get; set; } = string.Empty;
        public string? NombreCompleto { get; set; } = string.Empty;
        public string? TipoBase { get; set; } = string.Empty;
        public int IdAsignacion { get; set; } = 0;
        public string? NombreLista { get; set; } = string.Empty;
        public DateTime? FechaCreacion { get; set; } = DateTime.MinValue;
        public DateTime? FechaAsignacionSup { get; set; } = DateTime.MinValue;
        public int? IdUsuarioV { get; set; } = 0;
        public string? UltimaTipificacionGeneral { get; set; } = string.Empty;
        public string? ClienteNombreCompleto { get; set; } = string.Empty;

        //Tabla de Derivaciones
        public string? TelefonoDerivado { get; set; } = string.Empty;
        public DateTime? FechaDerivacion { get; set; } = DateTime.MinValue;
        public string? NombreAgenciaDerivada { get; set; } = string.Empty;
        public DateTime? FechaVisita { get; set; } = DateTime.MinValue;
        public decimal? OfertaMaxDerivada { get; set; } = 0;
        public string? DocAsesor { get; set; } = string.Empty; // Nuevo campo para el documento del asesor
        public string? NombreAsesor { get; set; } = string.Empty; // Nuevo campo para el nombre del asesor
    }
}