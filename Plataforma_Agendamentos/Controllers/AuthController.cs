using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Constants;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs;
using Plataforma_Agendamentos.Models;
using Plataforma_Agendamentos.Services;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AppDbContext context, JwtService jwtService, ILogger<AuthController> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Tentativa de registro: {Email}, Tipo: {UserType}", 
            request.Email, request.UserTypes.FirstOrDefault());

        if (!ModelState.IsValid)
            return BadRequest(new { 
                success = false, 
                message = "Dados invalidos", 
                errors = ModelState 
            });

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // Verificar se o email já existe
        if (await _context.Users.AnyAsync(u => u.Email == normalizedEmail))
        {
            _logger.LogWarning("Tentativa de registro com email duplicado: {Email}", normalizedEmail);
            return BadRequest(new { 
                success = false, 
                message = "Email ja esta em uso." 
            });
        }

        var userType = request.UserTypes.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(userType))
            return BadRequest(new { 
                success = false, 
                message = "Tipo de usuario deve ser 'cliente' ou 'prestador'." 
            });

        // Validar tipo de usuário
        if (userType != UserTypes.Cliente && userType != UserTypes.Prestador)
            return BadRequest(new { 
                success = false, 
                message = "Tipo de usuario deve ser 'cliente' ou 'prestador'." 
            });

        var user = new User
        {
            Name = request.Name,
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            UserType = userType
        };

        _context.Users.Add(user);
        
        // Criar perfil automaticamente conforme tipo de usuario
        if (userType == UserTypes.Cliente)
        {
            var clientePerfil = new ClientePerfil
            {
                UserId = user.Id
            };
            _context.ClientePerfis.Add(clientePerfil);
        }
        else if (userType == UserTypes.Prestador)
        {
            var prestadorPerfil = new PrestadorPerfil
            {
                UserId = user.Id,
                DisplayName = request.Name // Usar nome como display name inicial
            };
            prestadorPerfil.GerarSlug(); // Gerar slug automaticamente
            
            _context.PrestadorPerfis.Add(prestadorPerfil);
            
            // Criar branding e metricas vazios
            _context.PrestadorBrandings.Add(new PrestadorBranding { PrestadorPerfilId = prestadorPerfil.Id });
            _context.PrestadorMetricas.Add(new PrestadorMetricas { PrestadorPerfilId = prestadorPerfil.Id });
        }
        
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateJwtToken(user.Id.ToString(), user.Email, user.UserType);

        _logger.LogInformation("Usuario registrado com sucesso: {UserId}, {Email}", user.Id, user.Email);

        return Ok(new
        {
            success = true,
            message = "Usuario registrado com sucesso",
            data = new
            {
                token = token,
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    userType = user.UserType
                }
            }
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Tentativa de login: {Email}", request.Email);

        if (!ModelState.IsValid)
            return BadRequest(new { 
                success = false, 
                message = "Dados invalidos", 
                errors = ModelState 
            });

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _context.Users
            .Include(u => u.PrestadorPerfil)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Falha no login: {Email}", normalizedEmail);
            return Unauthorized(new { 
                success = false, 
                message = "Credenciais invalidas." 
            });
        }

        var token = _jwtService.GenerateJwtToken(user.Id.ToString(), user.Email, user.UserType);

        _logger.LogInformation("Login bem-sucedido: {UserId}, {Email}", user.Id, user.Email);

        var response = new
        {
            success = true,
            message = "Login realizado com sucesso",
            data = new
            {
                token = token,
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    userType = user.UserType,
                    slug = user.PrestadorPerfil?.Slug,
                    displayName = user.PrestadorPerfil?.DisplayName
                }
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Endpoint de teste para verificar se a API esta respondendo
    /// </summary>
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        _logger.LogInformation("Ping recebido de {Origin}", Request.Headers["Origin"].ToString());
        
        return Ok(new
        {
            success = true,
            message = "API esta funcionando!",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }
}
