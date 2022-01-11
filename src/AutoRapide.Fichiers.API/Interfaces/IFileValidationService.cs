namespace AutoRapide.Fichiers.API.Interfaces;

public interface IFileValidationService
{
    bool IsValidFileType(IFormFile file);
}