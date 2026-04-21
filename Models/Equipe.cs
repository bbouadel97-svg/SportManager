using System.Collections.Generic;
using System.Linq;

namespace SportManager.Models
{
    public class Equipe
    {
        public int Id { get; set; }
        public required string Nom { get; set; }

        public List<Joueur> Joueurs { get; set; } = new List<Joueur>();

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public double TotalPotential => Joueurs?.Where(j => !j.EstBlesse).Sum(j => j.CalculatePotential()) ?? 0.0;
    }
}