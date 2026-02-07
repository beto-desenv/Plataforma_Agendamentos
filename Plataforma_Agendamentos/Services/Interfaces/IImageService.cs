namespace Plataforma_Agendamentos.Services.Interfaces;

public interface IImageService
{
    /// <summary>
    /// Faz upload de uma imagem e retorna a URL
    /// </summary>
    Task<string> UploadImageAsync(string base64Image, string folder = "images");
    
    /// <summary>
    /// Valida se a imagem está dentro dos limites permitidos
    /// </summary>
    bool ValidateImageSize(string base64Image, int maxSizeKb = 500);
    
    /// <summary>
    /// Redimensiona uma imagem para um tamanho máximo
    /// </summary>
    Task<string> ResizeImageAsync(string base64Image, int maxWidth = 800, int maxHeight = 800);
    
    /// <summary>
    /// Deleta uma imagem
    /// </summary>
    Task<bool> DeleteImageAsync(string imageUrl);
    
    /// <summary>
    /// Converte base64 para arquivo e retorna o caminho
    /// </summary>
    Task<string> SaveBase64ImageAsync(string base64Image, string fileName, string folder = "images");
}
