using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Agendamentos.Controllers.Base;
using Plataforma_Agendamentos.DTOs.Profile;
using Plataforma_Agendamentos.Services.Interfaces;

namespace Plataforma_Agendamentos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : BaseApiController
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService, ILogger<ProfileController> logger)
        : base(logger)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var profile = await _profileService.GetProfileAsync(userId.Value);

        if (profile == null)
            return NotFound(CreateErrorResponse("Usuario nao encontrado"));

        return Ok(CreateSuccessResponse(profile, "Perfil obtido com sucesso"));
    }

    [HttpGet("prestador/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPrestadorBySlug(string slug)
    {
        try
        {
            var prestador = await _profileService.GetPrestadorBySlugAsync(slug);
            
            if (prestador == null)
                return NotFound(CreateErrorResponse("Prestador nao encontrado"));

            return Ok(CreateSuccessResponse(prestador, "Perfil publico obtido com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar prestador por slug: {Slug}", slug);
            return StatusCode(500, CreateErrorResponse("Erro ao buscar perfil do prestador"));
        }
    }

    [HttpPut("cliente")]
    public async Task<IActionResult> UpdateClienteProfile([FromBody] UpdateClienteProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _profileService.UpdateClienteProfileAsync(userId.Value, request);
            LogBusinessOperation("CLIENTE_PROFILE_UPDATE", "User", userId);
            return Ok(CreateSuccessResponse(result, "Perfil de cliente atualizado com sucesso"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(CreateErrorResponse(ex.Message));
        }
    }

    [HttpPut("prestador")]
    public async Task<IActionResult> UpdatePrestadorProfile([FromBody] UpdatePrestadorProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _profileService.UpdatePrestadorProfileAsync(userId.Value, request);
            LogBusinessOperation("PRESTADOR_PROFILE_UPDATE", "User", userId, new { Slug = request.Slug });
            return Ok(CreateSuccessResponse(result, "Perfil de prestador atualizado com sucesso"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(CreateErrorResponse(ex.Message));
        }
    }
}
