using AutoRapide.Fichiers.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace AutoRapide.Fichiers.API.Controllers;

[Route("api/fichiers")]
[ApiController]
public class FileStorageController : ControllerBase
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
    /// <response code="500">Une erreur est survenue sur le serveur lors de l'enregistrement du fichier.</response>
    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload([FromBody] IFormFile file)
    {
        try
        {
            var fileName = await _storage.UploadAsync(file);
            if (string.IsNullOrEmpty(fileName))
            {
                _logger.LogInformation("Le fichier {Fichier} n'était pas formatté correctement et n'a pas pu être enregistré", file.FileName);
                return BadRequest();
            }
        
            _logger.LogInformation("Le fichier {Fichier} a été enregistré dans le service de stockage", fileName);
            
            return Ok(fileName);
        }
        catch (IOException ex)
        {
            _logger.LogError("{Message}\n{StackTrace}", ex.Message, ex.StackTrace);
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
    /// <response code="500">Une erreur est survenue sur le serveur lors de l'obtention du fichier.</response>
    [HttpGet]
    [Route("{fileName:required}")]
    public async Task<IActionResult> GetFile(string fileName)
    {
        try
        {
            var sanitizedFileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            var fileData = await _storage.DownloadAsync(sanitizedFileName);

            if (fileData is null)
            {
                _logger.LogInformation("Le fichier {Fichier} n'a pas été trouvé", sanitizedFileName);
                return NotFound();
            }

            new FileExtensionContentTypeProvider()
                .TryGetContentType(sanitizedFileName, out var contentType);

            if (contentType?.Split('/')[0] is not "image" or "audio" or "video")
            {
                _logger.LogInformation("Le fichier {Fichier} n'était pas du Content-Type attendu ({ContentType})", sanitizedFileName, contentType ?? "aucun");
                return NotFound();
            }
            
            _logger.LogInformation("Le fichier {Fichier} a été téléchargé depuis le service avec succès", sanitizedFileName);
        
            return File(fileData, contentType);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex.Message, ex.StackTrace);
            return Problem();
        }
    }
    
    /// <summary>
    /// Tente de supprimer un fichier du service de stockage selon le nom du fichier.
    /// </summary>
    /// <param name="fileName">Le nom du fichier a supprimer.</param>
    /// <returns>La réponse HTTP de l'action. (Voir les codes de réponse pour plus d'information).</returns>
    /// <response code="204">L'action de supprimer a bien été reçue.</response>
    /// <response code="404">Le fichier avec le nom spécifié n'a pas été trouvé.</response>
    /// <response code="500">Une erreur est survenue sur le serveur lors de la suppression du fichier.</response>
    [HttpDelete]
    [Route("supprimer/{fileName:required}")]
    public async Task<IActionResult> Delete(string fileName)
    {
        try
        {
            var sanitizedFileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            var fileDeleted = await _storage.DeleteAsync(sanitizedFileName);

            if (fileDeleted)
            {
                _logger.LogInformation("Le fichier {Fichier} a été supprimé du service de stockage", sanitizedFileName);
                return NoContent();
            }
            
            _logger.LogInformation("Le fichier {Fichier} n'a pas été trouvé", sanitizedFileName);
            return NotFound();
        }
        catch (IOException ex)
        {
            _logger.LogError(ex.Message, ex.StackTrace);
            return Problem();
        }
        
    }
}