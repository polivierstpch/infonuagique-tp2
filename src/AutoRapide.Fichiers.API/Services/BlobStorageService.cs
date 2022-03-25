using AutoRapide.Fichiers.API.Interfaces;
using Azure.Storage.Blobs;

namespace AutoRapide.Fichiers.API.Services;

public class BlobStorageService : IStorageService
{
    private readonly BlobContainerClient _blobContainerClient;
    
    public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration config)
    {
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(config.GetValue<string>("ImageContainer"));
    }
    
    public async Task UploadAsync(string nomFichier, IFormFile fichier)
    {
        var extension = Path.GetExtension(nomFichier);
        if (string.IsNullOrEmpty(extension))
            return;

        var blobClient = _blobContainerClient.GetBlobClient(nomFichier);
        
        await using var fileStream = fichier.OpenReadStream(); 
        await blobClient.UploadAsync(fileStream);
    }

    public async Task<byte[]> DownloadAsync(string fileName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(fileName);
        
        if (!await blobClient.ExistsAsync())
            return Array.Empty<byte>();

        var content = await blobClient.DownloadContentAsync();

        return content.Value.Content.ToArray();
    }

    public async Task<bool> DeleteAsync(string fileName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(fileName);
        
        return await blobClient.DeleteIfExistsAsync();
    }
}