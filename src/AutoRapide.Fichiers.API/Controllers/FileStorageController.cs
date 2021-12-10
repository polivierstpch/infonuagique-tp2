using AutoRapide.Fichiers.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace AutoRapide.Fichiers.API.Controllers;

[ApiController]
[Route("api")]
public class FileStorageController : Controller
{
    private readonly IStorageService _storage;
    private readonly ILogger _logger;
    
    public FileStorageController(IStorageService storage, ILogger<FileStorageController> logger)
    {
        _storage = storage;
        _logger = logger;
    }
    
    /// <summary>
    /// Accepte un fichier de formulaire et l'enregistre dans le service.
    /// </summary>
    /// <param name="file">Fichier obtenu via la requête HTTP.</param>
    /// <returns>Le nom du fichier enregistré dans le service.</returns>
    /// <response code="200">Le fichier a bien été enregistré et le nom de celui-ci est retourné.</response>
    /// <response code="400">Le fichier reçu en paramètre n'était pas valide.</response>
    /// <response code="500">Une erreur est survenue lors de l'enregistrement du fichier.</response>
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
    
    /// <summary>
    /// Retourne le fichier multimédia demandé. (seulement les images, vidéos et audio sont retournés)
    /// </summary>
    /// <param name="fileName">Le nom du fichier demandé.</param>
    /// <returns>Retourne le fichier demandé s'il existe.</returns>
    /// <response code="200">Le fichier est retourné avec la réponse.</response>
    /// <response code="404">Le fichier avec le nom spécifié n'a pas été trouvé ou celui-ci n'est pas un fichier multimédia.</response>
    /// <response code="500">Une erreur est survenue lors de l'obtention du fichier.</response>
    [HttpGet]
    [Route("file/{fileName:required}")]
    public async Task<IActionResult> GetFile(string fileName)
    {
        try
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
        catch (IOException ex)
        {
            _logger.LogError(ex.Message, ex.StackTrace);
            return Problem();
        }
    }
}