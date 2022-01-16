using AutoRapide.Utilisateurs.API.Entities;

namespace AutoRapide.Utilisateurs.API.Data
{
    public class InitializeurBd
    {
        public static void Initialiser(UsagerContext contexte)
        {
            contexte.Database.EnsureCreated();

            if (contexte.Usagers.Any())
                return;

            var usagers = new Usager[]
            {
                new Usager
                {
                    Nom = "Lauzon",
                    Prenom = "Léopaul",
                    Email = "meurtrier@lamort.ca",
                    Adresse = "5150 Rue des Ormes"
                },
                new Usager
                {
                    Nom = "Arturo",
                    Prenom = "Lopez",
                    Email = "hola@lamuerte.mx"
                }
            };

            contexte.Usagers.AddRange(usagers);
            contexte.SaveChanges();
        }

    }
}