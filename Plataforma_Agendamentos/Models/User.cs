using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.Models;

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
    public string UserType { get; set; } = string.Empty; // cliente ou prestador

    // Perfil público
    [MaxLength(100)]
    public string? Slug { get; set; }
    
    [MaxLength(100)]
    public string? DisplayName { get; set; }
    
    public string? LogoUrl { get; set; }
    
    public string? CoverImageUrl { get; set; }
    
    [MaxLength(10)]
    public string? PrimaryColor { get; set; }
    
    public string? Bio { get; set; }

    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}