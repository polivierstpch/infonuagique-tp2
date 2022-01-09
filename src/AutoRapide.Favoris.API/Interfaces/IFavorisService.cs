namespace AutoRapide.Favoris.API.Interfaces
{
    public interface IFavorisService
    {
        IEnumerable<int> ObtenirLesFavoris();
        void AjouterFavori(int idVehicule);
        void EffacerFavori(int idVehicule);
    }
}
