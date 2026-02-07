namespace Plataforma_Agendamentos.DTOs.Profile;

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
    public string? FotoPerfilUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? PrimaryColor { get; set; }
}
