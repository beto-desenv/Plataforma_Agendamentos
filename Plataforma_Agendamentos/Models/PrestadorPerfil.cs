using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.Models;

/// <summary>
/// Perfil especifico de prestador de servicos
/// </summary>
public class PrestadorPerfil
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    // Identificacao
    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Slug { get; set; }
    
    [MaxLength(100)]
    public string? TituloProfissional { get; set; }
    
    [MaxLength(1000)]
    public string? Bio { get; set; }
    
    // Documentos
    [MaxLength(14)]
    public string? CPF { get; set; }
    
    [MaxLength(18)]
    public string? CNPJ { get; set; }
    
    public int? AnosExperiencia { get; set; }
    
    // Contato
    [MaxLength(20)]
    public string? Telefone { get; set; }
    
    [MaxLength(200)]
    public string? Site { get; set; }
    
    // Endereco
    [MaxLength(200)]
    public string? Endereco { get; set; }
    
    [MaxLength(100)]
    public string? Cidade { get; set; }
    
    [MaxLength(50)]
    public string? Estado { get; set; }
    
    [MaxLength(9)]
    public string? CEP { get; set; }
    
    // Atendimento
    public int? RaioAtendimento { get; set; } // em km
    
    public bool AceitaAgendamentoImediato { get; set; } = true;
    
    public int HorasAntecedenciaMinima { get; set; } = 2;
    
    [MaxLength(5)]
    public string? HorarioInicioSemana { get; set; } // "08:00"
    
    [MaxLength(5)]
    public string? HorarioFimSemana { get; set; } // "18:00"
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public User User { get; set; } = null!;
    public PrestadorBranding? Branding { get; set; }
    public PrestadorMetricas? Metricas { get; set; }
    
    // Helper methods
    public string GetPublicUrl() => !string.IsNullOrEmpty(Slug) ? $"/prestador/{Slug}" : $"/prestador/{UserId}";
    
    public bool TemPerfilCompleto() => !string.IsNullOrEmpty(Bio) && !string.IsNullOrEmpty(Slug);
    
    public void GerarSlug()
    {
        if (string.IsNullOrEmpty(Slug) && !string.IsNullOrEmpty(DisplayName))
        {
            var nome = DisplayName.ToLower();
            Slug = System.Text.RegularExpressions.Regex.Replace(nome, @"[^a-z0-9\s-]", "")
                   .Replace(" ", "-")
                   .Replace("--", "-")
                   .Trim('-');
        }
    }
}
