using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.DTOs.Service;

public class ServiceRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }
}
