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
}
