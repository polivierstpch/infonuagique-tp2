using AutoRapide.MVC.Models;

namespace AutoRapide.MVC.Services
{
    public interface IUsagerService
    {
        Task<Usager> ObtenirUsagerParId(int id);
        Task<Usager> ObtenirUsagerParCourriel(string courriel);
        Task<IEnumerable<Usager>> ObtenirTousLesUsagers();
        Task AjouterUsager(Usager usager);
        Task ModifierUsager(Usager usager);
        Task EffacerUsager(int id);
    }
}
