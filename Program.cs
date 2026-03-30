using SportManager.Models;
using SportManager.Services;

var equipe1 = new Equipe { Nom = "PSG" };
var equipe2 = new Equipe { Nom = "OM" };

// Joueurs PSG
equipe1.Joueurs.Add(new Joueur { Nom = "Mbappé", Technique = 95, Vitesse = 98, Endurance = 85, Force = 75, Poste = Poste.Attaquant, Equipe = equipe1 });
equipe1.Joueurs.Add(new Joueur { Nom = "Dembele", Technique = 85, Vitesse = 92, Endurance = 80, Force = 70, Poste = Poste.Attaquant, Equipe = equipe1 });

// Joueurs OM
equipe2.Joueurs.Add(new Joueur { Nom = "Aubameyang", Technique = 88, Vitesse = 90, Endurance = 82, Force = 72, Poste = Poste.Attaquant, Equipe = equipe2 });
equipe2.Joueurs.Add(new Joueur { Nom = "Harit", Technique = 80, Vitesse = 85, Endurance = 78, Force = 68, Poste = Poste.Attaquant, Equipe = equipe2 });

var matchService = new MatchService();
var match = matchService.SimulerMatch(equipe1, equipe2);

Console.WriteLine($"\nRésultat : {match.Equipe1.Nom} {match.Score1} - {match.Score2} {match.Equipe2.Nom}");