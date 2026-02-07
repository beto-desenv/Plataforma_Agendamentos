using System.Text.Json.Serialization;
using Plataforma_Agendamentos.Validators;

namespace Plataforma_Agendamentos.DTOs.Profile;

public class UpdateClienteProfileRequest
{
    public string? Name { get; set; }
    public string? Telefone { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    
    [JsonPropertyName("cep")]
    public string? CEP { get; set; }
    
    [JsonPropertyName("cpf")]
    public string? CPF { get; set; }
    
    [JsonPropertyName("contatoPreferido")]
    public string? ContatoPreferido { get; set; }
    
    [JsonPropertyName("interessesServicos")]
    public string? InteressesServicos { get; set; }
    
    public string? PreferenciasNotificacao { get; set; }
    public string? Bio { get; set; }
    
    [ValidBase64Image(500)]
    [JsonPropertyName("fotoPerfilUrl")] // Aceitar camelCase do frontend
    public string? FotoPerfilUrl { get; set; }
}
