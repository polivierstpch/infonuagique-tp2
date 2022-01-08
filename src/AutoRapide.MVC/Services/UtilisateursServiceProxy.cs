using AutoRapide.MVC.Models;

namespace AutoRapide.MVC.Services
{
    public class UtilisateursServiceProxy : IUsagerService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private const string _usagerApiUrl = "api/Usager/";
        public UtilisateursServiceProxy(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        Task<Usager> ObtenirUsagerParId(int id) { }
        Task<Usager> ObtenirUsagerParCourriel(string courriel) { }
        Task<IEnumerable<Usager>> ObtenirTousLesUsagers() { }
        Task AjouterUsager(Usager usager) { }
        Task ModifierUsager(Usager usager) { }
        Task EffacerUsager(int id) { }
    }
}
