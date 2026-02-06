using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Models;

namespace Plataforma_Agendamentos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    // Entidades principais
    public DbSet<User> Users { get; set; }
    public DbSet<ClientePerfil> ClientePerfis { get; set; }
    public DbSet<PrestadorPerfil> PrestadorPerfis { get; set; }
    public DbSet<PrestadorBranding> PrestadorBrandings { get; set; }
    public DbSet<PrestadorMetricas> PrestadorMetricas { get; set; }
    
    // Entidades de negocio
    public DbSet<Service> Services { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);
        ConfigureClientePerfil(modelBuilder);
        ConfigurePrestadorPerfil(modelBuilder);
        ConfigurePrestadorBranding(modelBuilder);
        ConfigurePrestadorMetricas(modelBuilder);
        ConfigureService(modelBuilder);
        ConfigureSchedule(modelBuilder);
        ConfigureBooking(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            
            // Relacionamentos 1:1 com perfis
            entity.HasOne(e => e.ClientePerfil)
                .WithOne(c => c.User)
                .HasForeignKey<ClientePerfil>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.PrestadorPerfil)
                .WithOne(p => p.User)
                .HasForeignKey<PrestadorPerfil>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Relacionamentos com servicos e agendas (mantidos)
            entity.HasMany(u => u.Services)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Schedules)
                .WithOne(s => s.Provider)
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureClientePerfil(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClientePerfil>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.CPF);
        });
    }

    private void ConfigurePrestadorPerfil(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrestadorPerfil>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.CNPJ);
            
            // Relacionamentos 1:1
            entity.HasOne(e => e.Branding)
                .WithOne(b => b.PrestadorPerfil)
                .HasForeignKey<PrestadorBranding>(b => b.PrestadorPerfilId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Metricas)
                .WithOne(m => m.PrestadorPerfil)
                .HasForeignKey<PrestadorMetricas>(m => m.PrestadorPerfilId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigurePrestadorBranding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrestadorBranding>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PrestadorPerfilId).IsUnique();
        });
    }

    private void ConfigurePrestadorMetricas(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrestadorMetricas>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PrestadorPerfilId).IsUnique();
            
            entity.Property(e => e.AvaliacaoMedia).HasColumnType("decimal(3,2)");
        });
    }

    private void ConfigureService(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Preco).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CriadoEm).HasDefaultValueSql("now() at time zone 'UTC'");
            entity.Property(e => e.AtualizadoEm).HasDefaultValueSql("now() at time zone 'UTC'");
        });
    }

    private void ConfigureSchedule(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }

    private void ConfigureBooking(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(b => b.Client)
                .WithMany()
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.Service)
                .WithMany()
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}