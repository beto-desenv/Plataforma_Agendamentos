using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.Models;

public class Service
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public decimal Preco { get; set; }
    
    [Required]
    public int DurationMinutes { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}