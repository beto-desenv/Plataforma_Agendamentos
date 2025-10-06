using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Plataforma_Agendamentos.Controllers.Base;

/// <summary>
/// Controller base com funcionalidades comuns para todos os controllers
/// </summary>
public abstract class BaseApiController : ControllerBase
{
    protected readonly ILogger _logger;

    protected BaseApiController(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Obtém o ID do usuário atual a partir do token JWT
    /// </summary>
    /// <returns>ID do usuário ou null se não autenticado</returns>
    protected Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("UserId claim não encontrada no token JWT para usuário {UserName}", User.Identity?.Name);
            return null;
        }

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogDebug("UserId extraído do token: {UserId}", userId);
            return userId;
        }

        _logger.LogWarning("UserId claim não é um GUID válido: {UserIdClaim}", userIdClaim);
        return null;
    }

    /// <summary>
    /// Obtém o email do usuário atual a partir do token JWT
    /// </summary>
    /// <returns>Email do usuário ou null se não encontrado</returns>
    protected string? GetCurrentUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
    }

    /// <summary>
    /// Obtém o tipo do usuário atual a partir do token JWT
    /// </summary>
    /// <returns>Tipo do usuário ou null se não encontrado</returns>
    protected string? GetCurrentUserType()
    {
        return User.FindFirst("UserType")?.Value;
    }

    /// <summary>
    /// Obtém o RequestId da requisição atual para rastreamento
    /// </summary>
    /// <returns>RequestId da requisição</returns>
    protected string GetRequestId()
    {
        return HttpContext.Items["RequestId"]?.ToString() ?? Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Cria uma resposta de erro padronizada
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="details">Detalhes adicionais do erro</param>
    /// <returns>Objeto de resposta de erro</returns>
    protected object CreateErrorResponse(string message, string? details = null)
    {
        var requestId = GetRequestId();
        _logger.LogWarning("Erro na requisição {RequestId}: {Message} - {Details}", requestId, message, details);
        
        return new
        {
            Error = true,
            Message = message,
            Details = details,
            RequestId = requestId,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Cria uma resposta de sucesso padronizada
    /// </summary>
    /// <param name="data">Dados a serem retornados</param>
    /// <param name="message">Mensagem de sucesso opcional</param>
    /// <returns>Objeto de resposta de sucesso</returns>
    protected object CreateSuccessResponse(object data, string? message = null)
    {
        var requestId = GetRequestId();
        
        return new
        {
            Success = true,
            Message = message,
            Data = data,
            RequestId = requestId,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Log estruturado para operações de negócio
    /// </summary>
    /// <param name="operation">Nome da operação</param>
    /// <param name="entityType">Tipo da entidade</param>
    /// <param name="entityId">ID da entidade</param>
    /// <param name="additionalData">Dados adicionais</param>
    protected void LogBusinessOperation(string operation, string entityType, object? entityId = null, object? additionalData = null)
    {
        var userId = GetCurrentUserId();
        var requestId = GetRequestId();
        
        _logger.LogInformation("Operação: {Operation} | Entidade: {EntityType} | ID: {EntityId} | Usuário: {UserId} | RequestId: {RequestId} | Dados: {@AdditionalData}",
            operation, entityType, entityId, userId, requestId, additionalData);
    }

    /// <summary>
    /// Valida se o modelo está válido e retorna erro se não estiver
    /// </summary>
    /// <returns>BadRequest com erros de validação ou null se válido</returns>
    protected IActionResult? ValidateModel()
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var errorMessage = string.Join("; ", errors);
            
            _logger.LogWarning("Erro de validação na requisição {RequestId}: {Errors}", GetRequestId(), errorMessage);
            
            return BadRequest(CreateErrorResponse("Dados inválidos fornecidos", errorMessage));
        }

        return null;
    }
}