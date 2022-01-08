using System.ComponentModel.DataAnnotations;

namespace AutoRapide.MVC.Models
{
    public class Usager : BaseEntity
    {
        public string Nom { get; set; } = "";
        public string Prenom { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Adresse { get; set; }
    }
}
