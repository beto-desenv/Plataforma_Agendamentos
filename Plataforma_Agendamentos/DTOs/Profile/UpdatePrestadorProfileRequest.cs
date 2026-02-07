using System.Text.Json.Serialization;
using Plataforma_Agendamentos.Validators;

namespace Plataforma_Agendamentos.DTOs.Profile;

public class UpdatePrestadorProfileRequest
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Slug { get; set; }
    
    [JsonPropertyName("tituloProfissional")]
    public string? TituloProfissional { get; set; }
    
    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }
    
    public string? Bio { get; set; }
    public string? Document { get; set; }
    
    [JsonPropertyName("documentType")]
    public string? DocumentType { get; set; }
    
    public string? CPF { get; set; }
    public string? CNPJ { get; set; }
    
    [JsonPropertyName("anosExperiencia")]
    public int? AnosExperiencia { get; set; }
    
    [JsonPropertyName("telefone")]
    public string? Telefone { get; set; }
    
    [JsonPropertyName("endereco")]
    public string? Endereco { get; set; }
    
    [JsonPropertyName("cidade")]
    public string? Cidade { get; set; }
    
    [JsonPropertyName("estado")]
    public string? Estado { get; set; }
    
    [JsonPropertyName("cep")]
    public string? CEP { get; set; }
    
    public string? Site { get; set; }
    
    [JsonPropertyName("raioAtendimento")]
    public int? RaioAtendimento { get; set; }
    
    public bool? AceitaAgendamentoImediato { get; set; }
    public int? HorasAntecedenciaMinima { get; set; }
    public string? HorarioInicioSemana { get; set; }
    public string? HorarioFimSemana { get; set; }
    
    [JsonPropertyName("fotoUrl")]
    [ValidBase64Image(500)] // Máximo 500KB
    public string? FotoPerfilUrl { get; set; }
    
    [ValidBase64Image(500)] // Máximo 500KB
    public string? LogoUrl { get; set; }
    
    [ValidBase64Image(1000)] // Cover pode ser um pouco maior: 1MB
    public string? CoverImageUrl { get; set; }
    
    public string? PrimaryColor { get; set; }
    
    [JsonPropertyName("servicos")]
    public List<ServicoRequest>? Servicos { get; set; }
}

public class ServicoRequest
{
    [JsonPropertyName("nome")]
    public string? Nome { get; set; }
    
    [JsonPropertyName("preco")]
    public decimal? Preco { get; set; }
    
    public string? Description { get; set; }
    public int? DurationMinutes { get; set; }
}
