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
    public string UserType { get; set; } = string.Empty;

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Campos especificos de CLIENTE
    [MaxLength(20)]
    public string? TelefoneCliente { get; set; }
    
    public DateTime? DataNascimento { get; set; }
    
    [MaxLength(50)]
    public string? EstadoCliente { get; set; }
    
    [MaxLength(100)]
    public string? CidadeCliente { get; set; }
    
    [MaxLength(200)]
    public string? EnderecoCliente { get; set; }
    
    [MaxLength(100)]
    public string? PreferenciasNotificacao { get; set; }
    
    [MaxLength(50)]
    public string? CPF { get; set; }
    
    [MaxLength(100)]
    public string? InteressesServicos { get; set; } // JSON array: ["Limpeza", "Reparos", ...]
    
    [MaxLength(50)]
    public string? ContatoPreferido { get; set; } // "whatsapp" | "telefone" | "email"
    
    public string? FotoPerfilUrl { get; set; }
    
    public int TotalAgendamentosCliente { get; set; } = 0;
    public DateTime? UltimoAgendamento { get; set; }

    // Campos especificos de PRESTADOR
    [MaxLength(100)]
    public string? Slug { get; set; }
    
    [MaxLength(100)]
    public string? DisplayName { get; set; }
    
    [MaxLength(100)]
    public string? TituloProfissional { get; set; } // Ex: Encanador, Eletricista
    
    [MaxLength(50)]
    public string? CPFPrestador { get; set; }
    
    public int? AnosExperiencia { get; set; } // 0, 1, 3, 6, 11, etc
    
    public string? LogoUrl { get; set; }
    
    public string? CoverImageUrl { get; set; }
    
    [MaxLength(7)]
    public string? PrimaryColor { get; set; }
    
    [MaxLength(1000)]
    public string? Bio { get; set; }
    
    [MaxLength(20)]
    public string? CNPJ { get; set; }
    
    [MaxLength(50)]
    public string? EstadoPrestador { get; set; }
    
    [MaxLength(100)]
    public string? CidadePrestador { get; set; }
    
    [MaxLength(200)]
    public string? EnderecoPrestador { get; set; }
    
    [MaxLength(20)]
    public string? TelefonePrestador { get; set; }
    
    [MaxLength(100)]
    public string? Site { get; set; }
    
    public int? RaioAtendimento { get; set; } // em km
    
    public decimal AvaliacaoMedia { get; set; } = 0;
    public int TotalAvaliacoes { get; set; } = 0;
    public int TotalServicos { get; set; } = 0;
    public int TotalAgendamentosPrestador { get; set; } = 0;
    
    public bool AceitaAgendamentoImediato { get; set; } = true;
    public int HorasAntecedenciaMinima { get; set; } = 2;
    public bool PerfilAtivo { get; set; } = true;
    
    [MaxLength(10)]
    public string? HorarioInicioSemana { get; set; }
    
    [MaxLength(10)]
    public string? HorarioFimSemana { get; set; }

    // Navigation properties
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    // Helper methods
    public bool IsCliente() => UserType == "cliente";
    public bool IsPrestador() => UserType == "prestador";
    
    public string? GetTelefone() => IsCliente() ? TelefoneCliente : TelefonePrestador;
    public string? GetEndereco() => IsCliente() ? EnderecoCliente : EnderecoPrestador;
    
    public string GetPublicUrl() => !string.IsNullOrEmpty(Slug) ? $"/prestador/{Slug}" : $"/prestador/{Id}";
    public bool TemPerfilCompleto() => !string.IsNullOrEmpty(DisplayName) && !string.IsNullOrEmpty(Bio);
    
    public void GerarSlug()
    {
        if (IsPrestador() && string.IsNullOrEmpty(Slug) && !string.IsNullOrEmpty(DisplayName ?? Name))
        {
            var nome = (DisplayName ?? Name).ToLower();
            Slug = System.Text.RegularExpressions.Regex.Replace(nome, @"[^a-z0-9\s-]", "")
                   .Replace(" ", "-")
                   .Replace("--", "-")
                   .Trim('-');
        }
    }
}