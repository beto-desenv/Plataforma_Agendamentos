using System.Diagnostics;
using System.Text;

namespace Plataforma_Agendamentos.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();
        
        // Adicionar requestId ao contexto para rastreamento
        context.Items["RequestId"] = requestId;

        // Log da requisição de entrada
        await LogRequestAsync(context, requestId);

        var originalResponseBodyStream = context.Response.Body;

        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            // Log da resposta
            await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds);
            
            // Copiar conteúdo de volta para o stream original
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
            context.Response.Body = originalResponseBodyStream;
        }
    }

    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        var request = context.Request;
        var logData = new
        {
            RequestId = requestId,
            Method = request.Method,
            Path = request.Path.Value,
            QueryString = request.QueryString.Value,
            UserAgent = request.Headers["User-Agent"].ToString(),
            RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserId = context.User?.Identity?.Name
        };

        _logger.LogInformation("Requisição recebida: {@RequestData}", logData);

        // Log do body apenas para POST/PUT e em desenvolvimento
        if ((request.Method == "POST" || request.Method == "PUT") && 
            request.ContentLength > 0 && 
            request.ContentType?.Contains("application/json") == true)
        {
            request.EnableBuffering();
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;

            // Log do body (cuidado com dados sensíveis)
            if (!string.IsNullOrEmpty(body) && !body.Contains("password", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug("Request Body para {RequestId}: {Body}", requestId, body);
            }
        }
    }

    private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMs)
    {
        var response = context.Response;
        
        response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        var logData = new
        {
            RequestId = requestId,
            StatusCode = response.StatusCode,
            ElapsedMilliseconds = elapsedMs,
            ResponseSize = responseText.Length
        };

        if (response.StatusCode >= 400)
        {
            _logger.LogWarning("Resposta com erro: {@ResponseData}", logData);
            
            if (response.StatusCode >= 500)
            {
                _logger.LogError("Response Body para {RequestId}: {Body}", requestId, responseText);
            }
        }
        else
        {
            _logger.LogInformation("Resposta enviada: {@ResponseData}", logData);
        }
    }
}