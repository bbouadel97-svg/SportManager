namespace SportManager.Models
{
    public class Poste
    {
        public static Poste Attaquant { get; } = new Poste { Nom = "Attaquant" };
        public int Id { get; set; }
        public required string Nom { get; set; }

        public int VitesseMin { get; set; }
        public int EnduranceMin { get; set; }
        public int ForceMin { get; set; }
        public int TechniqueMin { get; set; }

        public static implicit operator Poste(string v)
        {
            throw new NotImplementedException();
        }
    }
}