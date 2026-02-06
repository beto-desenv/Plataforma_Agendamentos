using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.Models;

/// <summary>
/// Branding e aparencia visual do prestador
/// </summary>
public class PrestadorBranding
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid PrestadorPerfilId { get; set; }
    
    public string? LogoUrl { get; set; }
    
    public string? CoverImageUrl { get; set; }
    
    [MaxLength(7)]
    public string? PrimaryColor { get; set; } // #FF5733
    
    // Navigation
    public PrestadorPerfil PrestadorPerfil { get; set; } = null!;
}
