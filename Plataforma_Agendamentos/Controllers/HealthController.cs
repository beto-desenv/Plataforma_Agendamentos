using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Controllers.Base;
using Plataforma_Agendamentos.Data;
using System.Diagnostics;

namespace Plataforma_Agendamentos.Controllers;

/// <summary>
/// Controller para verificação de saúde da API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : BaseApiController
{
    private readonly AppDbContext _context;

    public HealthController(AppDbContext context, ILogger<HealthController> logger)
        : base(logger)
    {
        _context = context;
    }

    /// <summary>
    /// Verifica o status de saúde da API e suas dependências
    /// </summary>
    /// <returns>Status detalhado de saúde</returns>
    /// <response code="200">API funcionando normalmente</response>
    /// <response code="503">Serviços indisponíveis</response>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CheckHealth()
    {
        var requestId = GetRequestId();
        var startTime = DateTime.UtcNow;
        
        _logger.LogInformation("Health check iniciado - RequestId: {RequestId}", requestId);

        var healthChecks = new List<object>();
        var isHealthy = true;

        try
        {
            // 1. Verificar API básica
            var apiCheckStart = DateTime.UtcNow;
            healthChecks.Add(new
            {
                name = "API",
                status = "Healthy",
                description = "API está respondendo",
                duration = (DateTime.UtcNow - apiCheckStart).TotalMilliseconds
            });

            // 2. Verificar banco de dados
            var dbCheckStart = DateTime.UtcNow;
            try
            {
                // Tentar uma query simples no banco
                var canConnect = await _context.Database.CanConnectAsync();
                var userCount = canConnect ? await _context.Users.CountAsync() : 0;
                
                healthChecks.Add(new
                {
                    name = "Database",
                    status = canConnect ? "Healthy" : "Unhealthy",
                    description = canConnect ? $"Conectado - {userCount} usuários" : "Não foi possível conectar",
                    duration = (DateTime.UtcNow - dbCheckStart).TotalMilliseconds
                });

                if (!canConnect)
                    isHealthy = false;
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Erro na verificação do banco de dados");
                healthChecks.Add(new
                {
                    name = "Database",
                    status = "Unhealthy",
                    description = $"Erro: {dbEx.Message}",
                    duration = (DateTime.UtcNow - dbCheckStart).TotalMilliseconds
                });
                isHealthy = false;
            }

            // 3. Verificar memória
            var memoryCheckStart = DateTime.UtcNow;
            var workingSet = Environment.WorkingSet;
            var memoryMB = workingSet / 1024 / 1024;
            
            healthChecks.Add(new
            {
                name = "Memory",
                status = memoryMB < 500 ? "Healthy" : "Warning",
                description = $"Usando {memoryMB} MB",
                duration = (DateTime.UtcNow - memoryCheckStart).TotalMilliseconds
            });

            var totalDuration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            
            var result = new
            {
                status = isHealthy ? "Healthy" : "Unhealthy",
                checks = healthChecks,
                totalDuration = totalDuration,
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            };

            LogBusinessOperation("HEALTH_CHECK", "System", null, new { 
                Status = result.status, 
                Duration = totalDuration,
                ChecksCount = healthChecks.Count
            });

            if (isHealthy)
            {
                return Ok(CreateSuccessResponse(result, "Todos os serviços estão funcionando"));
            }
            else
            {
                return StatusCode(503, CreateErrorResponse("Alguns serviços estão indisponíveis", 
                    "Verifique os detalhes dos health checks"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante health check - RequestId: {RequestId}", requestId);
            
            var errorResult = new
            {
                status = "Unhealthy",
                error = ex.Message,
                timestamp = DateTime.UtcNow,
                totalDuration = (DateTime.UtcNow - startTime).TotalMilliseconds
            };

            return StatusCode(503, CreateErrorResponse("Health check falhou", ex.Message));
        }
    }

    /// <summary>
    /// Verificação rápida de disponibilidade (ping)
    /// </summary>
    /// <returns>Resposta simples de disponibilidade</returns>
    [HttpGet("ping")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        return Ok(new
        {
            status = "OK",
            timestamp = DateTime.UtcNow,
            message = "API está respondendo"
        });
    }

    /// <summary>
    /// Informações detalhadas do sistema
    /// </summary>
    /// <returns>Informações técnicas da aplicação</returns>
    [HttpGet("info")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetSystemInfo()
    {
        var info = new
        {
            application = new
            {
                name = "Plataforma de Agendamentos",
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                framework = ".NET 8.0"
            },
            system = new
            {
                machineName = Environment.MachineName,
                osVersion = Environment.OSVersion.ToString(),
                processId = Environment.ProcessId,
                workingSet = $"{Environment.WorkingSet / 1024 / 1024} MB",
                uptime = DateTime.UtcNow.Subtract(Process.GetCurrentProcess().StartTime.ToUniversalTime())
            },
            features = new
            {
                authentication = "JWT Bearer",
                database = "PostgreSQL + Entity Framework",
                logging = "Serilog",
                validation = "FluentValidation",
                documentation = "Swagger/OpenAPI"
            }
        };

        return Ok(CreateSuccessResponse(info, "Informações do sistema obtidas"));
    }
}