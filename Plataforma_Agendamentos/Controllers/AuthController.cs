using Microsoft.AspNetCore.Mvc;
using Plataforma_Agendamentos.DTOs.Auth;
using Plataforma_Agendamentos.Services.Interfaces;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
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

        var result = await _authService.RegisterAsync(request);

        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = result.ErrorMessage
            });
        }

        return Ok(new
        {
            success = true,
            message = "Usuario registrado com sucesso",
            data = new
            {
                token = result.Token,
                user = result.User
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

        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(new
            {
                success = false,
                message = result.ErrorMessage
            });
        }

        return Ok(new
        {
            success = true,
            message = "Login realizado com sucesso",
            data = new
            {
                token = result.Token,
                user = result.User
            }
        });
    }
}
