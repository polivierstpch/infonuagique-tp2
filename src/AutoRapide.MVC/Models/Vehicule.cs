namespace AutoRapide.MVC.Models
{
    public class Vehicule
    {
        //[Autoincrement]
        public int ID { get; set; }

        public string Marque { get; set; }

        public string Modele { get; set; }

        public int Annee { get; set; }

        public string Carburant { get; set; } 

        public int Places { get; set; }

        public string Couleur { get; set; }

        public string NIV 
        { get 
            {
                string niv = "";
                string marque = Marque; //
                string codeMarque = marque.Substring(0, 3);
                string numero = ID.ToString("D8");
                if (string.IsNullOrEmpty(codeMarque)) 
                { 

                }
                else 
                { 
                    niv = codeMarque;
                    niv += numero;
                }

                return niv;
            } 
        }

        public string Description { get; set; }

        public double Prix { get; set; }

        public bool Disponible { get; set; } = true;
    }
}
