// New file: DbContext for EF Core
using Microsoft.EntityFrameworkCore;

namespace SportManager.Models
{
    public class SportContext : DbContext
    {
        public DbSet<Joueur> Joueurs { get; set; }
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Poste> Postes { get; set; }
        public DbSet<Match> Matches { get; set; }

        public SportContext()
        {
        }

        public SportContext(DbContextOptions<SportContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=sport.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipe>()
                .HasMany(e => e.Joueurs)
                .WithOne(j => j.Equipe)
                .HasForeignKey(j => j.EquipeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Poste>().HasIndex(p => p.Nom).IsUnique(false);

            // Configure Match relations
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Equipe1)
                .WithMany()
                .HasForeignKey(m => m.Equipe1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Equipe2)
                .WithMany()
                .HasForeignKey(m => m.Equipe2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

