using System.Collections.Generic;

namespace SportManager.Models
{
    public class Equipe
    {
        public int Id { get; set; }
        public required string Nom { get; set; }

        public List<Joueur> Joueurs { get; set; } = new List<Joueur>();
    }
}