using ALFINapp.Models;
using Microsoft.EntityFrameworkCore;

public class MDbContext : DbContext
{
    public MDbContext(DbContextOptions<MDbContext> options)
        : base(options)
    {
    }

    public DbSet<DNIUserModel> usuarios { get; set; }
    public DbSet<ClienteAsignado> clientes_asignados { get; set; }
    public DbSet<BaseCliente> base_clientes { get; set; }
}
