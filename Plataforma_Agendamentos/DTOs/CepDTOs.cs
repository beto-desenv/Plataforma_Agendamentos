using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.DTOs;

/// <summary>
/// Requisição para consultar endereço por CEP
/// </summary>
public class CepConsultaRequest
{
    [Required]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve estar no formato XXXXX-XXX ou XXXXXXXX")]
    public string Cep { get; set; } = string.Empty;
}

/// <summary>
/// Resposta com dados do endereço
/// </summary>
public class CepConsultaResponse
{
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;

    public CepConsultaResponse() { }

    public CepConsultaResponse(string cep, string logradouro, string bairro, string cidade, string estado, string uf)
    {
        Cep = cep;
        Logradouro = logradouro;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        UF = uf;
    }
}

/// <summary>
/// Resposta da ViaCEP (formato externo)
/// </summary>
public class ViaCepResponse
{
    public string? cep { get; set; }
    public string? logradouro { get; set; }
    public string? bairro { get; set; }
    public string? localidade { get; set; }
    public string? uf { get; set; }
    
    // ViaCEP retorna "erro": "true" como string, não como booleano
    public string? erro { get; set; }
    
    // Helper para verificar se há erro
    public bool TemErro => erro?.ToLower() == "true";
}
