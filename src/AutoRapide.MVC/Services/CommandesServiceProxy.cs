using AutoRapide.MVC.Interfaces;
using AutoRapide.MVC.Models;
using Newtonsoft.Json;

namespace AutoRapide.MVC.Services
{
    public class CommandesServiceProxy : ICommandesService
    {
        private const string RouteApi = "/api/commandes/";
        private readonly HttpClient _httpClient;
        
        public CommandesServiceProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Commande> ObtenirParIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Commande>($"{RouteApi}{id}");
        }

        public async Task<IEnumerable<Commande>> ObtenirToutAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Commande>>(RouteApi);
        }

        public async Task<IEnumerable<Commande>> ObtenirToutPourUsagerAsync(int idUsager)
        { 
            var reponse = await _httpClient.GetAsync($"{RouteApi}usager/{idUsager}");
            if (reponse.IsSuccessStatusCode)
            {
                var content = await reponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Commande>>(content);
            }

            return new List<Commande>();
        }

        public async Task<HttpResponseMessage> AjouterAsync(Commande commande)
        {
            return await _httpClient.PostAsJsonAsync($"{RouteApi}enregistrer", commande);
        }

        public async Task<HttpResponseMessage> ModifierAsync(Commande commande)
        {
            return await _httpClient.PutAsJsonAsync($"{RouteApi}modifier/{commande.Id}", commande);
        }

        public async Task<HttpResponseMessage> SupprimerAsync(int id)
        {
            return await _httpClient.DeleteAsync($"{RouteApi}supprimer/{id}");
        }
    }
}
