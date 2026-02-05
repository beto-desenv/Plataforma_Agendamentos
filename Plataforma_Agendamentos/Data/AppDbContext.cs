using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Models;

namespace Plataforma_Agendamentos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User configurations
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Slug).IsUnique();
            
            // Configurar campos decimais
            entity.Property(e => e.AvaliacaoMedia).HasColumnType("decimal(3,2)");
        });

        // Service configurations
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
        });

        // Schedule configurations
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // Booking configurations
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // Relationships
        modelBuilder.Entity<User>()
            .HasMany(u => u.Services)
            .WithOne(s => s.Provider)
            .HasForeignKey(s => s.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Schedules)
            .WithOne(s => s.Provider)
            .HasForeignKey(s => s.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Client)
            .WithMany()
            .HasForeignKey(b => b.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Service)
            .WithMany()
            .HasForeignKey(b => b.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}