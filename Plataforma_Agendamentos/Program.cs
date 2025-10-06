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
using System.Globalization;

// ================================================
// PLATAFORMA DE AGENDAMENTOS - .NET 8
// ================================================

// Configurar encoding UTF-8 para evitar problemas de caracteres especiais
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Configurar cultura para pt-BR
var culture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// Configurar Serilog com encoding correto
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        standardErrorFromLevel: Serilog.Events.LogEventLevel.Error
    )
    .CreateLogger();

try
{
    Log.Information("Iniciando Plataforma de Agendamentos...");
    
    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog
    builder.Host.UseSerilog();

    // Configurar encoding para o builder
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR");
        options.SupportedCultures = new List<CultureInfo> { new CultureInfo("pt-BR") };
        options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("pt-BR") };
    });

    Log.Information("Configurando servicos...");

    // Configurar URLs explicitamente
    builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

    // Add services to the container
    builder.Services.AddControllers(options =>
    {
        // Configurar encoding para controllers
        options.RespectBrowserAcceptHeader = true;
    });

    // Configurar JSON com encoding UTF-8
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.PropertyNamingPolicy = null;
        options.SerializerOptions.WriteIndented = true;
    });

    // FluentValidation
    try
    {
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        Log.Information("FluentValidation configurado");
    }
    catch (Exception ex)
    {
        Log.Warning("Erro ao configurar FluentValidation: {Error}", ex.Message);
    }

    // API Explorer
    builder.Services.AddEndpointsApiExplorer();
    
    // Swagger sem XML comments (removido para evitar erro)
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "Plataforma de Agendamentos API", 
            Version = "v1.0.0",
            Description = "API completa para gerenciamento de agendamentos de servicos"
        });
        
        // Configuracao JWT no Swagger
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

    Log.Information("Swagger configurado");

    // Database Configuration
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Host=localhost;Database=plataforma_agendamentos_dev;Username=plataforma_user;Password=180312;Encoding=UTF8;";

    Log.Information("Configurando banco de dados: {DatabaseHost}", connectionString.Split(';')[0]);

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            // Configurar encoding para PostgreSQL
            npgsqlOptions.CommandTimeout(30);
        });
        
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });

    // JWT Configuration
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? "chave_desenvolvimento_256_bits_nao_usar_em_producao_jamais";
    
    Log.Information("Configurando autenticacao JWT...");

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
                Log.Warning("Falha na autenticacao JWT: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Debug("Token JWT validado para usuario: {UserId}", context.Principal?.Identity?.Name);
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
    Log.Information("Configurando health checks...");
    builder.Services.AddHealthChecks()
        .AddCheck("application", () =>
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Application is running");
        });

    Log.Information("Construindo aplicacao...");
    var app = builder.Build();

    // Configure the HTTP request pipeline
    Log.Information("Configurando middleware pipeline...");

    // Configurar localization
    app.UseRequestLocalization();

    // Swagger
    Log.Information("Configurando Swagger UI");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Plataforma de Agendamentos API V1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });

    // Global exception handling
    try
    {
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        Log.Information("Middleware de tratamento de erros configurado");
    }
    catch (Exception ex)
    {
        Log.Warning("Erro ao configurar middleware de erros: {Error}", ex.Message);
    }

    // Request logging (apenas em desenvolvimento)
    if (app.Environment.IsDevelopment())
    {
        try
        {
            app.UseMiddleware<RequestLoggingMiddleware>();
            Log.Information("Middleware de logging configurado");
        }
        catch (Exception ex)
        {
            Log.Warning("Erro ao configurar middleware de logging: {Error}", ex.Message);
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
        context.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
        await next();
    });

    app.UseHttpsRedirection();
    app.UseCors("AllowAll");

    // Health checks endpoint
    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json; charset=utf-8";
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
            
            var jsonBytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
            
            await context.Response.Body.WriteAsync(jsonBytes);
        }
    });

    app.UseAuthentication();
    app.UseAuthorization();

    // Home endpoint com informacoes da API
    app.MapGet("/", () => new
    {
        Message = "Plataforma de Agendamentos API esta funcionando!",
        Version = "v1.0.0",
        Environment = app.Environment.EnvironmentName,
        Timestamp = DateTime.UtcNow,
        Encoding = "UTF-8",
        Culture = CultureInfo.CurrentCulture.Name,
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
    }).WithTags("System").WithSummary("Informacoes da API");

    app.MapControllers();

    // Log URLs da aplicacao
    Log.Information("===============================================");
    Log.Information("PLATAFORMA DE AGENDAMENTOS INICIADA!");
    Log.Information("===============================================");
    Log.Information("URLs disponiveis:");
    Log.Information("   Home: https://localhost:5001/");
    Log.Information("   Swagger: https://localhost:5001/swagger");
    Log.Information("   Health: https://localhost:5001/health");
    Log.Information("   API: https://localhost:5001/api");
    Log.Information("Encoding configurado: UTF-8");
    Log.Information("Cultura configurada: {Culture}", CultureInfo.CurrentCulture.Name);
    Log.Information("===============================================");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ERRO CRITICO: Aplicacao falhou ao iniciar");
    Console.WriteLine($"ERRO CRITICO: {ex.Message}");
    Console.WriteLine($"Stack: {ex.StackTrace}");
    Console.WriteLine("Pressione qualquer tecla para sair...");
    Console.ReadKey();
    Environment.Exit(1);
}
finally
{
    Log.CloseAndFlush();
}
