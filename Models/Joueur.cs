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

        // Relations
        public int EquipeId { get; set; }
        public required Equipe Equipe { get; set; }

        public int PosteId { get; set; }
        public required Poste Poste { get; set; }
    }
}