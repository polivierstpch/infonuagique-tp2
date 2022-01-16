using AutoRapide.Fichiers.API.Interfaces;

namespace AutoRapide.Fichiers.API.Services;

public struct ValidationBytes
{
    public byte[] ValidHeaderBytes { get; }
    public byte[] ValidTrailerBytes { get; }

    public ValidationBytes(byte[] validHeaderBytes, byte[] validTrailerBytes)
    {
        ValidHeaderBytes = validHeaderBytes;
        ValidTrailerBytes = validTrailerBytes;
    }

    public void Deconstruct(out byte[] validHeaderBytes, out byte[] validTrailerBytes)
    {
        validHeaderBytes = ValidHeaderBytes;
        validTrailerBytes = ValidTrailerBytes;
    }
}


public class FileValidationService : IFileValidationService
{
    private const int ImageMinimumBytes = 512;
    
    private readonly IEnumerable<string> _acceptedMimeTypes;
    private readonly IDictionary<string, ValidationBytes> _validationBytes;

    public FileValidationService()
    {
        _acceptedMimeTypes = new List<string> {"image/jpeg", "image/png"};
        var jpegValidationBytes = new ValidationBytes(
            validHeaderBytes: new byte[] { 0xFF, 0xD8 },
            validTrailerBytes: new byte[] { 0xFF, 0xD9 }
        );
        var pngValidationBytes = new ValidationBytes(
            validHeaderBytes: new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
            validTrailerBytes: new byte[] { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 }
        );
        _validationBytes = new Dictionary<string, ValidationBytes>
        {
            {"image/jpeg", jpegValidationBytes},
            {"image/png", pngValidationBytes}
        };
    }

    public bool IsValidFileType(IFormFile file)
    {
        var contentType = GetContentType(file);

        if (file.Length < ImageMinimumBytes && !_acceptedMimeTypes.Contains(contentType))
            return false;

        return ValidateFileHeaderAndTrailer(file);
    }
    
    private bool ValidateFileHeaderAndTrailer(IFormFile file)
    {
        var headerBytes = GetFileHeader(file);
        var trailerBytes = GetFileTrailer(file);

        var contentType = GetContentType(file);
        if (!_validationBytes.TryGetValue(contentType, out var validationBytes)) 
            return false;
        
        var (validHeaderBytes, validTrailerBytes) = validationBytes;

        return IsSubsetOf(headerBytes, validHeaderBytes) &&
               IsSubsetOf(trailerBytes, validTrailerBytes);
    }

    private bool IsSubsetOf<T>(IEnumerable<T> set, IEnumerable<T> potentialSubset)
    {
        return potentialSubset.All(set.Contains);
    }

    private IEnumerable<byte> GetFileHeader(IFormFile file)
    {
        const int length = 8;
        using var fileStream = new BinaryReader(file.OpenReadStream());
        
        var bytes = new byte[length];
        fileStream.Read(bytes, 0, length);
        fileStream.BaseStream.Position = 0;
        
        return bytes;
    }

    private IEnumerable<byte> GetFileTrailer(IFormFile file)
    {
        const int length = 8;
        using var fileStream = new BinaryReader(file.OpenReadStream());

        fileStream.BaseStream.Position = fileStream.BaseStream.Length - length;
        var bytes = fileStream.ReadBytes(length);
        fileStream.BaseStream.Position = 0;
        
        return bytes;
    }

    private string GetContentType(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);

        if (string.IsNullOrEmpty(extension))
            return string.Empty;

        return extension switch
        {
            ".jpeg" or ".jpg" => "image/jpeg",
            ".png" => "image/png",
            _ => string.Empty
        };
    }
    
}