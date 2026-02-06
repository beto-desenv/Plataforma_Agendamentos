using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Controllers.Base;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs;

namespace Plataforma_Agendamentos.Controllers;

/// <summary>
/// Controller para gerenciar perfis de usuarios (clientes e prestadores)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : BaseApiController
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context, ILogger<ProfileController> logger)
        : base(logger)
    {
        _context = context;
    }

    /// <summary>
    /// Obtem o perfil do usuario autenticado
    /// </summary>
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
            return NotFound(CreateErrorResponse("Usuario nao encontrado"));

        // Retornar dados conforme o tipo de usuario
        if (user.IsCliente())
        {
            return Ok(CreateSuccessResponse(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.UserType,
                user.CreatedAt,
                DadosCliente = new
                {
                    user.TelefoneCliente,
                    user.DataNascimento,
                    user.EnderecoCliente,
                    user.CPF,
                    user.PreferenciasNotificacao,
                    user.TotalAgendamentosCliente,
                    user.UltimoAgendamento
                }
            }, "Perfil do cliente obtido"));
        }
        else // Prestador
        {
            return Ok(CreateSuccessResponse(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.UserType,
                user.CreatedAt,
                DadosPrestador = new
                {
                    user.Slug,
                    user.DisplayName,
                    user.LogoUrl,
                    user.CoverImageUrl,
                    user.PrimaryColor,
                    user.Bio,
                    user.CNPJ,
                    user.TelefonePrestador,
                    user.EnderecoPrestador,
                    user.Site,
                    user.AvaliacaoMedia,
                    user.TotalAvaliacoes,
                    user.TotalServicos,
                    user.TotalAgendamentosPrestador,
                    user.AceitaAgendamentoImediato,
                    user.HorasAntecedenciaMinima,
                    user.PerfilAtivo,
                    user.HorarioInicioSemana,
                    user.HorarioFimSemana,
                    PublicUrl = user.GetPublicUrl(),
                    PerfilCompleto = user.TemPerfilCompleto()
                },
                Services = user.Services.Select(s => new
                {
                    s.Id,
                    s.Nome,
                    s.Description,
                    s.Preco,
                    s.DurationMinutes
                }),
                Schedules = user.Schedules.Select(sc => new
                {
                    sc.Id,
                    sc.DayOfWeek,
                    sc.StartTime,
                    sc.EndTime
                })
            }, "Perfil do prestador obtido"));
        }
    }

    /// <summary>
    /// Atualiza perfil de CLIENTE
    /// </summary>
    [HttpPut("cliente")]
    public async Task<IActionResult> UpdateClienteProfile([FromBody] UpdateClienteProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound(CreateErrorResponse("Usuario nao encontrado"));

        // Verificar se usuario e realmente um cliente
        if (!user.IsCliente())
            return BadRequest(CreateErrorResponse("Usuario nao e um cliente"));

        // Atualizar campos fornecidos
        if (!string.IsNullOrEmpty(request.Name))
            user.Name = request.Name.Trim();
        
        if (!string.IsNullOrEmpty(request.TelefoneCliente))
            user.TelefoneCliente = request.TelefoneCliente.Trim();
        
        if (request.DataNascimento.HasValue)
            user.DataNascimento = request.DataNascimento;
        
        if (!string.IsNullOrEmpty(request.EnderecoCliente))
            user.EnderecoCliente = request.EnderecoCliente.Trim();
        
        if (!string.IsNullOrEmpty(request.CPF))
            user.CPF = request.CPF.Trim();
        
        if (!string.IsNullOrEmpty(request.PreferenciasNotificacao))
            user.PreferenciasNotificacao = request.PreferenciasNotificacao.Trim();

        user.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        LogBusinessOperation("CLIENTE_PROFILE_UPDATE", "User", userId);

        return Ok(CreateSuccessResponse(new
        {
            user.Id,
            user.Name,
            user.Email,
            user.UserType,
            DadosCliente = new
            {
                user.TelefoneCliente,
                user.DataNascimento,
                user.EnderecoCliente,
                user.CPF,
                user.PreferenciasNotificacao,
                user.TotalAgendamentosCliente
            }
        }, "Perfil de cliente atualizado com sucesso"));
    }

    /// <summary>
    /// Atualiza perfil de PRESTADOR
    /// </summary>
    [HttpPut("prestador")]
    public async Task<IActionResult> UpdatePrestadorProfile([FromBody] UpdatePrestadorProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound(CreateErrorResponse("Usuario nao encontrado"));

        // Verificar se usuario e realmente um prestador
        if (!user.IsPrestador())
            return BadRequest(CreateErrorResponse("Usuario nao e um prestador"));

        // Atualizar campos fornecidos
        if (!string.IsNullOrEmpty(request.Name))
            user.Name = request.Name.Trim();
        
        if (!string.IsNullOrEmpty(request.DisplayName))
            user.DisplayName = request.DisplayName.Trim();
        
        if (!string.IsNullOrEmpty(request.Bio))
            user.Bio = request.Bio.Trim();
        
        if (!string.IsNullOrEmpty(request.TelefonePrestador))
            user.TelefonePrestador = request.TelefonePrestador.Trim();
        
        if (!string.IsNullOrEmpty(request.EnderecoPrestador))
            user.EnderecoPrestador = request.EnderecoPrestador.Trim();
        
        if (!string.IsNullOrEmpty(request.Site))
            user.Site = request.Site.Trim();
        
        if (!string.IsNullOrEmpty(request.PrimaryColor))
            user.PrimaryColor = request.PrimaryColor;
        
        if (!string.IsNullOrEmpty(request.LogoUrl))
            user.LogoUrl = request.LogoUrl;
        
        if (!string.IsNullOrEmpty(request.CoverImageUrl))
            user.CoverImageUrl = request.CoverImageUrl;
        
        if (!string.IsNullOrEmpty(request.CNPJ))
            user.CNPJ = request.CNPJ.Trim();
        
        if (request.AceitaAgendamentoImediato.HasValue)
            user.AceitaAgendamentoImediato = request.AceitaAgendamentoImediato.Value;
        
        if (request.HorasAntecedenciaMinima.HasValue)
            user.HorasAntecedenciaMinima = request.HorasAntecedenciaMinima.Value;
        
        if (request.PerfilAtivo.HasValue)
            user.PerfilAtivo = request.PerfilAtivo.Value;
        
        if (!string.IsNullOrEmpty(request.HorarioInicioSemana))
            user.HorarioInicioSemana = request.HorarioInicioSemana.Trim();
        
        if (!string.IsNullOrEmpty(request.HorarioFimSemana))
            user.HorarioFimSemana = request.HorarioFimSemana.Trim();

        // Atualizar slug se necessario
        if (!string.IsNullOrEmpty(request.Slug))
        {
            var normalizedSlug = request.Slug.Trim().ToLowerInvariant();
            
            // Verificar se slug nao esta em uso e e diferente do atual
            if (normalizedSlug != user.Slug && await _context.Users.AnyAsync(u => u.Slug == normalizedSlug && u.Id != userId))
            {
                return BadRequest(CreateErrorResponse("Slug ja esta em uso por outro prestador"));
            }
            
            user.Slug = normalizedSlug;
        }
        else if (string.IsNullOrEmpty(user.Slug))
        {
            // Gerar slug automaticamente se nao existir
            user.GerarSlug();
        }

        user.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        LogBusinessOperation("PRESTADOR_PROFILE_UPDATE", "User", userId, new { Slug = user.Slug });

        return Ok(CreateSuccessResponse(new
        {
            user.Id,
            user.Name,
            user.Email,
            user.UserType,
            DadosPrestador = new
            {
                user.Slug,
                user.DisplayName,
                user.Bio,
                user.TelefonePrestador,
                user.EnderecoPrestador,
                user.Site,
                user.PrimaryColor,
                user.LogoUrl,
                user.CoverImageUrl,
                user.CNPJ,
                user.AceitaAgendamentoImediato,
                user.HorasAntecedenciaMinima,
                user.PerfilAtivo,
                user.HorarioInicioSemana,
                user.HorarioFimSemana,
                PublicUrl = user.GetPublicUrl(),
                PerfilCompleto = user.TemPerfilCompleto(),
                user.AvaliacaoMedia,
                user.TotalServicos,
                user.TotalAgendamentosPrestador
            }
        }, "Perfil de prestador atualizado com sucesso"));
    }

    /// <summary>
    /// Endpoint legado - atualiza perfil de prestador (manter compatibilidade)
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound(CreateErrorResponse("Usuario nao encontrado"));

        // Se for prestador, usar campos de prestador
        if (user.IsPrestador())
        {
            var normalizedSlug = request.Slug?.Trim().ToLowerInvariant();

            // Combinar verificacao de slug nao vazio e diferente do atual
            if (!string.IsNullOrEmpty(normalizedSlug) && 
                normalizedSlug != user.Slug && 
                await _context.Users.AnyAsync(u => u.Slug == normalizedSlug && u.Id != userId))
            {
                return BadRequest(CreateErrorResponse("Slug ja esta em uso"));
            }

            user.Slug = normalizedSlug ?? user.Slug;
            user.DisplayName = request.DisplayName ?? user.DisplayName;
            user.LogoUrl = request.LogoUrl ?? user.LogoUrl;
            user.CoverImageUrl = request.CoverImageUrl ?? user.CoverImageUrl;
            user.PrimaryColor = request.PrimaryColor ?? user.PrimaryColor;
            user.Bio = request.Bio ?? user.Bio;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(CreateSuccessResponse(new
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
            }, "Perfil atualizado"));
        }

        return BadRequest(CreateErrorResponse("Use /api/profile/cliente ou /api/profile/prestador para atualizacao especifica"));
    }
}

// DTOs para atualizacao de perfil
public class UpdateClienteProfileRequest
{
    public string? Name { get; set; }
    public string? TelefoneCliente { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? EnderecoCliente { get; set; }
    public string? CPF { get; set; }
    public string? PreferenciasNotificacao { get; set; }
}

public class UpdatePrestadorProfileRequest
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Slug { get; set; }
    public string? Bio { get; set; }
    public string? TelefonePrestador { get; set; }
    public string? EnderecoPrestador { get; set; }
    public string? Site { get; set; }
    public string? PrimaryColor { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? CNPJ { get; set; }
    public bool? AceitaAgendamentoImediato { get; set; }
    public int? HorasAntecedenciaMinima { get; set; }
    public bool? PerfilAtivo { get; set; }
    public string? HorarioInicioSemana { get; set; }
    public string? HorarioFimSemana { get; set; }
}
