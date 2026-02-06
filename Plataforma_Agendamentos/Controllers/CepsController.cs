using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Agendamentos.Controllers.Base;
using Plataforma_Agendamentos.DTOs;
using Plataforma_Agendamentos.Services;

namespace Plataforma_Agendamentos.Controllers;

/// <summary>
/// Controller para consulta de endereços por CEP
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CepsController : BaseApiController
{
    private readonly ICepService _cepService;

    public CepsController(ICepService cepService, ILogger<CepsController> logger)
        : base(logger)
    {
        _cepService = cepService;
    }

    /// <summary>
    /// Consulta endereço por CEP
    /// </summary>
    /// <param name="request">Requisição contendo o CEP a consultar</param>
    /// <returns>Dados do endereço ou erro se não encontrado</returns>
    /// <response code="200">Endereço encontrado com sucesso</response>
    /// <response code="400">CEP inválido</response>
    /// <response code="404">CEP não encontrado</response>
    /// <response code="500">Erro ao consultar serviço de CEP</response>
    [HttpPost("consultar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CepConsultaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CepConsultaResponse>> ConsultarCep([FromBody] CepConsultaRequest request)
    {
        var requestId = GetRequestId();
        
        _logger.LogInformation("Consulta de CEP iniciada - RequestId: {RequestId}, CEP: {Cep}", 
            requestId, MascaraCep(request.Cep));

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Requisição inválida - RequestId: {RequestId}, Erros: {Errors}", 
                requestId, string.Join(", ", ModelState.Values.SelectMany(v => v.Errors)));
            
            return BadRequest(new ProblemDetails
            {
                Title = "Requisição inválida",
                Detail = "O CEP deve estar no formato XXXXX-XXX",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            var resultado = await _cepService.ConsultarCepAsync(request.Cep);

            if (resultado == null)
            {
                _logger.LogWarning("CEP não encontrado - RequestId: {RequestId}, CEP: {Cep}", 
                    requestId, MascaraCep(request.Cep));
                
                return NotFound(new ProblemDetails
                {
                    Title = "CEP não encontrado",
                    Detail = $"O CEP {MascaraCep(request.Cep)} não foi encontrado na base de dados",
                    Status = StatusCodes.Status404NotFound
                });
            }

            _logger.LogInformation("CEP consultado com sucesso - RequestId: {RequestId}, CEP: {Cep}, Cidade: {Cidade}", 
                requestId, MascaraCep(request.Cep), resultado.Cidade);

            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação na consulta de CEP - RequestId: {RequestId}", requestId);
            
            return BadRequest(new ProblemDetails
            {
                Title = "CEP inválido",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Erro ao consultar serviço de CEP - RequestId: {RequestId}", requestId);
            
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Erro ao consultar céP",
                Detail = "Ocorreu um erro ao consultar o serviço de CEP. Tente novamente mais tarde.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado na consulta de CEP - RequestId: {RequestId}", requestId);
            
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Erro interno do servidor",
                Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Mascara o CEP para logging (mostra apenas últimos 3 dígitos)
    /// </summary>
    private string MascaraCep(string cep)
    {
        if (string.IsNullOrEmpty(cep) || cep.Length < 3)
            return "***";

        var limpo = cep.Replace("-", "");
        return limpo.Length >= 3
            ? $"{new string('*', limpo.Length - 3)}{limpo[^3..]}"
            : "***";
    }
}
