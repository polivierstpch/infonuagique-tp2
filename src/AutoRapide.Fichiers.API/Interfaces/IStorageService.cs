namespace AutoRapide.Fichiers.API.Interfaces;

public interface IStorageService
{
    /// <summary>
    /// Registers a file into the file storage service and returns back the new filename.
    /// </summary>
    /// <param name="file">Form file to register in the service.</param>
    /// <returns>The new filename of the newly registered file.</returns>
    Task<string> UploadAsync(IFormFile file);
    
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