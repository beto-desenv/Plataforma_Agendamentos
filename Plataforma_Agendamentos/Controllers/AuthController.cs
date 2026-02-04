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

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // Verificar se o email já existe
        if (await _context.Users.AnyAsync(u => u.Email == normalizedEmail))
            return BadRequest("Email já está em uso.");

        // Validar tipo de usuário
        if (request.UserType != UserTypes.Cliente && request.UserType != UserTypes.Prestador)
            return BadRequest("Tipo de usuário deve ser 'cliente' ou 'prestador'.");

        var user = new User
        {
            Name = request.Name,
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            UserType = request.UserType
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateJwtToken(user.Id.ToString(), user.Email, user.UserType);

        return Ok(new
        {
            Token = token,
            User = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.UserType,
                user.Slug,
                user.DisplayName
            }
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Credenciais inválidas.");

        var token = _jwtService.GenerateJwtToken(user.Id.ToString(), user.Email, user.UserType);

        return Ok(new
        {
            Token = token,
            User = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.UserType,
                user.Slug,
                user.DisplayName
            }
        });
    }
}
