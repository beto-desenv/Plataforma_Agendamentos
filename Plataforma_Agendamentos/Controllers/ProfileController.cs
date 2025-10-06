using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs;
using System.Security.Claims;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(AppDbContext context, ILogger<ProfileController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("Buscando perfil para usuário: {UserId}", userId);
        
        if (userId == null)
        {
            _logger.LogWarning("Token JWT inválido ou usuário não encontrado nas claims");
            return Unauthorized("Token inválido");
        }

        var user = await _context.Users
            .Include(u => u.Services)
            .Include(u => u.Schedules)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            _logger.LogWarning("Usuário não encontrado no banco: {UserId}", userId);
            return NotFound("Usuário não encontrado");
        }

        _logger.LogInformation("Perfil encontrado para usuário: {UserId}", userId);

        return Ok(new
        {
            user.Id,
            user.Name,
            user.Email,
            UserTypes = user.GetRoles(),
            user.Slug,
            user.DisplayName,
            user.LogoUrl,
            user.CoverImageUrl,
            user.PrimaryColor,
            user.Bio,
            Services = user.Services.Select(s => new
            {
                s.Id,
                s.Title,
                s.Description,
                s.Price,
                s.DurationMinutes
            }),
            Schedules = user.Schedules.Select(sc => new
            {
                sc.Id,
                sc.DayOfWeek,
                sc.StartTime,
                sc.EndTime
            })
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("Atualizando perfil para usuário: {UserId}", userId);
        
        if (userId == null)
        {
            _logger.LogWarning("Token JWT inválido ou usuário não encontrado nas claims");
            return Unauthorized("Token inválido");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            _logger.LogWarning("Usuário não encontrado no banco: {UserId}", userId);
            return NotFound("Usuário não encontrado");
        }

        // Verificar se é prestador para atualizar perfil público
        if (!user.IsPrestador())
        {
            return BadRequest("Apenas prestadores podem ter perfil público");
        }

        // Log dos dados antes da atualização
        _logger.LogInformation("Dados ANTES da atualização - Slug: {OldSlug}, DisplayName: {OldDisplayName}", 
            user.Slug, user.DisplayName);

        // Verificar se o slug já existe (se foi fornecido)
        if (!string.IsNullOrEmpty(request.Slug) && request.Slug != user.Slug)
        {
            if (await _context.Users.AnyAsync(u => u.Slug == request.Slug && u.Id != userId))
            {
                _logger.LogWarning("Slug já existe: {Slug}", request.Slug);
                return BadRequest("Slug já está em uso.");
            }
        }

        // Atualizar dados
        user.Slug = request.Slug ?? user.Slug;
        user.DisplayName = request.DisplayName ?? user.DisplayName;
        user.LogoUrl = request.LogoUrl ?? user.LogoUrl;
        user.CoverImageUrl = request.CoverImageUrl ?? user.CoverImageUrl;
        user.PrimaryColor = request.PrimaryColor ?? user.PrimaryColor;
        user.Bio = request.Bio ?? user.Bio;

        // Log dos dados depois da atualização
        _logger.LogInformation("Dados DEPOIS da atualização - Slug: {NewSlug}, DisplayName: {NewDisplayName}", 
            user.Slug, user.DisplayName);

        try
        {
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Perfil atualizado com sucesso. {Changes} alterações salvas no banco", changes);

            if (changes == 0)
            {
                _logger.LogWarning("Nenhuma alteração foi salva no banco de dados");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar alterações no banco de dados");
            return StatusCode(500, "Erro interno do servidor");
        }

        return Ok(new
        {
            user.Id,
            user.Name,
            user.Email,
            UserTypes = user.GetRoles(),
            user.Slug,
            user.DisplayName,
            user.LogoUrl,
            user.CoverImageUrl,
            user.PrimaryColor,
            user.Bio
        });
    }

    private Guid? GetCurrentUserId()
    {
        // Tentar múltiplas claims para compatibilidade
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value 
                         ?? User.FindFirst("Sub")?.Value;
        
        _logger.LogInformation("Claims encontradas: {Claims}", 
            string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));
        
        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("UserId claim não encontrada no token JWT");
            return null;
        }

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogInformation("UserId extraído do token: {UserId}", userId);
            return userId;
        }

        _logger.LogWarning("UserId claim não é um GUID válido: {UserIdClaim}", userIdClaim);
        return null;
    }
}