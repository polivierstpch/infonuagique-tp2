using System.ComponentModel.DataAnnotations;

namespace AutoRapide.Vehicules.API.Entities;

public enum TypeVehicule
{
    Essence,
    Hybride
}

public class Vehicule
{
    [Key]
    public int Id { get; set; }

    public string Constructeur { get; set; }

    public string Modele { get; set; }
    
    public int AnneeFabrication { get; set; }
    
    public TypeVehicule Type { get; set; }

    public int NombreSiege { get; set; }

    public string Couleur { get; set; }

    public string NIV { get; set; }

    public string Image1Url { get; set; }

    public string Image2Url { get; set; }

    public string Description { get; set; }

    public bool EstDisponible { get; set; }

    public double Prix { get; set; }
}