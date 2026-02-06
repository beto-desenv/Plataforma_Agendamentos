using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs.Profile;
using Plataforma_Agendamentos.Models;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Service para gerenciar perfis de usuarios
/// </summary>
public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(AppDbContext context, ILogger<ProfileService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<object?> GetProfileAsync(Guid userId)
    {
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
            return null;

        if (user.IsCliente())
        {
            var perfil = user.ClientePerfil;
            return new
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
            };
        }
        else
        {
            var perfil = user.PrestadorPerfil;
            return new
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
            };
        }
    }

    public async Task<object> UpdateClienteProfileAsync(Guid userId, UpdateClienteProfileRequest request)
    {
        var user = await _context.Users
            .Include(u => u.ClientePerfil)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new InvalidOperationException("Usuario nao encontrado");

        if (!user.IsCliente())
            throw new InvalidOperationException("Usuario nao e um cliente");

        // Garantir que perfil existe (lazy creation)
        if (user.ClientePerfil == null)
        {
            user.ClientePerfil = new ClientePerfil { UserId = user.Id };
            _context.ClientePerfis.Add(user.ClientePerfil);
            _logger.LogInformation("Perfil de cliente criado automaticamente para usuario {UserId}", userId);
        }

        var perfil = user.ClientePerfil;

        // Atualizar campos fornecidos
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

        _logger.LogInformation("Perfil de cliente atualizado: {UserId}", userId);

        return new
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
        };
    }

    public async Task<object> UpdatePrestadorProfileAsync(Guid userId, UpdatePrestadorProfileRequest request)
    {
        var user = await _context.Users
            .Include(u => u.PrestadorPerfil)
                .ThenInclude(p => p!.Branding)
            .Include(u => u.PrestadorPerfil)
                .ThenInclude(p => p!.Metricas)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new InvalidOperationException("Usuario nao encontrado");

        if (!user.IsPrestador())
            throw new InvalidOperationException("Usuario nao e um prestador");

        // Garantir que perfil existe (lazy creation)
        if (user.PrestadorPerfil == null)
        {
            user.PrestadorPerfil = new PrestadorPerfil
            {
                UserId = user.Id,
                DisplayName = user.Name
            };
            _context.PrestadorPerfis.Add(user.PrestadorPerfil);
            _logger.LogInformation("Perfil de prestador criado automaticamente para usuario {UserId}", userId);
        }

        var perfil = user.PrestadorPerfil;

        // Atualizar campos fornecidos
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

        // Atualizar slug se necessario
        if (!string.IsNullOrEmpty(request.Slug))
        {
            var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

            // Verificar se slug esta disponivel
            if (normalizedSlug != perfil.Slug && await SlugExistsAsync(normalizedSlug, userId))
            {
                throw new InvalidOperationException("Slug ja esta em uso por outro prestador");
            }

            perfil.Slug = normalizedSlug;
        }
        else if (string.IsNullOrEmpty(perfil.Slug))
        {
            perfil.GerarSlug();
        }

        // Atualizar branding se fornecido
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

        _logger.LogInformation("Perfil de prestador atualizado: {UserId}, Slug: {Slug}", userId, perfil.Slug);

        return new
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
        };
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeUserId = null)
    {
        var normalizedSlug = slug.Trim().ToLowerInvariant();

        var query = _context.PrestadorPerfis.Where(p => p.Slug == normalizedSlug);

        if (excludeUserId.HasValue)
        {
            query = query.Where(p => p.UserId != excludeUserId.Value);
        }

        return await query.AnyAsync();
    }
}
