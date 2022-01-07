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
    
    [Required]
    public string Constructeur { get; set; }
    
    [Required]
    public string Modele { get; set; }
    
    [Required]
    [Range(1900, 9999)]
    public int AnneeFabrication { get; set; }
    
    [Required]
    public TypeVehicule Type { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int NombreSiege { get; set; }
    
    [Required]
    public string Couleur { get; set; }
    
    [Required]
    [RegularExpression(@"^[A-Z0-9]{17}$")]
    public string NIV { get; set; }
    
    [Required]
    public string Image1Url { get; set; }

    public string Image2Url { get; set; }
    
    public string Description { get; set; }
    
    public bool EstDisponible { get; set; }
    
    [Required]
    [Range(1, double.MaxValue)]
    public double Prix { get; set; }
}