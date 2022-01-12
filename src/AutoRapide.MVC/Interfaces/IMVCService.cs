using AutoRapide.Vehicules.API.Entities;
using System.Linq.Expressions;

namespace AutoRapide.MVC.Services
{
    public interface IMVCService
    {
        //Afficher liste complete
        Task<IEnumerable<Vehicule>> Liste();

        //Afficher liste filtree
        Task<IEnumerable<Vehicule>> Liste(Expression<Func<Vehicule, bool>> predicate);

        //Afficher details selon annonce selectionnee (id)
        Task<Vehicule> ObtenirParId(int id);
    }

    //public interface IMVCService<T>/* where T : */
    //{
    //    //Afficher liste complete
    //    Task<IEnumerable<T>> Liste();

    //    //Afficher liste filtree
    //    Task<IEnumerable<T>> Liste(Expression<Func<T, bool>> predicate);

    //    //Afficher details selon annonce selectionnee (id)
    //    Task<T> ObtenirParId(int id);
    //}
}
