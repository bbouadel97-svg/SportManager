using Microsoft.Maui.Controls;
using SportManager.Models;
using SportManager.Services;
using Microsoft.EntityFrameworkCore;

namespace SportManager.Maui
{
    public partial class MainPage : ContentPage
    {
        private readonly SportContext _db;
        private readonly MatchService _matchService;

        public MainPage(SportContext db, MatchService matchService)
        {
            InitializeComponent();
            _db = db;
            _matchService = matchService;

            BtnCreateTeam.Clicked += async (s, e) => await OnCreateTeam();
            BtnCreatePlayer.Clicked += async (s, e) => await OnCreatePlayer();
            BtnRefresh.Clicked += async (s, e) => await LoadData();
            BtnSimulate.Clicked += async (s, e) => await OnSimulate();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }

        private async Task LoadData()
        {
            var teams = await _db.Equipes.Include(t => t.Joueurs).ToListAsync();
            TeamsListView.ItemsSource = teams;

            var players = await _db.Joueurs.Include(p => p.Equipe).ToListAsync();
            PlayersListView.ItemsSource = players;

            PickerHome.ItemsSource = teams;
            PickerHome.ItemDisplayBinding = new Binding("Nom");
            PickerAway.ItemsSource = teams;
            PickerAway.ItemDisplayBinding = new Binding("Nom");
        }

        private async Task OnCreateTeam()
        {
            var name = await DisplayPromptAsync("Create Team", "Team name:");
            if (string.IsNullOrWhiteSpace(name)) return;
            _db.Equipes.Add(new Equipe { Nom = name });
            await _db.SaveChangesAsync();
            await LoadData();
        }

        private async Task OnEditTeam(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is int id)
            {
                var team = await _db.Equipes.FindAsync(id);
                if (team == null) return;
                var name = await DisplayPromptAsync("Edit Team", "Team name:", initialValue: team.Nom);
                if (string.IsNullOrWhiteSpace(name)) return;
                team.Nom = name;
                await _db.SaveChangesAsync();
                await LoadData();
            }
        }

        private async Task OnDeleteTeam(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is int id)
            {
                var confirm = await DisplayAlert("Delete", "Are you sure?", "Yes", "No");
                if (!confirm) return;
                var team = await _db.Equipes.Include(t => t.Joueurs).FirstOrDefaultAsync(t => t.Id == id);
                if (team == null) return;
                // Unassign players
                foreach (var p in team.Joueurs) p.EquipeId = null;
                _db.Equipes.Remove(team);
                await _db.SaveChangesAsync();
                await LoadData();
            }
        }

        private async Task OnCreatePlayer()
        {
            var nom = await DisplayPromptAsync("Create Player", "Name:");
            if (string.IsNullOrWhiteSpace(nom)) return;
            var ageStr = await DisplayPromptAsync("Create Player", "Age:", initialValue: "18");
            int.TryParse(ageStr, out var age);
            var vStr = await DisplayPromptAsync("Create Player", "Vitesse (0-100):", initialValue: "50"); int.TryParse(vStr, out var v);
            var eStr = await DisplayPromptAsync("Create Player", "Endurance (0-100):", initialValue: "50"); int.TryParse(eStr, out var en);
            var fStr = await DisplayPromptAsync("Create Player", "Force (0-100):", initialValue: "50"); int.TryParse(fStr, out var fo);
            var tStr = await DisplayPromptAsync("Create Player", "Technique (0-100):", initialValue: "50"); int.TryParse(tStr, out var te);

            var teams = await _db.Equipes.ToListAsync();
            int? equipeId = null;
            if (teams.Count > 0)
            {
                var choices = teams.Select(t => t.Nom).ToArray();
                var pick = await DisplayActionSheet("Choose team", "Cancel", null, choices);
                if (pick != null && pick != "Cancel") equipeId = teams.FirstOrDefault(t => t.Nom == pick)?.Id;
            }

            var player = new Joueur { Nom = nom, Age = age, Vitesse = v, Endurance = en, Force = fo, Technique = te, EquipeId = equipeId };
            _db.Joueurs.Add(player);
            await _db.SaveChangesAsync();
            await LoadData();
        }

        private async Task OnEditPlayer(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is int id)
            {
                var p = await _db.Joueurs.FindAsync(id);
                if (p == null) return;
                var nom = await DisplayPromptAsync("Edit Player", "Name:", initialValue: p.Nom);
                if (!string.IsNullOrWhiteSpace(nom)) p.Nom = nom;
                var ageStr = await DisplayPromptAsync("Edit Player", "Age:", initialValue: p.Age.ToString()); if (int.TryParse(ageStr, out var age)) p.Age = age;
                var vStr = await DisplayPromptAsync("Edit Player", "Vitesse:", initialValue: p.Vitesse.ToString()); if (int.TryParse(vStr, out var v)) p.Vitesse = v;
                var eStr = await DisplayPromptAsync("Edit Player", "Endurance:", initialValue: p.Endurance.ToString()); if (int.TryParse(eStr, out var en)) p.Endurance = en;
                var fStr = await DisplayPromptAsync("Edit Player", "Force:", initialValue: p.Force.ToString()); if (int.TryParse(fStr, out var fo)) p.Force = fo;
                var tStr = await DisplayPromptAsync("Edit Player", "Technique:", initialValue: p.Technique.ToString()); if (int.TryParse(tStr, out var te)) p.Technique = te;

                var teams = await _db.Equipes.ToListAsync();
                if (teams.Count > 0)
                {
                    var choices = teams.Select(t => t.Nom).ToArray();
                    var pick = await DisplayActionSheet("Choose team", "Keep", null, choices);
                    if (pick != null && pick != "Keep") p.EquipeId = teams.FirstOrDefault(t => t.Nom == pick)?.Id;
                }

                await _db.SaveChangesAsync();
                await LoadData();
            }
        }

        private async Task OnDeletePlayer(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is int id)
            {
                var confirm = await DisplayAlert("Delete", "Delete player?", "Yes", "No");
                if (!confirm) return;
                var p = await _db.Joueurs.FindAsync(id);
                if (p == null) return;
                _db.Joueurs.Remove(p);
                await _db.SaveChangesAsync();
                await LoadData();
            }
        }

        private async Task OnSimulate()
        {
            if (PickerHome.SelectedItem is not Equipe home || PickerAway.SelectedItem is not Equipe away)
            {
                await DisplayAlert("Simulation", "Please select both home and away teams.", "OK");
                return;
            }
            if (home.Id == away.Id) { await DisplayAlert("Simulation", "Please choose two different teams.", "OK"); return; }

            // Ensure players loaded
            await _db.Entry(home).Collection(h => h.Joueurs).LoadAsync();
            await _db.Entry(away).Collection(a => a.Joueurs).LoadAsync();

            var match = _matchService.SimulateAndSaveMatch(home, away, applyInjuries: true);
            var msg = $"Result: {home.Nom} {match.Score1} - {match.Score2} {away.Nom}";
            var injured = await _db.Joueurs.Where(j => j.EstBlesse && (j.EquipeId == home.Id || j.EquipeId == away.Id)).ToListAsync();
            if (injured.Count > 0)
            {
                var injuredText = string.Join(Environment.NewLine, injured.Select(i => i.Nom + " (" + (i.Equipe?.Nom ?? "?") + ")"));
                msg += "\nInjured:\n" + injuredText;
            }
            await DisplayAlert("Match", msg, "OK");
            await LoadData();
        }
    }
}
