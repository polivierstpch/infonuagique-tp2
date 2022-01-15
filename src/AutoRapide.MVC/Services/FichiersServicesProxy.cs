using System.Runtime.CompilerServices;
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
        
        public async Task<IEnumerable<string>> EnvoyerFichiers(string codeVehicule, IFormFileCollection fichiers)
        {
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(codeVehicule), "codeVehicule");

            foreach (var fichier in fichiers)
            {
                using var stream = fichier.OpenReadStream();
                formData.Add(new StreamContent(stream), "fichiers", fichier.FileName);
            }

            var response = await _httpClient.PostAsync(RouteApi + "upload", formData);

            response.EnsureSuccessStatusCode();

            var fileNames = await response.Content.ReadFromJsonAsync<IEnumerable<string>>();

            return fileNames;
        }

        public async Task SupprimerFichiers(params string[] urlsImage)
        { 
            var reponses = new List<Task<HttpResponseMessage>>(urlsImage.Length);

            var apiLinkToImages = $"{_httpClient.BaseAddress}{RouteApi}";
            
            foreach (var url in urlsImage)
            {
                var nomFichier = url.Replace(apiLinkToImages, "");
                reponses.Add(_httpClient.DeleteAsync($"{RouteApi}supprimer/{nomFichier}"));
            }

            await Task.WhenAll(reponses);
        }
    }
}
