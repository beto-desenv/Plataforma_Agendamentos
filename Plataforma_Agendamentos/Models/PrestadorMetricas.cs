using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.Models;

/// <summary>
/// Metricas e estatisticas do prestador
/// </summary>
public class PrestadorMetricas
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid PrestadorPerfilId { get; set; }
    
    public decimal AvaliacaoMedia { get; set; } = 0;
    
    public int TotalAvaliacoes { get; set; } = 0;
    
    public int TotalServicos { get; set; } = 0;
    
    public int TotalAgendamentos { get; set; } = 0;
    
    public DateTime UltimaAtualizacao { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public PrestadorPerfil PrestadorPerfil { get; set; } = null!;
}
