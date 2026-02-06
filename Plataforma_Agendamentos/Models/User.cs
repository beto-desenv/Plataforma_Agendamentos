using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.Models;

/// <summary>
/// Entidade base para identidade e autenticacao de usuarios
/// </summary>
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string UserType { get; set; } = string.Empty; // "cliente" | "prestador"
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public bool PerfilAtivo { get; set; } = true;
    
    public string? FotoPerfilUrl { get; set; }

    // Navigation properties
    public ClientePerfil? ClientePerfil { get; set; }
    public PrestadorPerfil? PrestadorPerfil { get; set; }
    
    // Para compatibilidade com codigo existente (Services e Schedules mantidos)
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    // Helper methods
    public bool IsCliente() => UserType == Constants.UserTypes.Cliente;
    public bool IsPrestador() => UserType == Constants.UserTypes.Prestador;
}