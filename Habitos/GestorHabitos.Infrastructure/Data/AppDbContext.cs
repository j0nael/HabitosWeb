
using GestorHabitos.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace GestorHabitos.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<UsuarioModel> Usuarios { get; set; }
    public DbSet<CategoriaHabitoModel> CategoriasHabito { get; set; }
    public DbSet<HabitoModel> Habitos { get; set; }
    public DbSet<RegistroModel> Registros { get; set; }
    public DbSet<MetaModel> Metas { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsuarioModel>()
            .HasMany(u => u.Habitos)
            .WithOne(h => h.Usuario)
            .HasForeignKey(h => h.UsuarioId);
        modelBuilder.Entity<UsuarioModel>()
            .HasMany(u => u.Metas)
            .WithOne(m => m.Usuario)
            .HasForeignKey(m => m.UsuarioId);
        modelBuilder.Entity<CategoriaHabitoModel>()
            .HasMany(c => c.Habitos)
            .WithOne(h => h.CategoriaHabito)
            .HasForeignKey(h => h.CategoriaHabitoId);
        modelBuilder.Entity<HabitoModel>()
            .HasMany(h => h.Registros)
            .WithOne(r => r.Habito)
            .HasForeignKey(r => r.HabitoId);
     
    base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MetaModel>()
            .HasOne(m => m.Usuario)
            .WithMany(u => u.Metas)
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MetaModel>()
      .HasOne(m => m.Usuario)
      .WithMany(u => u.Metas)
      .HasForeignKey(m => m.UsuarioId)
      .OnDelete(DeleteBehavior.NoAction);
    }
}

