using AutoRapide.MVC.Interfaces;
using AutoRapide.MVC.Models;

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
            return await _httpClient.GetFromJsonAsync<IEnumerable<Vehicule>>(RouteApi);
        }

        public async Task<Vehicule> ObtenirParIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Vehicule>($"{RouteApi}{id}");
        }

        public async Task<HttpResponseMessage> AjouterAsync(Vehicule vehicule)
        {
            return await _httpClient.PostAsJsonAsync($"{RouteApi}enregistrer", vehicule);
        }

        public async Task<HttpResponseMessage> ModifierAsync(Vehicule vehicule)
        {
            return await _httpClient.PutAsJsonAsync($"{RouteApi}modifier/{vehicule.Id}", vehicule);
        }

        public async Task<HttpResponseMessage> SupprimerAsync(int id)
        {
            return await _httpClient.DeleteAsync($"{RouteApi}supprimer/{id}");
        }
    }
}
