using Microsoft.EntityFrameworkCore;
using Mottu.Api.Domain;

namespace Mottu.Api.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) {}

  public DbSet<Usuario> Usuarios => Set<Usuario>();
  public DbSet<Moto> Motos => Set<Moto>();
  public DbSet<Manutencao> Manutencoes => Set<Manutencao>();

  protected override void OnModelCreating(ModelBuilder b)
  {
    b.Entity<Usuario>()
      .HasIndex(x => x.Email)
      .IsUnique();

    b.Entity<Moto>()
      .HasIndex(x => x.Placa)
      .IsUnique();

    b.Entity<Moto>()
      .Property(x => x.Modelo)
      .HasConversion<string>(); 

    b.Entity<Manutencao>()
      .HasKey(x => x.Id);

    b.Entity<Manutencao>()
      .Property(x => x.Status)
      .HasConversion<string>();

    base.OnModelCreating(b);
  }
}
