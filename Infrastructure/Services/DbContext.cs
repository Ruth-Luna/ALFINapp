using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;
using Microsoft.EntityFrameworkCore;

public class MDbContext : DbContext
{
    public MDbContext(DbContextOptions<MDbContext> options)
        : base(options)
    {
    }
    public DbSet<Usuario> usuarios { get; set; }
    public DbSet<ClientesAsignado> clientes_asignados { get; set; }
    public DbSet<BaseCliente> base_clientes { get; set; }
    public DbSet<ClientesEnriquecido> clientes_enriquecidos { get; set; }
    public DbSet<DetalleBase> detalle_base { get; set; }
    public DbSet<ClientesTipificado> clientes_tipificados { get; set; }
    public DbSet<Tipificaciones> tipificaciones { get; set; }
    public DbSet<TelefonosAgregados> telefonos_agregados { get; set; }
    public DbSet<DerivacionesAsesores> derivaciones_asesores { get; set; }
    public DbSet<BaseClientesBanco> base_clientes_banco { get; set; }
    public DbSet<CampanaGrupo> base_clientes_banco_campana_grupo { get; set; }
    public DbSet<Color> base_clientes_banco_color { get; set; }
    public DbSet<Plazo> base_clientes_banco_plazo { get; set; }
    public DbSet<RangoDeuda> base_clientes_banco_rango_deuda { get; set; }
    public DbSet<UsuarioBanco> base_clientes_banco_usuario { get; set; }
    public DbSet<ClientesReferidos> clientes_referidos { get; set; }
    public DbSet<VistaRutas> Vista_Rutas { get; set; }
    public DbSet<Roles> roles { get; set; }
    public DbSet<BancoUserV3> base_clientes_banco_user_v3 { get; set; }
    public DbSet<GESTIONDETALLE> GESTION_DETALLE { get; set; }
    public DbSet<Desembolsos> desembolsos { get; set; }
    public DbSet<PermisosRolesVistas> Permisos_Roles_Vistas { get; set; }
    public DbSet<Retiros> retiros { get; set; }
    public DbSet<Correos> correos { get; set; }
    public DbSet<DirectorioComercial> directorio_comercial { get; set; }
    public DbSet<AsesoresOcultos> Asesores_Ocultos { get; set; }
    //PROCEDIMIENTOS ALMACENADOS NO BORRAR
    public DbSet<StringDTO> string_dto { get; set; }
    public DbSet<NumerosEnterosDTO> numeros_enteros_dto { get; set; }
    public DbSet<USupervisoresDTO> u_supervisores_dto { get; set; }
    public DbSet<AgenciasDisponiblesDTO> agencias_disponibles_dto { get; set; }
    public DbSet<AsignacionFiltrarBasesDTO> asignacion_filtrar_bases_dto { get; set; }
    public DbSet<VistasPorRolDTO> vistas_por_rol_dto { get; set; }
    public DbSet<DetallesClienteDTO> detalles_clientes_dto { get; set; }
    public DbSet<DetalleClienteA365TipificarDTO> detalle_cliente_a365_tipificar_dto { get; set; }
    public DbSet<InicioDetallesClientesFromAsesor> inicio_detalles_clientes_from_asesor { get; set; }
    public DbSet<SupervisorGetInicioData> supervisor_get_inicio_data { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AsignacionFiltrarBasesDTO>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<AgenciasDisponiblesDTO>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<VistasPorRolDTO>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<FeedGReportes>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<DerivacionesBSDIALDTO>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<DetallesClienteDTO>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<BoolDTO>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<DetalleTipificarClienteDTO>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<DetalleClienteA365TipificarDTO>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<DerivacionesAsesoresDTO>()
            .HasNoKey()
            .ToView(null);
        
        modelBuilder.Entity<InicioDetallesClientesFromAsesor>()
            .HasNoKey()
            .ToView(null);
        
        modelBuilder.Entity<ReportsSupervisorDerivacionFecha>()
            .HasNoKey()
            .ToView(null);
        
        modelBuilder.Entity<ReportsSupervisorGestion>()
            .HasNoKey()
            .ToView(null);
        
        modelBuilder.Entity<SupervisorGetInicioData>()
            .HasNoKey()
            .ToView(null);
    }
}
