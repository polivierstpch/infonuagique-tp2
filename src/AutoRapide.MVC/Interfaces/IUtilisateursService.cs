using AutoRapide.Utilisateurs.API.Entities;

namespace AutoRapide.MVC.Services
{
    public interface IUtilisateursService
    {
        public Task Creer(Utilisateur utilisateur);
        public Task<Utilisateur> ObtenirUtilisateur(int id);
        public Task Supprimer(Utilisateur utilisateur);

    }
}
