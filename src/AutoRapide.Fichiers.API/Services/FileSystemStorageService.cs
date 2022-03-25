using AutoRapide.Fichiers.API.Interfaces;
using Microsoft.AspNetCore.StaticFiles;

namespace AutoRapide.Fichiers.API.Services;

public class FileSystemStorageService : IStorageService
{
    private const string DefaultFolderName = "AutoRapideFiles";

    private readonly string _fileFolderPath;
    
    public FileSystemStorageService()
    {
        _fileFolderPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            DefaultFolderName
        );
    }
    
    public async Task UploadAsync(string nomFichier, IFormFile fichier)
    {
        var extension = Path.GetExtension(nomFichier);
        if (string.IsNullOrEmpty(extension))
            return;
        
        EnsureFolderExists(_fileFolderPath);
        
        var cheminFichier = Path.Combine(_fileFolderPath, nomFichier);
        
        await using var writer = new FileStream(cheminFichier, FileMode.Create);
        await fichier.CopyToAsync(writer);
    }
    
    public async Task<byte[]> DownloadAsync(string fileName)
    {
        var completePath = Path.Combine(_fileFolderPath, fileName);
        
        if (!File.Exists(completePath))
            return Array.Empty<byte>();

        return await File.ReadAllBytesAsync(completePath);
    }

    public async Task<bool> DeleteAsync(string fileName)
    {
        var completePath = Path.Combine(_fileFolderPath, fileName);

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