using AutoRapide.MVC.Models;

namespace AutoRapide.MVC.Services
{
    public interface IUsagerService
    {
        Task<Usager> ObtenirUsagerParId(int id);
        Task<Usager> ObtenirUsagerParCodeUsager(string code);
        Task<IEnumerable<Usager>> ObtenirTousLesUsagers();
        Task<HttpResponseMessage> AjouterUsager(Usager usager);
        Task<HttpResponseMessage> ModifierUsager(Usager usager);
        Task<HttpResponseMessage> EffacerUsager(int id);
    }
}
