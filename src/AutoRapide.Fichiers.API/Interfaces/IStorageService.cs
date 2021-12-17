namespace AutoRapide.Fichiers.API.Interfaces;

public interface IStorageService
{
    Task<string> UploadAsync(IFormFile file);
    Task<byte[]> DownloadAsync(string fileName);
    Task<bool> DeleteAsync(string fileName);
}