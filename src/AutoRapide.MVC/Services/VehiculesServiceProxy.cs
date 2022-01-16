using System.Text;
using AutoRapide.MVC.Interfaces;
using AutoRapide.MVC.Models;
using Newtonsoft.Json;

namespace AutoRapide.MVC.Services
{
    public class VehiculesServiceProxy : IVehiculesService
    {
        private const string RouteApi = "/api/vehicules/";
        private readonly HttpClient _httpClient;

        public VehiculesServiceProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Vehicule>> ObtenirToutAsync()
        {
            var response = await _httpClient.GetAsync(RouteApi);
            var content = await response.Content.ReadAsStringAsync();
            var vehicules = JsonConvert.DeserializeObject<IEnumerable<Vehicule>>(content);
            return vehicules;
        }

        public async Task<Vehicule> ObtenirParIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{RouteApi}{id}");
            var content = await response.Content.ReadAsStringAsync();
            var vehicule = JsonConvert.DeserializeObject<Vehicule>(content);
            return vehicule;
        }

        public async Task<HttpResponseMessage> AjouterAsync(Vehicule vehicule)
        {
            var content = new StringContent(JsonConvert.SerializeObject(vehicule), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync($"{RouteApi}enregistrer", content);
        }

        public async Task<HttpResponseMessage> ModifierAsync(Vehicule vehicule)
        {
            var content = new StringContent(JsonConvert.SerializeObject(vehicule), Encoding.UTF8, "application/json");
            return await _httpClient.PutAsync($"{RouteApi}modifier/{vehicule.Id}", content);
        }

        public async Task<HttpResponseMessage> SupprimerAsync(int id)
        {
            return await _httpClient.DeleteAsync($"{RouteApi}supprimer/{id}");
        }
    }
}
