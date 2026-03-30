using System;

namespace SportManager.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int Equipe1Id { get; set; }
        public required Equipe Equipe1 { get; set; }

        public int Equipe2Id { get; set; }
        public required Equipe Equipe2 { get; set; }

        public int Score1 { get; set; }
        public int Score2 { get; set; }

        public DateTime Date { get; set; }
    }
}