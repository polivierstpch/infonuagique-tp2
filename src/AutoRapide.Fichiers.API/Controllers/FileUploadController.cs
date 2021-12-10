using AutoRapide.Fichiers.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace AutoRapide.Fichiers.API.Controllers;

[ApiController]
public class FileUploadController : Controller
{
    private readonly IStorageService _storage;
    private readonly ILogger _logger;

    public FileUploadController(IStorageService storage, ILogger<FileUploadController> logger)
    {
        _storage = storage;
        _logger = logger;
    }
    
    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            var fileName = await _storage.UploadAsync(file);

            if (string.IsNullOrEmpty(fileName))
                return BadRequest();
        
            return Ok(fileName);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex.Message, ex.StackTrace);
            return Problem();
        }
    }

    [HttpGet]
    [Route("file/{fileName:required}")]
    public async Task<IActionResult> GetFile(string fileName)
    {
        var sanitizedFileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
        
        var fileData = await _storage.DownloadAsync(sanitizedFileName);

        if (fileData is null)
            return NotFound();

        new FileExtensionContentTypeProvider()
            .TryGetContentType(sanitizedFileName, out var contentType);

        if (contentType?.Split('/')[0] is not "image" or "audio" or "video")
            return NotFound();
        
        return File(fileData, contentType);
    }
}