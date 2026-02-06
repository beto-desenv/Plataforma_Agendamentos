using Microsoft.Extensions.Caching.Memory;
using Plataforma_Agendamentos.DTOs;
using System.Text.Json;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Service para consulta de CEP via ViaCEP
/// </summary>
public interface ICepService
{
    Task<CepConsultaResponse?> ConsultarCepAsync(string cep);
}

public class CepService : ICepService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CepService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Mapa de UF (sigla) para nome completo do estado
    private static readonly Dictionary<string, string> UfParaEstado = new()
    {
        { "AC", "Acre" },
        { "AL", "Alagoas" },
        { "AP", "Amapá" },
        { "AM", "Amazonas" },
        { "BA", "Bahia" },
        { "CE", "Ceará" },
        { "DF", "Distrito Federal" },
        { "ES", "Espírito Santo" },
        { "GO", "Goiás" },
        { "MA", "Maranhão" },
        { "MT", "Mato Grosso" },
        { "MS", "Mato Grosso do Sul" },
        { "MG", "Minas Gerais" },
        { "PA", "Pará" },
        { "PB", "Paraíba" },
        { "PR", "Paraná" },
        { "PE", "Pernambuco" },
        { "PI", "Piauí" },
        { "RJ", "Rio de Janeiro" },
        { "RN", "Rio Grande do Norte" },
        { "RS", "Rio Grande do Sul" },
        { "RO", "Rondônia" },
        { "RR", "Roraima" },
        { "SC", "Santa Catarina" },
        { "SP", "São Paulo" },
        { "SE", "Sergipe" },
        { "TO", "Tocantins" }
    };

    public CepService(HttpClient httpClient, IMemoryCache memoryCache, ILogger<CepService> logger)
    {
        _httpClient = httpClient;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<CepConsultaResponse?> ConsultarCepAsync(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
        {
            throw new ArgumentException("CEP não pode estar vazio", nameof(cep));
        }

        // Normalizar CEP: remover hífem
        var cepLimpo = cep.Replace("-", "").Trim();

        // Validar formato
        if (!System.Text.RegularExpressions.Regex.IsMatch(cepLimpo, @"^\d{8}$"))
        {
            throw new ArgumentException("CEP deve conter 8 dígitos", nameof(cep));
        }

        // Verificar cache
        var cacheKey = $"cep_{cepLimpo}";
        if (_memoryCache.TryGetValue(cacheKey, out CepConsultaResponse? cachedResult))
        {
            _logger.LogInformation("CEP {Cep} encontrado em cache", cepLimpo);
            return cachedResult;
        }

        try
        {
            _logger.LogInformation("Consultando ViaCEP para CEP {Cep}", cepLimpo);

            // Formatar CEP com hífen para ViaCEP
            var cepFormatado = $"{cepLimpo[..5]}-{cepLimpo[5..]}";
            var url = $"https://viacep.com.br/ws/{cepFormatado}/json/";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var viaCepResponse = JsonSerializer.Deserialize<ViaCepResponse>(content, JsonOptions);

            if (viaCepResponse == null || viaCepResponse.TemErro)
            {
                _logger.LogWarning("CEP {Cep} não encontrado na ViaCEP", cepLimpo);
                return null;
            }

            // Obter nome do estado a partir da sigla UF
            var uf = viaCepResponse.uf?.ToUpper() ?? string.Empty;
            var estadoNome = UfParaEstado.TryGetValue(uf, out var estado) ? estado : uf;

            var result = new CepConsultaResponse(
                cepFormatado,
                viaCepResponse.logradouro ?? string.Empty,
                viaCepResponse.bairro ?? string.Empty,
                viaCepResponse.localidade ?? string.Empty,
                estadoNome, // Estado completo (ex: São Paulo)
                uf // UF (sigla)
            );

            // Armazenar em cache por 24 horas
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(24));

            _memoryCache.Set(cacheKey, result, cacheOptions);
            _logger.LogInformation("CEP {Cep} consultado com sucesso e armazenado em cache", cepLimpo);

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao consultar ViaCEP para CEP {Cep}", cepLimpo);
            throw new InvalidOperationException("Erro ao consultar serviço de CEP", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao desserializar resposta da ViaCEP para CEP {Cep}", cepLimpo);
            throw new InvalidOperationException("Erro ao processar resposta do serviço de CEP", ex);
        }
    }
}
