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
    public DbSet<AsesoresOcultos> Asesores_Ocultos { get; set; }
    public DbSet<ListasAsignacion> listas_asignacion { get; set; }
    //PROCEDIMIENTOS ALMACENADOS NO BORRAR
    public DbSet<StringDTO> string_dto { get; set; }
    public DbSet<NumerosEnterosDTO> numeros_enteros_dto { get; set; }
    public DbSet<USupervisoresDTO> u_supervisores_dto { get; set; }
    public DbSet<AgenciasDisponiblesDTO> agencias_disponibles_dto { get; set; }
    public DbSet<AsignacionFiltrarBasesDTO> asignacion_filtrar_bases_dto { get; set; }
    public DbSet<VistasPorRolDTO> vistas_por_rol_dto { get; set; }
    public DbSet<ConsultaObtenerCliente> consulta_obtener_cliente { get; set; }
    public DbSet<DetalleClienteA365TipificarDTO> detalle_cliente_a365_tipificar_dto { get; set; }
    public DbSet<InicioDetallesClientesFromAsesor> inicio_detalles_clientes_from_asesor { get; set; }
    public DbSet<SupervisorGetAsignacionLeads> supervisor_get_asignacion_leads { get; set; }
    public DbSet<ReportsGLineasGestionVsDerivacionDiaria> reports_g_lineas_gestion_vs_derivacion_diaria { get; set; }
    public DbSet<ReportsGPiePorcentajeGestionadosSobreAsignados> reports_g_pie_gestion_asignados { get; set; }
    public DbSet<ReportsGPiePorcentajeGestionadoDerivadoDesembolsado> reports_g_pie_derivados_desembolsados { get; set; }
    public DbSet<ResultadoVerificacion> resultado_verificacion { get; set; }
    public DbSet<LeadsGetClientesAsignadosGestionLeads> leads_get_clientes_asignados_gestion_leads { get; set; }
    public DbSet<ReportsBarTop5Derivaciones> reports_bar_top_5_derivaciones { get; set; }
    public DbSet<LeadsGetClientesAsignadosCantidades> leads_get_clientes_asignados_cantidades { get; set; }
    public DbSet<ReportsPieContactabilidadCliente> reports_pie_contactabilidad_cliente { get; set; }
    public DbSet<ReportsTablaGestionadoDerivadoDesembolsadoImporte> reports_tabla_gestionado_derivado_desembolsado_importe { get; set; }
    public DbSet<ReportsDesembolsosNMonto> reports_desembolsos_n_monto { get; set; }
    public DbSet<DerivacionConsultaDerivacionesXAsesorPorDniConReagendacion> derivaciones_asesores_for_view_derivacion { get; set; }
    public DbSet<ReportsTablasMetas> reports_tablas_metas { get; set; }
    public DbSet<ReportsGeneralDatosActualesPorIdUsuarioFecha> reports_general_datos_actuales { get; set; }
    public DbSet<ReportsEtiquetaMetaImporte> reports_etiqueta_meta_importe { get; set; }
    public DbSet<ReportsSupervisorGestionFecha> reports_supervisor_gestion_fecha { get; set; }
    public DbSet<SupervisorGetNumberOfLeads> supervisor_get_number_of_leads { get; set; }
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

        modelBuilder.Entity<ConsultaObtenerCliente>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<DetalleTipificarClienteDTO>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<DetalleClienteA365TipificarDTO>()
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

        modelBuilder.Entity<SupervisorGetAsignacionLeads>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ReportsGLineasGestionVsDerivacionDiaria>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ReportsGPiePorcentajeGestionadosSobreAsignados>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ReportsGPiePorcentajeGestionadoDerivadoDesembolsado>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ResultadoVerificacion>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<LeadsGetClientesAsignadosGestionLeads>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ReportsBarTop5Derivaciones>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<LeadsGetClientesAsignadosCantidades>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ReportsPieContactabilidadCliente>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ReportsTablaGestionadoDerivadoDesembolsadoImporte>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ReportsDesembolsosNMonto>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<DerivacionConsultaDerivacionesXAsesorPorDniConReagendacion>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ReportsTablasMetas>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ReportsGeneralDatosActualesPorIdUsuarioFecha>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ReportsEtiquetaMetaImporte>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<ReportsSupervisorGestionFecha>()
            .HasNoKey()
            .ToView(null);
        modelBuilder.Entity<SupervisorGetNumberOfLeads>()
            .HasNoKey()
            .ToView(null);
    }
}
