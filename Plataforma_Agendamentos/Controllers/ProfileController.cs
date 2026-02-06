using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Agendamentos.Controllers.Base;
using Plataforma_Agendamentos.Services;

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

public class UpdateClienteProfileRequest
{
    public string? Name { get; set; }
    public string? Telefone { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? CEP { get; set; }
    public string? CPF { get; set; }
    public string? PreferenciasNotificacao { get; set; }
}

public class UpdatePrestadorProfileRequest
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Slug { get; set; }
    public string? TituloProfissional { get; set; }
    public string? Bio { get; set; }
    public string? CPF { get; set; }
    public string? CNPJ { get; set; }
    public int? AnosExperiencia { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? CEP { get; set; }
    public string? Site { get; set; }
    public int? RaioAtendimento { get; set; }
    public bool? AceitaAgendamentoImediato { get; set; }
    public int? HorasAntecedenciaMinima { get; set; }
    public string? HorarioInicioSemana { get; set; }
    public string? HorarioFimSemana { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? PrimaryColor { get; set; }
}
