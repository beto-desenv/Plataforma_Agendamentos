using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Controllers.Base;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs;
using Plataforma_Agendamentos.Models;
using Plataforma_Agendamentos.Services;
using System.Security.Claims;

namespace Plataforma_Agendamentos.Controllers;

/// <summary>
/// Controller responsável por operações de autenticação e autorização
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : BaseApiController
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService, ILogger<AuthController> logger)
        : base(logger)
    {
        _context = context;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    /// <param name="request">Dados do usuário a ser cadastrado</param>
    /// <returns>Token JWT e dados do usuário criado</returns>
    /// <response code="200">Usuário criado com sucesso</response>
    /// <response code="400">Dados inválidos ou email já em uso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var requestId = GetRequestId();
        _logger.LogInformation("Iniciando registro de usuário - RequestId: {RequestId}, Email: {Email}, UserTypes: {UserTypes}", 
            requestId, request.Email, string.Join(",", request.UserTypes));

        var validationResult = ValidateModel();
        if (validationResult != null)
            return validationResult;

        try
        {
            // Validar tipos de usuário - usar apenas o primeiro tipo por enquanto
            var validTypes = new[] { "cliente", "prestador" };
            var userType = request.UserTypes.FirstOrDefault();
            
            if (string.IsNullOrEmpty(userType) || !validTypes.Contains(userType))
            {
                LogBusinessOperation("REGISTER_FAILED", "User", null, new { Reason = "InvalidUserType", UserTypes = request.UserTypes });
                return BadRequest(CreateErrorResponse("Tipo de usuário deve ser 'cliente' ou 'prestador'.", 
                    $"Tipos fornecidos: {string.Join(", ", request.UserTypes)}"));
            }

            // Verificar se o email já existe
            var existingUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
            
            if (existingUser != null)
            {
                _logger.LogWarning("Tentativa de registro com email existente - RequestId: {RequestId}, Email: {Email}", requestId, request.Email);
                LogBusinessOperation("REGISTER_FAILED", "User", null, new { Reason = "EmailAlreadyExists", Email = request.Email });
                return BadRequest(CreateErrorResponse("Email já está em uso.", 
                    "Para adicionar roles, use o endpoint de login."));
            }

            // Criar novo usuário
            var user = new User
            {
                Name = request.Name.Trim(),
                Email = request.Email.ToLower().Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                UserType = userType
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuário criado com sucesso - RequestId: {RequestId}, UserId: {UserId}, Email: {Email}", 
                requestId, user.Id, user.Email);

            // Gerar token JWT
            var token = _jwtService.GenerateJwtToken(
                user.Id.ToString(), 
                user.Email, 
                user.UserType
            );

            LogBusinessOperation("REGISTER_SUCCESS", "User", user.Id, new { UserType = userType });

            var responseData = new
            {
                Message = "Usuário criado com sucesso",
                Token = token,
                User = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    UserType = user.UserType,
                    UserTypes = user.GetRoles(), // Para compatibilidade
                    user.Slug,
                    user.DisplayName
                }
            };

            return Ok(CreateSuccessResponse(responseData, "Usuário registrado com sucesso"));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao registrar usuário - RequestId: {RequestId}, Email: {Email}", requestId, request.Email);
            return StatusCode(500, CreateErrorResponse("Erro interno do servidor", "Falha ao salvar dados do usuário"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao registrar usuário - RequestId: {RequestId}, Email: {Email}", requestId, request.Email);
            return StatusCode(500, CreateErrorResponse("Erro interno do servidor", "Erro inesperado durante o registro"));
        }
    }

    /// <summary>
    /// Realiza login de usuário no sistema
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Token JWT e dados do usuário</returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Credenciais inválidas</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var requestId = GetRequestId();
        _logger.LogInformation("Tentativa de login - RequestId: {RequestId}, Email: {Email}", requestId, request.Email);

        var validationResult = ValidateModel();
        if (validationResult != null)
            return validationResult;

        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                _logger.LogWarning("Tentativa de login com email inexistente - RequestId: {RequestId}, Email: {Email}", requestId, request.Email);
                LogBusinessOperation("LOGIN_FAILED", "User", null, new { Reason = "UserNotFound", Email = request.Email });
                return Unauthorized(CreateErrorResponse("Credenciais inválidas.", "Email ou senha incorretos"));
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Tentativa de login com senha incorreta - RequestId: {RequestId}, UserId: {UserId}", requestId, user.Id);
                LogBusinessOperation("LOGIN_FAILED", "User", user.Id, new { Reason = "InvalidPassword" });
                return Unauthorized(CreateErrorResponse("Credenciais inválidas.", "Email ou senha incorretos"));
            }

            // Gerar token JWT
            var token = _jwtService.GenerateJwtToken(
                user.Id.ToString(), 
                user.Email, 
                user.UserType
            );

            _logger.LogInformation("Login realizado com sucesso - RequestId: {RequestId}, UserId: {UserId}", requestId, user.Id);
            LogBusinessOperation("LOGIN_SUCCESS", "User", user.Id, new { UserType = user.UserType });

            var responseData = new
            {
                Message = "Login realizado com sucesso",
                Token = token,
                User = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    UserType = user.UserType,
                    UserTypes = user.GetRoles(), // Para compatibilidade
                    user.Slug,
                    user.DisplayName
                }
            };

            return Ok(CreateSuccessResponse(responseData, "Login realizado com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado durante login - RequestId: {RequestId}, Email: {Email}", requestId, request.Email);
            return StatusCode(500, CreateErrorResponse("Erro interno do servidor", "Erro inesperado durante o login"));
        }
    }

    /// <summary>
    /// Adiciona um novo role ao usuário atual (funcionalidade futura)
    /// </summary>
    /// <param name="request">Role a ser adicionado</param>
    /// <returns>Mensagem informativa</returns>
    /// <response code="400">Funcionalidade não implementada</response>
    [HttpPost("add-role")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddRole([FromBody] AddRoleRequest request)
    {
        var requestId = GetRequestId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("Tentativa de adicionar role - RequestId: {RequestId}, UserId: {UserId}, Role: {Role}", 
            requestId, userId, request.Role);

        LogBusinessOperation("ADD_ROLE_ATTEMPTED", "User", userId, new { RequestedRole = request.Role });

        return BadRequest(CreateErrorResponse(
            "Funcionalidade multi-role será implementada em versão futura.", 
            "Use cadastros separados por enquanto."));
    }

    /// <summary>
    /// Obtém informações do usuário atual
    /// </summary>
    /// <returns>Dados do usuário logado</returns>
    /// <response code="200">Dados do usuário</response>
    /// <response code="401">Token inválido ou expirado</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var requestId = GetRequestId();
        var userId = GetCurrentUserId();
        
        if (userId == null)
        {
            _logger.LogWarning("Tentativa de acesso ao perfil sem token válido - RequestId: {RequestId}", requestId);
            return Unauthorized(CreateErrorResponse("Token inválido ou expirado"));
        }

        _logger.LogInformation("Buscando perfil do usuário - RequestId: {RequestId}, UserId: {UserId}", requestId, userId);

        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado no banco - RequestId: {RequestId}, UserId: {UserId}", requestId, userId);
                return NotFound(CreateErrorResponse("Usuário não encontrado"));
            }

            LogBusinessOperation("PROFILE_ACCESSED", "User", userId);

            var userData = new
            {
                user.Id,
                user.Name,
                user.Email,
                UserType = user.UserType,
                UserTypes = user.GetRoles(), // Para compatibilidade
                user.Slug,
                user.DisplayName,
                user.LogoUrl,
                user.CoverImageUrl,
                user.PrimaryColor,
                user.Bio
            };

            return Ok(CreateSuccessResponse(userData, "Perfil obtido com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar perfil do usuário - RequestId: {RequestId}, UserId: {UserId}", requestId, userId);
            return StatusCode(500, CreateErrorResponse("Erro interno do servidor", "Erro ao buscar dados do usuário"));
        }
    }
}