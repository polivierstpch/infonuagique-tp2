using AutoRapide.Fichiers.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoRapide.Fichiers.API.Controllers;

[Route("api/fichiers")]
[ApiController]
public class FileStorageController : ControllerBase
{
    private readonly IStorageService _storage;
    private readonly IFileValidationService _fileValidation;
    private readonly ILogger<FileStorageController> _logger;
    
    public FileStorageController(IStorageService storage, ILogger<FileStorageController> logger, IFileValidationService fileValidation)
    {
        _storage = storage;
        _logger = logger;
        _fileValidation = fileValidation;
    }

    /// <summary>
    /// Accepte des fichiers de formulaire et les enregistre dans le service.
    /// </summary>
    /// <param name="codeVehicule">Code du véhicule pour stocker les images.</param>
    /// <param name="fichiers">Fichiers obtenu via la requête HTTP.</param>
    /// <returns>Le nom des fichiers enregistrés dans le service.</returns>
    /// <response code="200">Le fichier a bien été enregistré et le nom de celui-ci est retourné.</response>
    /// <response code="400">Le fichier reçu en paramètre n'était pas valide.</response>
    /// <response code="500">Une erreur est survenue sur le serveur lors de l'enregistrement du fichier.</response>
    [HttpPost]
    [Route("upload")]
    public async Task<ActionResult<IEnumerable<string>>> Upload([FromForm] string codeVehicule, [FromForm] IFormFileCollection fichiers)
    {
        const int longueurCodeVehicule = 17;
        bool peutEnregistrer = fichiers is { Count: 2 } &&
                               !string.IsNullOrEmpty(codeVehicule) &&
                               codeVehicule.Length == longueurCodeVehicule;

        if (!peutEnregistrer)
        {
            _logger.LogInformation("La requête ne contenait pas un code véhicule et/ou des fichiers valides");
            return BadRequest("La requête n'était pas valide. Veuillez envoyer un NIV de 17 caractère et deux fichiers d'images.");
        }
        
        try
        {
            var nomFichiers = new List<string>();
            var tachesUpload = new List<Task>();
            
            for (int idx = 0; idx < fichiers.Count; idx += 1)
            {
                var fichier = fichiers[idx];
                _logger.LogInformation("le fichier {Fichier} va être validé...", fichier.FileName);
                
                if (!_fileValidation.IsValidFileType(fichier))
                {
                    _logger.LogInformation("Le fichier n'était pas formatté correctement et n'a pas pu être enregistré");
                    return BadRequest("Un des fichiers était invalide. (fichiers images seulement)");
                }
                
                var extension = Path.GetExtension(fichier.FileName);
                var nomFichier = $"{codeVehicule}_I{idx + 1}{extension}";
                nomFichiers.Add(nomFichier);
                tachesUpload.Add(_storage.UploadAsync(nomFichier, fichier));
                
                _logger.LogInformation("Sauvegarde du fichier {Fichier} débutée", nomFichier);
            }

            await Task.WhenAll(tachesUpload);
            
            _logger.LogInformation("Le fichiers {Fichier1} et {Fichier2} a été enregistré dans le service de stockage", 
                nomFichiers[0],  nomFichiers[1]);
            
            return CreatedAtAction(nameof(GetFile), new { fileName = nomFichiers[0] }, nomFichiers);
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

            if (fileData.Length == 0)
            {
                _logger.LogInformation("Le fichier {Fichier} n'a pas été trouvé", sanitizedFileName);
                return NotFound();
            }
            
            _logger.LogInformation("Le fichier {Fichier} a été téléchargé depuis le service avec succès", sanitizedFileName);

            var extension = Path.GetExtension(fileName).TrimStart('.');

            extension = extension == "jpg" ? "jpeg" : extension;
            
            return File(fileData, $"image/{extension}");
        }
        catch (IOException ex)
        {
            _logger.LogError("{Message}\n{StackTrace}", ex.Message, ex.StackTrace);
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
            _logger.LogError("{Message}\n{StackTrace}", ex.Message, ex.StackTrace);
            return Problem();
        }
        
    }
}