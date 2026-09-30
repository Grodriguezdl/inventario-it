using InventarioIT.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioIT.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Asignacion> Asignaciones => Set<Asignacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.Property(u => u.Nombre).HasMaxLength(100);
            e.Property(u => u.Email).HasMaxLength(150);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Rol).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Empleado>(e =>
        {
            e.Property(x => x.Nombre).HasMaxLength(100);
            e.Property(x => x.Apellido).HasMaxLength(100);
            e.Property(x => x.Email).HasMaxLength(150);
            e.Property(x => x.Departamento).HasMaxLength(100);
            e.Property(x => x.Puesto).HasMaxLength(100);
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Categoria>(e =>
        {
            e.Property(c => c.Nombre).HasMaxLength(80);
            e.HasIndex(c => c.Nombre).IsUnique();
            e.HasData(
                new Categoria { Id = 1, Nombre = "Laptop" },
                new Categoria { Id = 2, Nombre = "Desktop" },
                new Categoria { Id = 3, Nombre = "Monitor" },
                new Categoria { Id = 4, Nombre = "Impresora" },
                new Categoria { Id = 5, Nombre = "Periférico" },
                new Categoria { Id = 6, Nombre = "Equipo de red" });
        });

        modelBuilder.Entity<Equipo>(e =>
        {
            e.Property(x => x.CodigoInventario).HasMaxLength(30);
            e.HasIndex(x => x.CodigoInventario).IsUnique();
            e.Property(x => x.NumeroSerie).HasMaxLength(100);
            e.Property(x => x.Marca).HasMaxLength(80);
            e.Property(x => x.Modelo).HasMaxLength(100);
            e.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            e.HasOne(x => x.Categoria)
             .WithMany(c => c.Equipos)
             .HasForeignKey(x => x.CategoriaId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Asignacion>(e =>
        {
            e.HasOne(a => a.Equipo)
             .WithMany(x => x.Asignaciones)
             .HasForeignKey(a => a.EquipoId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Empleado)
             .WithMany(x => x.Asignaciones)
             .HasForeignKey(a => a.EmpleadoId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.AsignadoPor)
             .WithMany()
             .HasForeignKey(a => a.AsignadoPorId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}