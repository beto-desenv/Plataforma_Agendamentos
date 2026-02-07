using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Agendamentos.Controllers.Base;
using Plataforma_Agendamentos.Services.Interfaces;

namespace Plataforma_Agendamentos.Controllers;

/// <summary>
/// Controller para validação de imagens
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImageController : BaseApiController
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService, ILogger<ImageController> logger)
        : base(logger)
    {
        _imageService = imageService;
    }

    /// <summary>
    /// Valida se uma imagem base64 está dentro do limite de tamanho
    /// </summary>
    [HttpPost("validate")]
    public IActionResult ValidateImage([FromBody] ValidateImageRequest request)
    {
        var isValid = _imageService.ValidateImageSize(request.Base64Image, request.MaxSizeKb ?? 500);

        if (!isValid)
        {
            var base64Data = request.Base64Image.Contains(",") 
                ? request.Base64Image.Split(',')[1] 
                : request.Base64Image;
            
            var sizeInBytes = (base64Data.Length * 3) / 4;
            var sizeInKb = sizeInBytes / 1024;

            return BadRequest(CreateErrorResponse(
                $"Imagem muito grande ({sizeInKb}KB). Tamanho máximo: {request.MaxSizeKb ?? 500}KB. " +
                "Comprima a imagem antes de enviar usando ferramentas como https://tinyjpg.com/"
            ));
        }

        return Ok(CreateSuccessResponse(new { valid = true }, "Imagem válida"));
    }

    /// <summary>
    /// Faz upload de uma imagem (salva como arquivo e retorna URL)
    /// </summary>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage([FromBody] UploadImageRequest request)
    {
        try
        {
            var imageUrl = await _imageService.UploadImageAsync(request.Base64Image, request.Folder ?? "images");
            return Ok(CreateSuccessResponse(new { url = imageUrl }, "Upload realizado com sucesso"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload da imagem");
            return BadRequest(CreateErrorResponse("Erro ao processar a imagem"));
        }
    }
}

public class ValidateImageRequest
{
    public string Base64Image { get; set; } = string.Empty;
    public int? MaxSizeKb { get; set; }
}

public class UploadImageRequest
{
    public string Base64Image { get; set; } = string.Empty;
    public string? Folder { get; set; }
}
