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

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _context.Users
            .Include(u => u.Services)
            .Include(u => u.Schedules)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.Name,
            user.Email,
            user.UserType,
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
        if (userId == null)
            return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound();

        // Verificar se o slug já existe (se foi fornecido)
        if (!string.IsNullOrEmpty(request.Slug) && request.Slug != user.Slug)
        {
            if (await _context.Users.AnyAsync(u => u.Slug == request.Slug && u.Id != userId))
                return BadRequest("Slug já está em uso.");
        }

        user.Slug = request.Slug ?? user.Slug;
        user.DisplayName = request.DisplayName ?? user.DisplayName;
        user.LogoUrl = request.LogoUrl ?? user.LogoUrl;
        user.CoverImageUrl = request.CoverImageUrl ?? user.CoverImageUrl;
        user.PrimaryColor = request.PrimaryColor ?? user.PrimaryColor;
        user.Bio = request.Bio ?? user.Bio;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            user.Id,
            user.Name,
            user.Email,
            user.UserType,
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}