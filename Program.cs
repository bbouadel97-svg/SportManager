using SportManager.Models;

var equipe = new Equipe { Nom = "PSG" };


var joueur = new Joueur
{
    Nom = "Mbappé",
    Age = 25,
    Vitesse = 95,
    Endurance = 90,
    Force = 85,
    Technique = 92,
    Equipe = equipe,
    Poste = Poste.Attaquant
};
Console.WriteLine($"{joueur.Nom} joue dans {equipe.Nom}");