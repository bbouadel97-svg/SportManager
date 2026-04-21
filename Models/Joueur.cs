using System.ComponentModel.DataAnnotations.Schema;

namespace SportManager.Models
{
    public class Joueur
    {
        public int Id { get; set; }
        public required string Nom { get; set; }
        public int Age { get; set; }

        public int Vitesse { get; set; }
        public int Endurance { get; set; }
        public int Force { get; set; }
        public int Technique { get; set; }

        public bool EstBlesse { get; set; } = false;

        // Relations - make nullable so a joueur can exist without assignment
        public int? EquipeId { get; set; }
        public Equipe? Equipe { get; set; }

        public int? PosteId { get; set; }
        public Poste? Poste { get; set; }

        [NotMapped]
        public double Potential => CalculatePotential();

        public double CalculatePotential()
        {
            // Weighted sum: technique matters most for collective sports in this model
            return Technique * 0.4 + Vitesse * 0.3 + Endurance * 0.2 + Force * 0.1;
        }
    }
}