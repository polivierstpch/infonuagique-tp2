using AutoRapide.MVC.Models;
using Newtonsoft.Json;
using System.Text;
using AutoRapide.MVC.Interfaces;

namespace AutoRapide.MVC.Services
{
    public class UtilisateursServiceProxy : IUsagerService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private const string _usagerApiUrl = "api/usager/";
        public UtilisateursServiceProxy(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<Usager> ObtenirUsagerParId(int id)
        {
            var content = await _httpClient.GetFromJsonAsync<Usager>(_usagerApiUrl + id);
            return content;
        }
        public async Task<Usager> ObtenirUsagerParCodeUsager(string code)
        {
            var reponse = await _httpClient.GetAsync(_usagerApiUrl + code);
            if (reponse.IsSuccessStatusCode)
            {
                var content = await reponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Usager>(content);
            }
            
            return null;
        }
        public async Task<IEnumerable<Usager>> ObtenirTousLesUsagers()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Usager>>(_usagerApiUrl);
        }
        public async Task<HttpResponseMessage> AjouterUsager(Usager usager) {
            StringContent content = new StringContent(JsonConvert.SerializeObject(usager), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(_usagerApiUrl, content);
        }
        public async Task<HttpResponseMessage> ModifierUsager(Usager usager) {
            StringContent content = new StringContent(JsonConvert.SerializeObject(usager), Encoding.UTF8, "application/json");
            return await _httpClient.PutAsync(_usagerApiUrl + usager.CodeUniqueUsager, content);
        }
        public async Task<HttpResponseMessage> EffacerUsager(string code) {
            return await _httpClient.DeleteAsync(_usagerApiUrl + code);
        }
    }
}
