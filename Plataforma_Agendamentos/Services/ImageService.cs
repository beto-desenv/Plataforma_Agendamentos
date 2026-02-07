using Plataforma_Agendamentos.Services.Interfaces;

namespace Plataforma_Agendamentos.Services;

/// <summary>
/// Serviço básico para validação e gerenciamento de imagens
/// NOTA: Para redimensionamento, instale: dotnet add package SixLabors.ImageSharp
/// </summary>
public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImageService> _logger;
    private const int MAX_SIZE_KB = 500;

    public ImageService(IWebHostEnvironment environment, ILogger<ImageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public bool ValidateImageSize(string base64Image, int maxSizeKb = MAX_SIZE_KB)
    {
        try
        {
            var base64Data = GetBase64Data(base64Image);
            
            var sizeInBytes = (base64Data.Length * 3) / 4;
            var sizeInKb = sizeInBytes / 1024;
            
            _logger.LogInformation($"Tamanho da imagem: {sizeInKb}KB (limite: {maxSizeKb}KB)");
            
            return sizeInKb <= maxSizeKb;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar tamanho da imagem");
            return false;
        }
    }

    public async Task<string> UploadImageAsync(string base64Image, string folder = "images")
    {
        try
        {
            if (!ValidateImageSize(base64Image))
            {
                throw new InvalidOperationException(
                    "Imagem muito grande. Comprima a imagem antes de enviar. " +
                    "Tamanho máximo: 500KB. " +
                    "Dica: Use ferramentas como https://tinyjpg.com/ ou comprima no frontend."
                );
            }

            var fileName = $"{Guid.NewGuid()}.jpg";
            var url = await SaveBase64ImageAsync(base64Image, fileName, folder);
            
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload da imagem");
            throw;
        }
    }

    public Task<string> ResizeImageAsync(string base64Image, int maxWidth = 800, int maxHeight = 800)
    {
        // NOTA: Implementação completa requer SixLabors.ImageSharp
        // Para instalar: dotnet add package SixLabors.ImageSharp
        
        throw new NotImplementedException(
            "Redimensionamento de imagens requer o pacote SixLabors.ImageSharp. " +
            "Execute: dotnet add package SixLabors.ImageSharp " +
            "Por enquanto, comprima as imagens no frontend antes de enviar."
        );
    }

    public async Task<string> SaveBase64ImageAsync(string base64Image, string fileName, string folder = "images")
    {
        try
        {
            var base64Data = GetBase64Data(base64Image);
            var imageBytes = Convert.FromBase64String(base64Data);

            var uploadsPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", folder);
            Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, fileName);
            await File.WriteAllBytesAsync(filePath, imageBytes);

            var url = $"/{folder}/{fileName}";
            
            _logger.LogInformation($"Imagem salva: {url} ({imageBytes.Length / 1024}KB)");
            
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar imagem");
            throw new InvalidOperationException("Falha ao salvar a imagem", ex);
        }
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return false;

            var relativePath = imageUrl.TrimStart('/');
            var filePath = Path.Combine(_environment.WebRootPath ?? "wwwroot", relativePath);

            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
                _logger.LogInformation($"Imagem deletada: {imageUrl}");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao deletar imagem: {imageUrl}");
            return false;
        }
    }

    private string GetBase64Data(string base64Image)
    {
        if (base64Image.Contains(","))
        {
            return base64Image.Split(',')[1];
        }
        return base64Image;
    }
}
