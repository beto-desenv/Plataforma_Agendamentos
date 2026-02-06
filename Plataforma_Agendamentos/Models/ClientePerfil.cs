using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.Models;

/// <summary>
/// Perfil especifico de cliente
/// </summary>
public class ClientePerfil
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    // Contato
    [MaxLength(20)]
    public string? Telefone { get; set; }
    
    [MaxLength(50)]
    public string? ContatoPreferido { get; set; } // "whatsapp" | "telefone" | "email"
    
    // Documentos
    [MaxLength(14)]
    public string? CPF { get; set; }
    
    public DateTime? DataNascimento { get; set; }
    
    // Endereco
    [MaxLength(200)]
    public string? Endereco { get; set; }
    
    [MaxLength(100)]
    public string? Cidade { get; set; }
    
    [MaxLength(50)]
    public string? Estado { get; set; }
    
    [MaxLength(9)]
    public string? CEP { get; set; }
    
    // Preferencias
    [MaxLength(200)]
    public string? PreferenciasNotificacao { get; set; }
    
    [MaxLength(500)]
    public string? InteressesServicos { get; set; } // JSON array
    
    // Metricas
    public int TotalAgendamentos { get; set; } = 0;
    public DateTime? UltimoAgendamento { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public User User { get; set; } = null!;
}
