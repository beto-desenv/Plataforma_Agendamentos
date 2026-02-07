using System.ComponentModel.DataAnnotations;

namespace Plataforma_Agendamentos.Validators;

/// <summary>
/// Atributo de validação para imagens em base64
/// </summary>
public class ValidBase64ImageAttribute : ValidationAttribute
{
    private readonly int _maxSizeKb;

    public ValidBase64ImageAttribute(int maxSizeKb = 500)
    {
        _maxSizeKb = maxSizeKb;
        ErrorMessage = $"A imagem não pode ter mais de {maxSizeKb}KB";
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return ValidationResult.Success!; // Permitir null/vazio (usar [Required] separadamente)

        var base64Image = value.ToString()!;

        // Verifica se é base64 válido
        if (!IsValidBase64(base64Image))
        {
            return new ValidationResult("Formato de imagem inválido. Use base64.");
        }

        // Obtém os dados base64 (sem o prefixo data:image...)
        var base64Data = GetBase64Data(base64Image);

        // Calcula o tamanho
        var sizeInBytes = (base64Data.Length * 3) / 4;
        var sizeInKb = sizeInBytes / 1024;

        if (sizeInKb > _maxSizeKb)
        {
            return new ValidationResult($"A imagem tem {sizeInKb}KB. O tamanho máximo permitido é {_maxSizeKb}KB. " +
                                       "Por favor, comprima ou redimensione a imagem.");
        }

        return ValidationResult.Success!;
    }

    private bool IsValidBase64(string base64String)
    {
        try
        {
            var data = GetBase64Data(base64String);
            Convert.FromBase64String(data);
            return true;
        }
        catch
        {
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
