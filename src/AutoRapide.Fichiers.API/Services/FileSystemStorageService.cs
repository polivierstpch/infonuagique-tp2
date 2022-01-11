using AutoRapide.Fichiers.API.Interfaces;
using Microsoft.AspNetCore.StaticFiles;

namespace AutoRapide.Fichiers.API.Services;

public class FileSystemStorageService : IStorageService
{
    private const string DefaultFolderName = "AutoRapideFiles";
    private const string DefaultSubFolderName = "other";
    
    private readonly IConfiguration _config;
    private readonly string _defaultPath;
    
    private string RootDirectoryPath => _config.GetValue<string>("StoragePath") ?? _defaultPath;
    
    public FileSystemStorageService(IConfiguration config)
    {
        _config = config;
        _defaultPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            DefaultFolderName
        );
    }
    
    public async Task UploadAsync(string nomFichier, IFormFile fichier)
    {
        var extension = Path.GetExtension(nomFichier);
        if (string.IsNullOrEmpty(extension))
            return;
        
        EnsureFolderExists(RootDirectoryPath);
        
        var cheminFichier = Path.Combine(RootDirectoryPath, nomFichier);
        
        await using var writer = new FileStream(cheminFichier, FileMode.Create);
        await fichier.CopyToAsync(writer);
    }
    
    
    public async Task<byte[]> DownloadAsync(string fileName)
    {
        var completePath = Path.Combine(RootDirectoryPath, fileName);
        
        if (!File.Exists(completePath))
            return Array.Empty<byte>();

        return await File.ReadAllBytesAsync(completePath);
    }

    public async Task<bool> DeleteAsync(string fileName)
    {
        var completePath = Path.Combine(RootDirectoryPath, fileName);

        if (!File.Exists(completePath))
            return false;

        await using var fileStream = new FileStream(
            completePath,
            FileMode.Open,
            FileAccess.Read, 
            FileShare.None, 
            4096, 
            FileOptions.DeleteOnClose
        );
        await fileStream.FlushAsync();
        
        return true;
    }
    
    private static void EnsureFolderExists(string path)
    {
        Directory.CreateDirectory(path);
    }
    
}