namespace AutoRapide.Fichiers.API.Services;

public interface IStorageService
{
    Task<string> UploadAsync(IFormFile file);
    Task<byte[]> DownloadAsync(string fileName);
}