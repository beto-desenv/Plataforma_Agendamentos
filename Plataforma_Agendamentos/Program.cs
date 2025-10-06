using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.Services;
using Plataforma_Agendamentos.Middleware;
using Plataforma_Agendamentos.Validators;
using System.Text;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;

// ================================================
// PLATAFORMA DE AGENDAMENTOS - .NET 8 (CORRIGIDO)
// ================================================

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("?? Iniciando Plataforma de Agendamentos...");
    
    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog
    builder.Host.UseSerilog();

    Log.Information("?? Configurando serviços...");

    // Configurar URLs explicitamente
    builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

    // Add services to the container
    builder.Services.AddControllers();

    // FluentValidation
    try
    {
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        Log.Information("? FluentValidation configurado");
    }
    catch (Exception ex)
    {
        Log.Warning("?? Erro ao configurar FluentValidation: {Error}", ex.Message);
    }

    // API Explorer
    builder.Services.AddEndpointsApiExplorer();
    
    // Swagger
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "Plataforma de Agendamentos API", 
            Version = "v1.0.0",
            Description = "API completa para gerenciamento de agendamentos de serviços"
        });
        
        // Configuração JWT no Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header usando Bearer scheme. Exemplo: \"Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    Log.Information("? Swagger configurado");

    // Database Configuration
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Host=localhost;Database=plataforma_agendamentos_dev;Username=plataforma_user;Password=180312";

    Log.Information("??? Configurando banco de dados: {DatabaseHost}", connectionString.Split(';')[0]);

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(connectionString);
        
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });

    // JWT Configuration
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? "chave_desenvolvimento_256_bits_nao_usar_em_producao_jamais";
    
    Log.Information("?? Configurando autenticação JWT...");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "PlataformaAgendamentos",
            ValidAudience = jwtSettings["Audience"] ?? "PlataformaAgendamentos",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning("Falha na autenticação JWT: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Debug("Token JWT validado para usuário: {UserId}", context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

    // Services
    builder.Services.AddScoped<JwtService>();

    // CORS Configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Health Checks
    Log.Information("?? Configurando health checks...");
    builder.Services.AddHealthChecks()
        .AddCheck("application", () =>
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Application is running");
        });

    Log.Information("??? Construindo aplicação...");
    var app = builder.Build();

    // Configure the HTTP request pipeline
    Log.Information("?? Configurando middleware pipeline...");

    // Swagger
    Log.Information("?? Configurando Swagger UI");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Plataforma de Agendamentos API V1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });

    // Global exception handling
    try
    {
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        Log.Information("? Middleware de tratamento de erros configurado");
    }
    catch (Exception ex)
    {
        Log.Warning("?? Erro ao configurar middleware de erros: {Error}", ex.Message);
    }

    // Request logging (apenas em desenvolvimento)
    if (app.Environment.IsDevelopment())
    {
        try
        {
            app.UseMiddleware<RequestLoggingMiddleware>();
            Log.Information("? Middleware de logging configurado");
        }
        catch (Exception ex)
        {
            Log.Warning("?? Erro ao configurar middleware de logging: {Error}", ex.Message);
        }

        app.UseDeveloperExceptionPage();
    }

    // Security headers
    app.Use(async (context, next) =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        await next();
    });

    app.UseHttpsRedirection();
    app.UseCors("AllowAll");

    // Health checks endpoint
    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(x => new
                {
                    name = x.Key,
                    status = x.Value.Status.ToString(),
                    description = x.Value.Description,
                    duration = x.Value.Duration.TotalMilliseconds
                }),
                totalDuration = report.TotalDuration.TotalMilliseconds
            };
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    });

    app.UseAuthentication();
    app.UseAuthorization();

    // Home endpoint com informações da API
    app.MapGet("/", () => new
    {
        Message = "?? Plataforma de Agendamentos API está funcionando!",
        Version = "v1.0.0",
        Environment = app.Environment.EnvironmentName,
        Timestamp = DateTime.UtcNow,
        Endpoints = new
        {
            Swagger = "/swagger",
            Health = "/health",
            Auth = "/api/auth",
            Profile = "/api/profile",
            Services = "/api/services",
            Schedules = "/api/schedules",
            Bookings = "/api/bookings",
            Public = "/api/prestador/{slug}"
        }
    }).WithTags("System").WithSummary("Informações da API");

    app.MapControllers();

    // Log URLs da aplicação
    Log.Information("?? ===============================================");
    Log.Information("?? PLATAFORMA DE AGENDAMENTOS INICIADA!");
    Log.Information("?? ===============================================");
    Log.Information("?? URLs disponíveis:");
    Log.Information("   ?? Home: https://localhost:5001/");
    Log.Information("   ?? Swagger: https://localhost:5001/swagger");
    Log.Information("   ?? Health: https://localhost:5001/health");
    Log.Information("   ?? API: https://localhost:5001/api");
    Log.Information("?? ===============================================");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "?? ERRO CRÍTICO: Aplicação falhou ao iniciar");
    Console.WriteLine($"?? ERRO CRÍTICO: {ex.Message}");
    Console.WriteLine($"?? Stack: {ex.StackTrace}");
    Console.WriteLine("Pressione qualquer tecla para sair...");
    Console.ReadKey();
    Environment.Exit(1);
}
finally
{
    Log.CloseAndFlush();
}
