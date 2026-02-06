using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.DTOs;

/// <summary>
/// DTO para atualizar perfil de Prestador
/// </summary>
public class PrestadorProfileUpdateRequest
{
    [Required]
    [RegularExpression(@"^(cpf|cnpj)$")]
    public string DocumentType { get; set; } = "cpf";

    [Required]
    [RegularExpression(@"^[\d.\/-]+$")]
    public string Document { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string TituloProfissional { get; set; } = string.Empty;

    [Required]
    public int AnosExperiencia { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Descricao { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Telefone { get; set; }

    [Required]
    [MaxLength(50)]
    public string Estado { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Cidade { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Endereco { get; set; } = string.Empty;

    [Required]
    public int RaioAtendimento { get; set; }

    public string? FotoUrl { get; set; }

    // Serviços (relacionado com tabela Service)
    public List<PrestadorServicoRequest> Servicos { get; set; } = new();
}

public class PrestadorServicoRequest
{
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public decimal Preco { get; set; }
}

/// <summary>
/// DTO para atualizar perfil de Cliente
/// </summary>
public class ClienteProfileUpdateRequest
{
    [MaxLength(20)]
    public string? Telefone { get; set; }

    public DateTime? DataNascimento { get; set; }

    [MaxLength(50)]
    public string? CPF { get; set; }

    [Required]
    [MaxLength(50)]
    public string Estado { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Cidade { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Endereco { get; set; } = string.Empty;

    public List<string> InteressesServicos { get; set; } = new();

    [Required]
    [RegularExpression(@"^(whatsapp|telefone|email)$")]
    public string ContatoPreferido { get; set; } = "email";

    [MaxLength(500)]
    public string? Bio { get; set; }

    public string? FotoUrl { get; set; }
}
