namespace AutoRapide.MVC.Interfaces
{
    public interface IFichiersService
    {
        public Task<IEnumerable<string>> EnvoyerFichiers(string niv, IFormFileCollection fichiers);
        public Task SupprimerFichiers(string nomImage1, string nomImage2);
    }
}
