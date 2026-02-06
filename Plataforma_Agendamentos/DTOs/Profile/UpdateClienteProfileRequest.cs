namespace Plataforma_Agendamentos.DTOs.Profile;

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
