using System;
using System.Linq;
using SportManager.Models;

namespace SportManager.Services
{
    public class MatchService
    {
        private Random random = new Random();

        public Match SimulerMatch(Equipe equipe1, Equipe equipe2)
        {
            // Appliquer blessures avant match
            AppliquerBlessures(equipe1);
            AppliquerBlessures(equipe2);

            int score1 = CalculerScore(equipe1);
            int score2 = CalculerScore(equipe2);

            return new Match
            {
                Equipe1 = equipe1,
                Equipe2 = equipe2,
                Score1 = score1,
                Score2 = score2,
                Date = DateTime.Now
            };
        }

        private int CalculerScore(Equipe equipe)
        {
            var joueursValides = equipe.Joueurs.Where(j => !j.EstBlesse);

            if (!joueursValides.Any())
                return 0;

            int total = joueursValides.Sum(j =>
                j.Technique * 3 +   // plus important
                j.Vitesse * 2 +
                j.Endurance * 2 +
                j.Force
            );

            int moyenne = total / joueursValides.Count();

            int alea = random.Next(-10, 10);

            return Math.Max(0, (moyenne / 20) + alea);
        }

        private void AppliquerBlessures(Equipe equipe)
        {
            foreach (var joueur in equipe.Joueurs)
            {
                if (random.NextDouble() < 0.1) // 10%
                {
                    joueur.EstBlesse = true;
                    Console.WriteLine($"{joueur.Nom} est blessé !");
                }
            }
        }
    }
}