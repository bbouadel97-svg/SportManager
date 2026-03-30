using Microsoft.EntityFrameworkCore;
using SportManager.Models;

namespace SportManager.Data
{
    public class SportContext : DbContext
    {
        public DbSet<Joueur> Joueurs { get; set; }
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Poste> Postes { get; set; }
        public DbSet<Match> Matchs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=sportmanager.db");
        }
    }
}