using AutoRapide.MVC.Interfaces;

namespace AutoRapide.MVC.Services
{
    public class FichiersServicesProxy : IFichiersService
    {
        private const string RouteApi = "/api/fichiers/";
        private readonly HttpClient _httpClient;

        public FichiersServicesProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<IEnumerable<string>> EnvoyerFichiers(string codeVehicule, IEnumerable<IFormFile> fichiers)
        {
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(codeVehicule), "codeVehicule");

            foreach (var fichier in fichiers)
            {
                formData.Add(new StreamContent(fichier.OpenReadStream()), "fichiers", fichier.FileName);
            }
            var reponse = await _httpClient.PostAsync(RouteApi + "upload", formData);
            
            if (!reponse.IsSuccessStatusCode)
                return Array.Empty<string>();

            var nomFichier = await reponse.Content.ReadFromJsonAsync<IEnumerable<string>>();

            return nomFichier;
        }
    }
}
