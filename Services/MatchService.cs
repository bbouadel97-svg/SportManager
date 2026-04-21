using System;
using System.Collections.Generic;
using System.Linq;
using SportManager.Models;

namespace SportManager.Services
{
    public class MatchService
    {
        private readonly SportContext _db;
        private readonly Random _rng;

        public MatchService(SportContext db, Random? rng = null)
        {
            _db = db;
            _rng = rng ?? new Random();
        }

        public Match SimulateAndSaveMatch(Equipe home, Equipe away, bool applyInjuries = true)
        {
            var homePot = home.Joueurs?.Where(j => !j.EstBlesse).Sum(j => j.CalculatePotential()) ?? 0.0;
            var awayPot = away.Joueurs?.Where(j => !j.EstBlesse).Sum(j => j.CalculatePotential()) ?? 0.0;
            var total = homePot + awayPot;
            if (total <= 0)
            {
                homePot = awayPot = 1;
                total = 2;
            }

            double homeExpected = (homePot / total) * 5.0;
            double awayExpected = (awayPot / total) * 5.0;

            int homeGoals = SamplePoisson(homeExpected);
            int awayGoals = SamplePoisson(awayExpected);

            // Apply injuries after match
            if (applyInjuries)
            {
                ApplyRandomInjuries(home);
                ApplyRandomInjuries(away);
            }

            var m = new Match
            {
                Equipe1Id = home.Id,
                Equipe2Id = away.Id,
                Score1 = homeGoals,
                Score2 = awayGoals,
                Date = DateTime.Now
            };

            _db.Matches.Add(m);
            _db.SaveChanges();

            return m;
        }

        private int SamplePoisson(double lambda)
        {
            if (lambda <= 0) return 0;
            double L = Math.Exp(-lambda);
            int k = 0;
            double p = 1.0;
            do
            {
                k++;
                p *= _rng.NextDouble();
            } while (p > L);
            return k - 1;
        }

        private void ApplyRandomInjuries(Equipe equipe)
        {
            foreach (var joueur in equipe.Joueurs)
            {
                if (joueur.EstBlesse) continue;
                double baseChance = 0.01; // 1%
                double enduranceFactor = (100 - joueur.Endurance) / 100.0;
                double chance = baseChance + enduranceFactor * 0.06; // up to ~7%
                if (_rng.NextDouble() < chance)
                {
                    joueur.EstBlesse = true;
                }
            }

            _db.SaveChanges();
        }
    }
}
