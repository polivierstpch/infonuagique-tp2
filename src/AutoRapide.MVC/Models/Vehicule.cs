using System.ComponentModel.DataAnnotations;

namespace AutoRapide.MVC.Models
{
    public enum TypeVehicule
    {
        Essence,
        Hybride
    }
    
    public class Vehicule : BaseEntity
    {
         
        [Required(ErrorMessage = "Ce champ est requis.")]
        public string Constructeur { get; set; }
    
        [Required(ErrorMessage = "Ce champ est requis.")]
        public string Modele { get; set; }
    
        [Required(ErrorMessage = "Ce champ est requis.")]
        [Range(1900, 9999, ErrorMessage = "Veuillez fournir une date de fabrication valide (1900 à 9999).")]
        public int AnneeFabrication { get; set; }
    
        [Required(ErrorMessage = "Ce champ est requis.")]
        public TypeVehicule Type { get; set; }
    
        [Required(ErrorMessage = "Ce champ est requis.")]
        [Range(1, 99, ErrorMessage = "Veuillez fournir un nombre de siège(s) valide (1 à 99).")]
        public int NombreSiege { get; set; }
    
        [Required(ErrorMessage = "Ce champ est requis.")]
        public string Couleur { get; set; }

        public string NIV => ConstruireNIV();
    
        [Required(ErrorMessage = "Ce champ est requis.")]
        public string Image1Url { get; set; }

        public string Image2Url { get; set; }
    
        public string Description { get; set; }
    
        [Display(Name ="Disponibilité")]
        public bool EstDisponible { get; set; }
    
        [Required(ErrorMessage = "Ce champ est requis.")]
        [Range(1, 9_999_999.99, ErrorMessage = "Veuillez fournir un prix valide. (1,00 à 9,999,999.99)")]
        public double Prix { get; set; }

        private string ConstruireNIV()
        {
            string codeConstructeur = Constructeur[0..3];
            string anneeRaccourcie = AnneeFabrication.ToString().Substring(2, 2);
            string codeAnneeModel = $"{Modele[0..2]}{anneeRaccourcie}";
            string typeVehicule = ((int)Type).ToString("D2");
            string codeSiege = NombreSiege.ToString("D2");
            string numero = Id.ToString("D6");
            return $"{codeConstructeur}{codeSiege}{typeVehicule}{codeAnneeModel}{numero}".ToUpper();
        }
    }
}
