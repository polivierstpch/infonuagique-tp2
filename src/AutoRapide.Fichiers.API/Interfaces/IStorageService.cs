namespace AutoRapide.Fichiers.API.Interfaces;

public interface IStorageService
{
    /// <summary>
    /// Sauvegarde un fichier dans le système de stockage.
    /// </summary>
    /// <param name="nomFichier">Nom du fichier pour le stockage.</param>
    /// <param name="fichier">Fichier de formulaire à stocker.</param>
    Task UploadAsync(string nomFichier, IFormFile fichier);
    
    /// <summary>
    /// Gets a file on the service by filename, returns the bytes of the file.
    /// </summary>
    /// <param name="fileName">Name of the file to get on the service.</param>
    /// <returns>Array of bytes constituting the file.</returns>
    Task<byte[]> DownloadAsync(string fileName);
    
    /// <summary>
    /// Deletes a file if it is found in the service. Returns
    /// indication that a file was deleted or not.
    /// </summary>
    /// <param name="fileName">Name of the file to delete on the service.</param>
    /// <returns>Wether or not the deletion of the file was successful.</returns>
    Task<bool> DeleteAsync(string fileName);
    
}