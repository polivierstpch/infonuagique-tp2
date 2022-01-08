using AutoRapide.Vehicules.API.Entities;

namespace AutoRapide.Vehicules.API.Data;

public class InitialiseurBd
{
    public static void Initialiser(VehiculeContext contexte, string apiUrl)
    {
        contexte.Database.EnsureCreated();

        if (contexte.Vehicules.Any())
            return;

        var vehicules = new Vehicule[]
        {
            new Vehicule
            {
                Constructeur = "Ford",
                Modele = "Focus",
                AnneeFabrication = 2020,
                Couleur = "Argenté",
                Description = "Un véhicule utilitaire sport.",
                EstDisponible = true,
                Image1Url = $"{apiUrl}/files/ford1.png",
                Image2Url = "",
                NIV = "3FAHP0JG1BR151027",
                NombreSiege = 4,
                Type = TypeVehicule.Essence,
                Prix = 21000.00
            },
            new Vehicule
            {
                Constructeur = "Mazda",
                Modele = "CX-9 GT",
                AnneeFabrication = 2019,
                Couleur = "Argenté",
                Description = "Véhicule familial avec beaucoup d'espace.",
                EstDisponible = false,
                Image1Url = $"{apiUrl}/files/mazda1.png",
                Image2Url = "",
                NIV = "JM3TCBCY8K0318010",
                NombreSiege = 7,
                Type = TypeVehicule.Essence,
                Prix = 37519.00
            }
        };
        
        contexte.Vehicules.AddRange(vehicules);
        contexte.SaveChanges();
    }
    
}