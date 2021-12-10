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
    
    public async Task<string> UploadAsync(IFormFile file)
    {
        var fileExtension = Path.GetExtension(file.FileName);
        
        if (fileExtension == string.Empty)
            return string.Empty;
        
        var subFolder = GetFolderNameFromContentType(file.FileName);
        EnsureFolderExists(Path.Combine(RootDirectoryPath, subFolder));
        
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var newFilePath = Path.Combine(RootDirectoryPath, subFolder + " !", fileName);
        
        await using var writer = new FileStream(newFilePath, FileMode.Create);
        await file.CopyToAsync(writer);
        
        return fileName;
    }
    
    public async Task<byte[]> DownloadAsync(string fileName)
    {
        var folder = GetFolderNameFromContentType(fileName);

        var completePath = Path.Combine(RootDirectoryPath, folder, fileName);
        
        if (!File.Exists(completePath))
            return null;

        return await File.ReadAllBytesAsync(completePath);
    }

    private string GetFolderNameFromContentType(string fileName)
    {
        var typeFound = new FileExtensionContentTypeProvider()
            .TryGetContentType(fileName, out var contentType);
        
        if (!typeFound)
            return DefaultSubFolderName; 
        
        var mainType = contentType.Split('/')[0];

        return mainType switch
        {
            "image" => "images",
            "video" => "videos",
            "audio" => "audio",
            _ => DefaultSubFolderName
        };
    }
    private void EnsureFolderExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
    
}