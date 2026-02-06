using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.Services;
using System.Text;

namespace Plataforma_Agendamentos
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Iniciando Plataforma de Agendamentos...");
                
                var builder = WebApplication.CreateBuilder(args);

                // Configurar logging para console
                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();
                
                // Configurar URLs explicitamente apenas quando ASPNETCORE_URLS não estiver definido
                var configuredUrls = builder.Configuration["ASPNETCORE_URLS"];
                if (string.IsNullOrWhiteSpace(configuredUrls))
                {
                    builder.WebHost.UseUrls("https://localhost:5001");
                }

                Console.WriteLine("Configurando serviços...");

                // Add services to the container.
                builder.Services.AddControllers();

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo 
                    { 
                        Title = "Plataforma de Agendamentos API", 
                        Version = "v1",
                        Description = "API completa para gerenciamento de agendamentos de serviços"
                    });
                    
                    // Configuração JWT no Swagger
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
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
                            new string[] {}
                        }
                    });
                });

                // Database Configuration
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                    ?? "Host=localhost;Database=plataforma_agendamentos;Username=postgres;Password=postgres";

                Console.WriteLine($"Configurando banco de dados: {connectionString.Split(';')[0]}...");

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
                var secretKey = jwtSettings["SecretKey"] ?? "sua_chave_secreta_muito_segura_com_pelo_menos_256_bits";

                Console.WriteLine("Configurando autenticação JWT...");

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
                });

                // Services
                builder.Services.AddScoped<JwtService>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<IProfileService, ProfileService>();
                builder.Services.AddScoped<IBookingService, BookingService>();
                builder.Services.AddMemoryCache();
                builder.Services.AddHttpClient<ICepService, CepService>();

                // CORS Configuration - Configurado para permitir frontend em localhost:3000
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowFrontend", policy =>
                    {
                        policy.WithOrigins(
                                "http://localhost:3000",      // Frontend em desenvolvimento
                                "https://localhost:3000",     // Frontend HTTPS
                                "http://localhost:5173",      // Vite default port
                                "https://localhost:5173"      // Vite HTTPS
                            )
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });

                    // Policy menos restritiva para desenvolvimento
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                });

                Console.WriteLine("Construindo aplicação...");
                var app = builder.Build();

                // Configure the HTTP request pipeline.
                Console.WriteLine("Configurando middleware...");
                
                // IMPORTANTE: Swagger deve estar antes do HTTPS redirect
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Plataforma de Agendamentos API V1");
                    c.RoutePrefix = "swagger"; // Swagger em /swagger
                    c.DisplayRequestDuration();
                    c.EnableTryItOutByDefault();
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                });

                if (app.Environment.IsDevelopment())
                {
                    Console.WriteLine("Modo DESENVOLVIMENTO ativo");
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseCors("AllowFrontend"); // Usar policy específica para o frontend
                app.UseAuthentication();
                app.UseAuthorization();

                // Adicionar rota de teste na raiz
                app.MapGet("/", () => new
                {
                    Message = "Plataforma de Agendamentos API está funcionando!",
                    Swagger = "/swagger",
                    Version = "v1.0.0",
                    Status = "OK",
                    Endpoints = new
                    {
                        Auth = "/api/auth",
                        Profile = "/api/profile",
                        Services = "/api/services",
                        Schedules = "/api/schedules",
                        Bookings = "/api/bookings",
                        Public = "/api/prestador/{slug}"
                    }
                });

                app.MapControllers();

                // Ensure database is created
                Console.WriteLine("Inicializando banco de dados...");
                try
                {
                    using (var scope = app.Services.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var created = context.Database.EnsureCreated();
                        
                        if (created)
                        {
                            Console.WriteLine("Banco de dados criado com sucesso!");
                        }
                        else
                        {
                            Console.WriteLine("Banco de dados já existe e está atualizado");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao inicializar banco de dados: {ex.Message}");
                    Console.WriteLine("Verifique se o PostgreSQL está rodando e as credenciais estão corretas");
                }

                // Log URLs da aplicação
                Console.WriteLine();
                Console.WriteLine("===============================================");
                Console.WriteLine("PLATAFORMA DE AGENDAMENTOS INICIADA!");
                Console.WriteLine("===============================================");
                Console.WriteLine();
                Console.WriteLine("URLs disponíveis:");
                Console.WriteLine("   Home: https://localhost:5001/");
                Console.WriteLine("   Swagger: https://localhost:5001/swagger");
                Console.WriteLine("   API: https://localhost:5001/api");
                Console.WriteLine();
                Console.WriteLine("Endpoints principais:");
                Console.WriteLine("   POST /api/auth/register - Cadastro");
                Console.WriteLine("   POST /api/auth/login    - Login");
                Console.WriteLine("   GET  /api/prestador/{slug} - Perfil público");
                Console.WriteLine();
                Console.WriteLine("Para testar:");
                Console.WriteLine("   1. Acesse: https://localhost:5001/");
                Console.WriteLine("   2. Acesse: https://localhost:5001/swagger");
                Console.WriteLine("   3. Use a Collection do Postman");
                Console.WriteLine();
                Console.WriteLine("Para parar a aplicação: Ctrl+C");
                Console.WriteLine("===============================================");
                Console.WriteLine();

                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO CRÍTICO ao iniciar aplicação: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine();
                Console.WriteLine("Possíveis soluções:");
                Console.WriteLine("   1. Verifique se as portas 5000/5001 estão livres");
                Console.WriteLine("   2. Execute como administrador");
                Console.WriteLine("   3. Verifique se o PostgreSQL está rodando");
                Console.WriteLine("   4. Confirme as configurações em appsettings.json");
                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para sair...");
                Console.ReadKey();
                
                Environment.Exit(1);
            }
        }
    }
}
