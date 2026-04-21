using System.ComponentModel.DataAnnotations;

namespace SportManager.Models
{
    public class Poste
    {
        [Key]
        public int Id { get; set; }
        public required string Nom { get; set; }

        public int VitesseMin { get; set; }
        public int EnduranceMin { get; set; }
        public int ForceMin { get; set; }
        public int TechniqueMin { get; set; }

        // Common named roles for convenience when seeding
        public static Poste Gardien => new Poste { Nom = "Gardien", VitesseMin = 20, EnduranceMin = 30, ForceMin = 40, TechniqueMin = 30 };
        public static Poste Defense => new Poste { Nom = "Defense", VitesseMin = 40, EnduranceMin = 50, ForceMin = 60, TechniqueMin = 40 };
        public static Poste Milieu => new Poste { Nom = "Milieu", VitesseMin = 50, EnduranceMin = 60, ForceMin = 50, TechniqueMin = 60 };
        public static Poste Attaquant => new Poste { Nom = "Attaquant", VitesseMin = 60, EnduranceMin = 50, ForceMin = 50, TechniqueMin = 70 };
    }
}