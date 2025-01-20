using ALFINapp.Models;
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
    public DbSet<SubirFeed> SUBIR_FEED { get; set; }
    public DbSet<CargaManualCsv> carga_manual_csv { get; set; }
    public DbSet<AsesoresSecundariosAsignacion> asesores_secundarios_asignacion { get; set; }
    public DbSet<DerivacionesAsesores> derivaciones_asesores { get; set; }
    public DbSet<NumerosEnterosDTO> numeros_enteros_dto { get; set; }
}
