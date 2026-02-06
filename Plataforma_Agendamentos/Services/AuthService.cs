using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Constants;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs.Auth;
using Plataforma_Agendamentos.Models;
using Plataforma_Agendamentos.Services.Interfaces;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Service para gerenciar autenticacao e registro de usuarios
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context, 
        JwtService jwtService, 
        ILogger<AuthService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        // Validar tipo de usuario
        var userType = request.UserTypes.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(userType))
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Tipo de usuario deve ser 'cliente' ou 'prestador'"
            };
        }

        if (userType != UserTypes.Cliente && userType != UserTypes.Prestador)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Tipo de usuario deve ser 'cliente' ou 'prestador'"
            };
        }

        // Normalizar email
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // Verificar se email ja existe
        if (await EmailExistsAsync(normalizedEmail))
        {
            _logger.LogWarning("Tentativa de registro com email duplicado: {Email}", normalizedEmail);
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Email ja esta em uso"
            };
        }

        // Criar usuario
        var user = new User
        {
            Name = request.Name,
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            UserType = userType
        };

        _context.Users.Add(user);

        // Criar perfil automaticamente conforme tipo
        if (userType == UserTypes.Cliente)
        {
            await CreateClienteProfileAsync(user);
        }
        else if (userType == UserTypes.Prestador)
        {
            await CreatePrestadorProfileAsync(user, request.Name);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuario registrado com sucesso: {UserId}, {Email}, {UserType}", 
            user.Id, user.Email, user.UserType);

        // Gerar token
        var token = _jwtService.GenerateJwtToken(user.Id.ToString(), user.Email, user.UserType);

        // Recarregar usuario com perfil para obter slug/displayName
        user = await _context.Users
            .Include(u => u.PrestadorPerfil)
            .FirstAsync(u => u.Id == user.Id);

        return new AuthResult
        {
            Success = true,
            Token = token,
            User = MapToUserDto(user)
        };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .Include(u => u.PrestadorPerfil)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        // Verificar credenciais
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Falha no login: {Email}", normalizedEmail);
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Credenciais invalidas"
            };
        }

        // Gerar token
        var token = _jwtService.GenerateJwtToken(user.Id.ToString(), user.Email, user.UserType);

        _logger.LogInformation("Login bem-sucedido: {UserId}, {Email}", user.Id, user.Email);

        return new AuthResult
        {
            Success = true,
            Token = token,
            User = MapToUserDto(user)
        };
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _context.Users.AnyAsync(u => u.Email == normalizedEmail);
    }

    // Helpers privados

    private async Task CreateClienteProfileAsync(User user)
    {
        var clientePerfil = new ClientePerfil
        {
            UserId = user.Id
        };
        _context.ClientePerfis.Add(clientePerfil);
        
        _logger.LogInformation("Perfil de cliente criado para usuario {UserId}", user.Id);
    }

    private async Task CreatePrestadorProfileAsync(User user, string name)
    {
        var prestadorPerfil = new PrestadorPerfil
        {
            UserId = user.Id,
            DisplayName = name
        };
        
        // Gerar slug automaticamente
        prestadorPerfil.GerarSlug();
        
        _context.PrestadorPerfis.Add(prestadorPerfil);

        // Criar branding e metricas vazios
        var branding = new PrestadorBranding
        {
            PrestadorPerfilId = prestadorPerfil.Id
        };
        _context.PrestadorBrandings.Add(branding);

        var metricas = new PrestadorMetricas
        {
            PrestadorPerfilId = prestadorPerfil.Id
        };
        _context.PrestadorMetricas.Add(metricas);

        _logger.LogInformation("Perfil de prestador criado para usuario {UserId} com slug {Slug}", 
            user.Id, prestadorPerfil.Slug);
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            UserType = user.UserType,
            Slug = user.PrestadorPerfil?.Slug,
            DisplayName = user.PrestadorPerfil?.DisplayName
        };
    }
}
