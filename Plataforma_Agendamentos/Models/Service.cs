using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.Models;

public class Service
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProviderId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public int DurationMinutes { get; set; }

    public User Provider { get; set; } = null!;
}