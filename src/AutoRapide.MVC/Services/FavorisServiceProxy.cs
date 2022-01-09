using Newtonsoft.Json;
using System.Text;

namespace AutoRapide.MVC.Services
{
    public class FavorisServiceProxy : IFavorisService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private const string _favorisApiUrl = "api/Favoris/";
        public FavorisServiceProxy(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        public async Task<IEnumerable<int>> ObtenirLesFavoris() 
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<int>>(_favorisApiUrl);
        }
        public async Task<HttpResponseMessage> AjouterFavori(int idVehicule) 
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(idVehicule), Encoding.UTF8, "application/json");

            return await _httpClient.PostAsync(_favorisApiUrl, content);
        }
        public async Task<HttpResponseMessage> EffacerFavori(int idVehicule) 
        {
            return await _httpClient.DeleteAsync(_favorisApiUrl + idVehicule);
        }
    }
}
