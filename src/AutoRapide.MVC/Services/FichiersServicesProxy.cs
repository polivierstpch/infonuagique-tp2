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

            var reponse = await _httpClient.PostAsync(RouteApi + "upload", formData);

            reponse.EnsureSuccessStatusCode();

            var nomFichier = await reponse.Content.ReadFromJsonAsync<IEnumerable<string>>();

            return nomFichier;
        }

        public async Task SupprimerFichiers(params string[] urlsImage)
        { 
            var reponses = new List<Task<HttpResponseMessage>>(urlsImage.Length);

            var lienApiImages = $"{_httpClient.BaseAddress}{RouteApi}";
            
            foreach (var url in urlsImage)
            {
                var nomFichier = url.Replace(lienApiImages, string.Empty);
                reponses.Add(_httpClient.DeleteAsync($"{RouteApi}supprimer/{nomFichier}"));
            }

            await Task.WhenAll(reponses);
        }
    }
}
