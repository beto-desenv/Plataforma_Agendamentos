using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs.Profile;
using Plataforma_Agendamentos.Models;
using Plataforma_Agendamentos.Services.Interfaces;

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
                user.FotoPerfilUrl,
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
                    perfil.ContatoPreferido,
                    perfil.InteressesServicos,
                    perfil.Bio,
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
                user.FotoPerfilUrl,
                user.CreatedAt,
                DadosPrestador = perfil != null ? new
                {
                    DocumentType = !string.IsNullOrEmpty(perfil.CNPJ) ? "cnpj" : "cpf",
                    Document = !string.IsNullOrEmpty(perfil.CNPJ) ? perfil.CNPJ : perfil.CPF,
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
                    FotoUrl = user.FotoPerfilUrl,
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

        if (!string.IsNullOrEmpty(request.FotoPerfilUrl))
        {
            var newPhotoUrl = request.FotoPerfilUrl.Trim();
            _logger.LogInformation("Atualizando foto do cliente {UserId} - Tamanho: {Size} chars", userId, newPhotoUrl.Length);
            user.FotoPerfilUrl = newPhotoUrl;
        }

        if (!string.IsNullOrEmpty(request.Telefone))
            perfil.Telefone = request.Telefone.Trim();

        if (request.DataNascimento.HasValue)
            perfil.DataNascimento = DateTime.SpecifyKind(request.DataNascimento.Value, DateTimeKind.Utc);

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

        if (!string.IsNullOrEmpty(request.ContatoPreferido))
            perfil.ContatoPreferido = request.ContatoPreferido.Trim();

        if (!string.IsNullOrEmpty(request.InteressesServicos))
            perfil.InteressesServicos = request.InteressesServicos.Trim();

        if (!string.IsNullOrEmpty(request.PreferenciasNotificacao))
            perfil.PreferenciasNotificacao = request.PreferenciasNotificacao.Trim();

        user.UpdatedAt = DateTime.UtcNow;
        perfil.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Perfil de cliente atualizado: {UserId} - FotoPerfilUrl: {HasPhoto}", 
            userId, !string.IsNullOrEmpty(user.FotoPerfilUrl));

        return new
        {
            user.Id,
            user.Name,
            user.Email,
            user.UserType,
            user.FotoPerfilUrl,
            DadosCliente = new
            {
                perfil.Telefone,
                perfil.DataNascimento,
                perfil.CPF,
                perfil.Endereco,
                perfil.Cidade,
                perfil.Estado,
                perfil.CEP,
                perfil.ContatoPreferido,
                perfil.InteressesServicos,
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
            .Include(u => u.Services)
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

        if (!string.IsNullOrEmpty(request.FotoPerfilUrl))
            user.FotoPerfilUrl = request.FotoPerfilUrl.Trim();

        if (!string.IsNullOrEmpty(request.DisplayName))
            perfil.DisplayName = request.DisplayName.Trim();

        if (!string.IsNullOrEmpty(request.TituloProfissional))
            perfil.TituloProfissional = request.TituloProfissional.Trim();

        // Mapear descricao ou bio para o campo Bio
        if (!string.IsNullOrEmpty(request.Descricao))
            perfil.Bio = request.Descricao.Trim();
        else if (!string.IsNullOrEmpty(request.Bio))
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

        // Mapear document para CPF ou CNPJ baseado no documentType
        if (!string.IsNullOrEmpty(request.Document))
        {
            if (request.DocumentType == "cpf")
                perfil.CPF = request.Document.Trim();
            else if (request.DocumentType == "cnpj")
                perfil.CNPJ = request.Document.Trim();
        }

        // Aceitar tanto CPF quanto CNPJ diretos
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

        // Processar serviços se fornecido
        if (request.Servicos != null && request.Servicos.Count > 0)
        {
            // Remover serviços antigos para este usuário
            var servicosAntigos = _context.Services.Where(s => s.UserId == userId).ToList();
            _context.Services.RemoveRange(servicosAntigos);

            // Adicionar novos serviços
            foreach (var servicoRequest in request.Servicos.Where(s => !string.IsNullOrEmpty(s.Nome)))
            {
                var novoServico = new Service
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Nome = servicoRequest.Nome!.Trim(),
                    Description = servicoRequest.Description?.Trim(),
                    Preco = servicoRequest.Preco ?? 0,
                    DurationMinutes = servicoRequest.DurationMinutes ?? 60,
                    CriadoEm = DateTime.UtcNow,
                    AtualizadoEm = DateTime.UtcNow
                };
                _context.Services.Add(novoServico);
            }

            _logger.LogInformation("Serviços atualizados para usuario {UserId}: {Count} servicos", userId, request.Servicos.Count);
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
            user.FotoPerfilUrl,
            DadosPrestador = new
            {
                DocumentType = !string.IsNullOrEmpty(perfil.CNPJ) ? "cnpj" : "cpf",
                Document = !string.IsNullOrEmpty(perfil.CNPJ) ? perfil.CNPJ : perfil.CPF,
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
                FotoUrl = user.FotoPerfilUrl,
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
                    TotalAvaliacoes = perfil.Metricas.TotalAvaliacoes ?? 0,
                    perfil.Metricas.TotalServicos,
                    perfil.Metricas.TotalAgendamentos
                } : null
            },
            Services = user.Services.Select(s => new
            {
                s.Id,
                s.Nome,
                s.Description,
                s.Preco,
                s.DurationMinutes
            })
        };
    }

    public async Task<object?> GetPrestadorBySlugAsync(string slug)
    {
        var prestador = await _context.PrestadorPerfis
            .Include(p => p.User)
            .Include(p => p.Branding)
            .Include(p => p.Metricas)
            .FirstOrDefaultAsync(p => p.Slug == slug.Trim().ToLowerInvariant());

        if (prestador == null)
            return null;

        // Buscar serviços do prestador
        var servicos = await _context.Services
            .Where(s => s.UserId == prestador.UserId)
            .Select(s => new
            {
                s.Id,
                s.Nome,
                Descricao = s.Description,
                s.Preco,
                s.DurationMinutes
            })
            .ToListAsync();

        return new
        {
            Slug = prestador.Slug,
            DisplayName = prestador.DisplayName,
            TituloProfissional = prestador.TituloProfissional,
            Bio = prestador.Bio,
            Descricao = prestador.Bio, // Alias para compatibilidade
            AnosExperiencia = prestador.AnosExperiencia,
            Telefone = prestador.Telefone,
            Endereco = prestador.Endereco,
            Cidade = prestador.Cidade,
            Estado = prestador.Estado,
            CEP = prestador.CEP,
            Site = prestador.Site,
            RaioAtendimento = prestador.RaioAtendimento,
            AceitaAgendamentoImediato = prestador.AceitaAgendamentoImediato,
            HorasAntecedenciaMinima = prestador.HorasAntecedenciaMinima,
            HorarioInicioSemana = prestador.HorarioInicioSemana,
            HorarioFimSemana = prestador.HorarioFimSemana,
            FotoUrl = prestador.User.FotoPerfilUrl,
            FotoPerfilUrl = prestador.User.FotoPerfilUrl,
            User = new
            {
                Name = prestador.User.Name,
                prestador.User.Email
            },
            Branding = prestador.Branding != null ? new
            {
                prestador.Branding.LogoUrl,
                prestador.Branding.CoverImageUrl,
                prestador.Branding.PrimaryColor
            } : null,
            Metricas = prestador.Metricas != null ? new
            {
                prestador.Metricas.AvaliacaoMedia,
                prestador.Metricas.TotalServicos,
                prestador.Metricas.TotalAgendamentos
            } : null,
            Servicos = servicos
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
