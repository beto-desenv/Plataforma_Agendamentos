using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Controllers.Base;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs;
using Plataforma_Agendamentos.Models;
using System.Text.Json;

namespace Plataforma_Agendamentos.Controllers;

/// <summary>
/// Controller para gerenciar perfis de usuários (Prestador e Cliente)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProfilesController : BaseApiController
{
    private readonly AppDbContext _context;

    public ProfilesController(AppDbContext context, ILogger<ProfilesController> logger)
        : base(logger)
    {
        _context = context;
    }

    /// <summary>
    /// Atualizar perfil de Prestador
    /// </summary>
    [HttpPost("prestador/atualizar")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AtualizarPerfil([FromBody] PrestadorProfileUpdateRequest request)
    {
        var requestId = GetRequestId();
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Não autenticado",
                Detail = "Usuário não está autenticado",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        var validationError = ValidateModel();
        if (validationError != null)
            return validationError;

        _logger.LogInformation("Atualizando perfil do Prestador {UserId} - RequestId: {RequestId}", userId, requestId);

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.UserType == "prestador");

            if (user == null)
            {
                _logger.LogWarning("Usuário Prestador {UserId} não encontrado - RequestId: {RequestId}", userId, requestId);
                return NotFound(new ProblemDetails
                {
                    Title = "Usuário não encontrado",
                    Detail = "Usuário não existe ou não é um Prestador",
                    Status = StatusCodes.Status404NotFound
                });
            }

            // Atualizar campos
            user.TituloProfissional = request.TituloProfissional;
            user.AnosExperiencia = request.AnosExperiencia;
            user.Bio = request.Descricao;
            user.TelefonePrestador = request.Telefone;
            user.EstadoPrestador = request.Estado;
            user.CidadePrestador = request.Cidade;
            user.EnderecoPrestador = request.Endereco;
            user.RaioAtendimento = request.RaioAtendimento;
            user.FotoPerfilUrl = request.FotoUrl;

            // Atualizar documento CPF/CNPJ
            if (request.DocumentType == "cpf")
            {
                user.CPFPrestador = request.Document;
            }
            else if (request.DocumentType == "cnpj")
            {
                user.CNPJ = request.Document;
            }

            // Gerar slug
            user.DisplayName = request.TituloProfissional;
            user.GerarSlug();

            user.UpdatedAt = DateTime.UtcNow;

            // Adicionar/atualizar serviços
            if (request.Servicos != null && request.Servicos.Any())
            {
                // Remover serviços antigos
                var servicosAntigos = _context.Services.Where(s => s.UserId == user.Id).ToList();
                _context.Services.RemoveRange(servicosAntigos);

                // Adicionar novos serviços
                foreach (var servicoRequest in request.Servicos)
                {
                    var servico = new Service
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Nome = servicoRequest.Nome,
                        Preco = servicoRequest.Preco,
                        DurationMinutes = 60, // Padrão, pode passar na request depois
                        CriadoEm = DateTime.UtcNow,
                        AtualizadoEm = DateTime.UtcNow
                    };
                    _context.Services.Add(servico);
                }
            }

            await _context.SaveChangesAsync();

            LogBusinessOperation("PerfildoAtualizar", "User", user.Id, new { userType = user.UserType });

            _logger.LogInformation("Perfil do Prestador atualizado com sucesso - UserId: {UserId}, RequestId: {RequestId}", userId, requestId);

            return Ok(new
            {
                success = true,
                message = "Perfil atualizado com sucesso",
                data = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.TituloProfissional,
                    user.EstadoPrestador,
                    user.CidadePrestador,
                    user.TelefonePrestador
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar perfil do Prestador - RequestId: {RequestId}", requestId);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Erro ao atualizar perfil",
                Detail = "Ocorreu um erro ao atualizar seu perfil. Tente novamente.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Atualizar perfil de Cliente
    /// </summary>
    [HttpPost("cliente/atualizar")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AtualizarPerfilCliente([FromBody] ClienteProfileUpdateRequest request)
    {
        var requestId = GetRequestId();
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Não autenticado",
                Detail = "Usuário não está autenticado",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        var validationError = ValidateModel();
        if (validationError != null)
            return validationError;

        _logger.LogInformation("Atualizando perfil do Cliente {UserId} - RequestId: {RequestId}", userId, requestId);

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.UserType == "cliente");

            if (user == null)
            {
                _logger.LogWarning("Usuário Cliente {UserId} não encontrado - RequestId: {RequestId}", userId, requestId);
                return NotFound(new ProblemDetails
                {
                    Title = "Usuário não encontrado",
                    Detail = "Usuário não existe ou não é um Cliente",
                    Status = StatusCodes.Status404NotFound
                });
            }

            // Atualizar campos
            user.TelefoneCliente = request.Telefone;
            user.DataNascimento = request.DataNascimento;
            user.CPF = request.CPF;
            user.EstadoCliente = request.Estado;
            user.CidadeCliente = request.Cidade;
            user.EnderecoCliente = request.Endereco;
            user.ContatoPreferido = request.ContatoPreferido;
            user.Bio = request.Bio; // Reutilizar Bio para armazenar bio do cliente
            user.FotoPerfilUrl = request.FotoUrl;

            // Serializar interesses como JSON
            if (request.InteressesServicos != null && request.InteressesServicos.Any())
            {
                user.InteressesServicos = JsonSerializer.Serialize(request.InteressesServicos);
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            LogBusinessOperation("PerfilAtualizar", "User", user.Id, new { userType = user.UserType });

            _logger.LogInformation("Perfil do Cliente atualizado com sucesso - UserId: {UserId}, RequestId: {RequestId}", userId, requestId);

            return Ok(new
            {
                success = true,
                message = "Perfil atualizado com sucesso",
                data = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.EstadoCliente,
                    user.CidadeCliente,
                    user.TelefoneCliente,
                    user.ContatoPreferido
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar perfil do Cliente - RequestId: {RequestId}", requestId);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Erro ao atualizar perfil",
                Detail = "Ocorreu um erro ao atualizar seu perfil. Tente novamente.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Obter perfil do usuário autenticado
    /// </summary>
    [HttpGet("meu-perfil")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterMeuPerfil()
    {
        var usuario = GetCurrentUserId();
        if (usuario == null)
            return Unauthorized();

        var user = await _context.Users.Include(u => u.Services)
            .FirstOrDefaultAsync(u => u.Id == usuario);

        if (user == null)
            return NotFound();

        // Desserializar interesses se for cliente
        List<string>? interesses = null;
        if (!string.IsNullOrEmpty(user.InteressesServicos))
        {
            try
            {
                interesses = JsonSerializer.Deserialize<List<string>>(user.InteressesServicos);
            }
            catch { }
        }

        return Ok(new
        {
            id = user.Id,
            name = user.Name,
            email = user.Email,
            userType = user.UserType,
            // Cliente
            telefoneCliente = user.TelefoneCliente,
            dataNascimento = user.DataNascimento,
            cpf = user.CPF,
            estadoCliente = user.EstadoCliente,
            cidadeCliente = user.CidadeCliente,
            enderecoCliente = user.EnderecoCliente,
            contatoPreferido = user.ContatoPreferido,
            // Prestador
            tituloProfissional = user.TituloProfissional,
            anosExperiencia = user.AnosExperiencia,
            cpfPrestador = user.CPFPrestador,
            cnpj = user.CNPJ,
            estatePrestador = user.EstadoPrestador,
            cidadePrestador = user.CidadePrestador,
            enderecoPrestador = user.EnderecoPrestador,
            telefonePrestador = user.TelefonePrestador,
            raioAtendimento = user.RaioAtendimento,
            bio = user.Bio,
            fotoUrl = user.FotoPerfilUrl,
            slug = user.Slug,
            // Serviços
            servicos = user.Services?.Select(s => new { s.Id, s.Nome, s.Preco }).ToList()
        });
    }
}
