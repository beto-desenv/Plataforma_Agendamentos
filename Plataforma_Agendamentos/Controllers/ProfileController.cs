using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Controllers.Base;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.Models;

namespace Plataforma_Agendamentos.Controllers;

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

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _context.Users
            .Include(u => u.ClientePerfil)
            .Include(u => u.PrestadorPerfil)
                .ThenInclude(p => p!.Branding)
            .Include(u => u.PrestadorPerfil)
                .ThenInclude(p => p!.Metricas)
            .Include(u => u.Services)
            .Include(u => u.Schedules)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound(CreateErrorResponse("Usuario nao encontrado"));

        if (user.IsCliente())
        {
            var perfil = user.ClientePerfil;
            return Ok(CreateSuccessResponse(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.UserType,
                user.CreatedAt,
                DadosCliente = perfil != null ? new
                {
                    perfil.Telefone,
                    perfil.DataNascimento,
                    perfil.Endereco,
                    perfil.Cidade,
                    perfil.Estado,
                    perfil.CEP,
                    perfil.CPF,
                    perfil.PreferenciasNotificacao,
                    perfil.TotalAgendamentos,
                    perfil.UltimoAgendamento
                } : null
            }, "Perfil do cliente obtido"));
        }
        else
        {
            var perfil = user.PrestadorPerfil;
            return Ok(CreateSuccessResponse(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.UserType,
                user.CreatedAt,
                DadosPrestador = perfil != null ? new
                {
                    perfil.Slug,
                    perfil.DisplayName,
                    perfil.TituloProfissional,
                    perfil.Bio,
                    perfil.CPF,
                    perfil.CNPJ,
                    perfil.AnosExperiencia,
                    perfil.Telefone,
                    perfil.Endereco,
                    perfil.Cidade,
                    perfil.Estado,
                    perfil.CEP,
                    perfil.Site,
                    perfil.RaioAtendimento,
                    perfil.AceitaAgendamentoImediato,
                    perfil.HorasAntecedenciaMinima,
                    perfil.HorarioInicioSemana,
                    perfil.HorarioFimSemana,
                    PublicUrl = perfil.GetPublicUrl(),
                    PerfilCompleto = perfil.TemPerfilCompleto(),
                    Branding = perfil.Branding != null ? new
                    {
                        perfil.Branding.LogoUrl,
                        perfil.Branding.CoverImageUrl,
                        perfil.Branding.PrimaryColor
                    } : null,
                    Metricas = perfil.Metricas != null ? new
                    {
                        perfil.Metricas.AvaliacaoMedia,
                        perfil.Metricas.TotalAvaliacoes,
                        perfil.Metricas.TotalServicos,
                        perfil.Metricas.TotalAgendamentos
                    } : null
                } : null,
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

    [HttpPut("cliente")]
    public async Task<IActionResult> UpdateClienteProfile([FromBody] UpdateClienteProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _context.Users
            .Include(u => u.ClientePerfil)
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        if (user == null)
            return NotFound(CreateErrorResponse("Usuario nao encontrado"));

        if (!user.IsCliente())
            return BadRequest(CreateErrorResponse("Usuario nao e um cliente"));

        if (user.ClientePerfil == null)
        {
            user.ClientePerfil = new ClientePerfil { UserId = user.Id };
            _context.ClientePerfis.Add(user.ClientePerfil);
        }

        var perfil = user.ClientePerfil;

        if (!string.IsNullOrEmpty(request.Name))
            user.Name = request.Name.Trim();
        
        if (!string.IsNullOrEmpty(request.Telefone))
            perfil.Telefone = request.Telefone.Trim();
        
        if (request.DataNascimento.HasValue)
            perfil.DataNascimento = request.DataNascimento;
        
        if (!string.IsNullOrEmpty(request.Endereco))
            perfil.Endereco = request.Endereco.Trim();
        
        if (!string.IsNullOrEmpty(request.Cidade))
            perfil.Cidade = request.Cidade.Trim();
        
        if (!string.IsNullOrEmpty(request.Estado))
            perfil.Estado = request.Estado.Trim();
        
        if (!string.IsNullOrEmpty(request.CEP))
            perfil.CEP = request.CEP.Trim();
        
        if (!string.IsNullOrEmpty(request.CPF))
            perfil.CPF = request.CPF.Trim();
        
        if (!string.IsNullOrEmpty(request.PreferenciasNotificacao))
            perfil.PreferenciasNotificacao = request.PreferenciasNotificacao.Trim();

        user.UpdatedAt = DateTime.UtcNow;
        perfil.UpdatedAt = DateTime.UtcNow;
        
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
                perfil.Telefone,
                perfil.DataNascimento,
                perfil.Endereco,
                perfil.Cidade,
                perfil.Estado,
                perfil.CEP,
                perfil.CPF,
                perfil.PreferenciasNotificacao,
                perfil.TotalAgendamentos
            }
        }, "Perfil de cliente atualizado com sucesso"));
    }

    [HttpPut("prestador")]
    public async Task<IActionResult> UpdatePrestadorProfile([FromBody] UpdatePrestadorProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _context.Users
            .Include(u => u.PrestadorPerfil)
                .ThenInclude(p => p!.Branding)
            .Include(u => u.PrestadorPerfil)
                .ThenInclude(p => p!.Metricas)
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        if (user == null)
            return NotFound(CreateErrorResponse("Usuario nao encontrado"));

        if (!user.IsPrestador())
            return BadRequest(CreateErrorResponse("Usuario nao e um prestador"));

        if (user.PrestadorPerfil == null)
        {
            user.PrestadorPerfil = new PrestadorPerfil 
            { 
                UserId = user.Id,
                DisplayName = user.Name
            };
            _context.PrestadorPerfis.Add(user.PrestadorPerfil);
        }

        var perfil = user.PrestadorPerfil;

        if (!string.IsNullOrEmpty(request.Name))
            user.Name = request.Name.Trim();
        
        if (!string.IsNullOrEmpty(request.DisplayName))
            perfil.DisplayName = request.DisplayName.Trim();
        
        if (!string.IsNullOrEmpty(request.TituloProfissional))
            perfil.TituloProfissional = request.TituloProfissional.Trim();
        
        if (!string.IsNullOrEmpty(request.Bio))
            perfil.Bio = request.Bio.Trim();
        
        if (!string.IsNullOrEmpty(request.Telefone))
            perfil.Telefone = request.Telefone.Trim();
        
        if (!string.IsNullOrEmpty(request.Endereco))
            perfil.Endereco = request.Endereco.Trim();
        
        if (!string.IsNullOrEmpty(request.Cidade))
            perfil.Cidade = request.Cidade.Trim();
        
        if (!string.IsNullOrEmpty(request.Estado))
            perfil.Estado = request.Estado.Trim();
        
        if (!string.IsNullOrEmpty(request.CEP))
            perfil.CEP = request.CEP.Trim();
        
        if (!string.IsNullOrEmpty(request.Site))
            perfil.Site = request.Site.Trim();
        
        if (!string.IsNullOrEmpty(request.CPF))
            perfil.CPF = request.CPF.Trim();
        
        if (!string.IsNullOrEmpty(request.CNPJ))
            perfil.CNPJ = request.CNPJ.Trim();
        
        if (request.AnosExperiencia.HasValue)
            perfil.AnosExperiencia = request.AnosExperiencia;
        
        if (request.RaioAtendimento.HasValue)
            perfil.RaioAtendimento = request.RaioAtendimento;
        
        if (request.AceitaAgendamentoImediato.HasValue)
            perfil.AceitaAgendamentoImediato = request.AceitaAgendamentoImediato.Value;
        
        if (request.HorasAntecedenciaMinima.HasValue)
            perfil.HorasAntecedenciaMinima = request.HorasAntecedenciaMinima.Value;
        
        if (!string.IsNullOrEmpty(request.HorarioInicioSemana))
            perfil.HorarioInicioSemana = request.HorarioInicioSemana.Trim();
        
        if (!string.IsNullOrEmpty(request.HorarioFimSemana))
            perfil.HorarioFimSemana = request.HorarioFimSemana.Trim();

        if (!string.IsNullOrEmpty(request.Slug))
        {
            var normalizedSlug = request.Slug.Trim().ToLowerInvariant();
            
            if (normalizedSlug != perfil.Slug && 
                await _context.PrestadorPerfis.AnyAsync(p => p.Slug == normalizedSlug && p.UserId != userId))
            {
                return BadRequest(CreateErrorResponse("Slug ja esta em uso por outro prestador"));
            }
            
            perfil.Slug = normalizedSlug;
        }
        else if (string.IsNullOrEmpty(perfil.Slug))
        {
            perfil.GerarSlug();
        }

        if (request.LogoUrl != null || request.CoverImageUrl != null || request.PrimaryColor != null)
        {
            if (perfil.Branding == null)
            {
                perfil.Branding = new PrestadorBranding { PrestadorPerfilId = perfil.Id };
                _context.PrestadorBrandings.Add(perfil.Branding);
            }

            if (request.LogoUrl != null)
                perfil.Branding.LogoUrl = request.LogoUrl;
            
            if (request.CoverImageUrl != null)
                perfil.Branding.CoverImageUrl = request.CoverImageUrl;
            
            if (request.PrimaryColor != null)
                perfil.Branding.PrimaryColor = request.PrimaryColor;
        }

        user.UpdatedAt = DateTime.UtcNow;
        perfil.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        LogBusinessOperation("PRESTADOR_PROFILE_UPDATE", "User", userId, new { Slug = perfil.Slug });

        return Ok(CreateSuccessResponse(new
        {
            user.Id,
            user.Name,
            user.Email,
            user.UserType,
            DadosPrestador = new
            {
                perfil.Slug,
                perfil.DisplayName,
                perfil.TituloProfissional,
                perfil.Bio,
                perfil.CPF,
                perfil.CNPJ,
                perfil.AnosExperiencia,
                perfil.Telefone,
                perfil.Endereco,
                perfil.Cidade,
                perfil.Estado,
                perfil.CEP,
                perfil.Site,
                perfil.RaioAtendimento,
                perfil.AceitaAgendamentoImediato,
                perfil.HorasAntecedenciaMinima,
                perfil.HorarioInicioSemana,
                perfil.HorarioFimSemana,
                PublicUrl = perfil.GetPublicUrl(),
                PerfilCompleto = perfil.TemPerfilCompleto(),
                Branding = perfil.Branding != null ? new
                {
                    perfil.Branding.LogoUrl,
                    perfil.Branding.CoverImageUrl,
                    perfil.Branding.PrimaryColor
                } : null,
                Metricas = perfil.Metricas != null ? new
                {
                    perfil.Metricas.AvaliacaoMedia,
                    perfil.Metricas.TotalServicos,
                    perfil.Metricas.TotalAgendamentos
                } : null
            }
        }, "Perfil de prestador atualizado com sucesso"));
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
