using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Controllers.Base;
using Plataforma_Agendamentos.Data;
using System.Diagnostics;

namespace Plataforma_Agendamentos.Controllers;

/// <summary>
/// Controller para verificacao de saude da API
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
    /// Verifica o status de saude da API e suas dependencias
    /// </summary>
    /// <returns>Status detalhado de saude</returns>
    /// <response code="200">API funcionando normalmente</response>
    /// <response code="503">Servicos indisponiveis</response>
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
            // 1. Verificar API basica
            var apiCheckStart = DateTime.UtcNow;
            healthChecks.Add(new
            {
                name = "API",
                status = "Healthy",
                description = "API esta respondendo",
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
                    description = canConnect ? $"Conectado - {userCount} usuarios" : "Nao foi possivel conectar",
                    duration = (DateTime.UtcNow - dbCheckStart).TotalMilliseconds
                });

                if (!canConnect)
                    isHealthy = false;
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Erro na verificacao do banco de dados");
                healthChecks.Add(new
                {
                    name = "Database",
                    status = "Unhealthy",
                    description = $"Erro: {dbEx.Message}",
                    duration = (DateTime.UtcNow - dbCheckStart).TotalMilliseconds
                });
                isHealthy = false;
            }

            // 3. Verificar memoria
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
                return Ok(CreateSuccessResponse(result, "Todos os servicos estao funcionando"));
            }
            else
            {
                return StatusCode(503, CreateErrorResponse("Alguns servicos estao indisponiveis", 
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
    /// Verificacao rapida de disponibilidade (ping)
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
            message = "API esta respondendo"
        });
    }

    /// <summary>
    /// Informacoes detalhadas do sistema
    /// </summary>
    /// <returns>Informacoes tecnicas da aplicacao</returns>
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
                framework = ".NET 8.0",
                swagger = "/swagger"
            },
            endpoints = new
            {
                auth = new
                {
                    register = "POST /api/auth/register",
                    login = "POST /api/auth/login"
                },
                health = new
                {
                    status = "GET /api/health",
                    ping = "GET /api/health/ping",
                    info = "GET /api/health/info"
                },
                profile = new
                {
                    get = "GET /api/profile",
                    updateCliente = "PUT /api/profile/cliente",
                    updatePrestador = "PUT /api/profile/prestador"
                },
                services = new
                {
                    list = "GET /api/services",
                    create = "POST /api/services",
                    get = "GET /api/services/{id}",
                    update = "PUT /api/services/{id}",
                    delete = "DELETE /api/services/{id}"
                },
                schedules = new
                {
                    list = "GET /api/schedules",
                    create = "POST /api/schedules",
                    get = "GET /api/schedules/{id}",
                    update = "PUT /api/schedules/{id}",
                    delete = "DELETE /api/schedules/{id}"
                },
                bookings = new
                {
                    list = "GET /api/bookings",
                    create = "POST /api/bookings",
                    get = "GET /api/bookings/{id}",
                    updateStatus = "PUT /api/bookings/{id}/status"
                },
                publicAccess = new
                {
                    prestador = "GET /api/prestador/{slug}",
                    availableTimes = "GET /api/prestador/{slug}/available-times",
                    cep = "POST /api/ceps/consultar"
                }
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

        return Ok(CreateSuccessResponse(info, "Informacoes do sistema obtidas"));
    }
}