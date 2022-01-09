namespace AutoRapide.MVC.Services
{
    public interface IFavorisService
    {
        Task<IEnumerable<int>> ObtenirLesFavoris();
        Task<HttpResponseMessage> AjouterFavori(int idVehicule);
        Task<HttpResponseMessage> EffacerFavori(int idVehicule);
    }
}
