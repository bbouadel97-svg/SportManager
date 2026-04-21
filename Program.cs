using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SportManager.Models;
using SportManager.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<SportContext>();
builder.Services.AddScoped<MatchService>();

var app = builder.Build();

app.Urls.Clear();
app.Urls.Add("http://localhost:5000");

app.UseStaticFiles();

string Layout(string title, string body)
{
    return $@"<!doctype html>
<html>
<head>
  <meta charset='utf-8'/>
  <meta name='viewport' content='width=device-width, initial-scale=1'/>
  <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css' rel='stylesheet'/>
  <title>{title}</title>
</head>
<body class='p-4'>
<div class='container'>
  <h1>{title}</h1>
  <nav class='mb-3'>
    <a class='btn btn-sm btn-primary' href='/'>Home</a>
    <a class='btn btn-sm btn-secondary' href='/players/new'>Create Player</a>
    <a class='btn btn-sm btn-secondary' href='/teams/new'>Create Team</a>
  </nav>
  {body}
</div>
</body>
</html>";
}

app.MapGet("/", async (SportContext db) =>
{
    var teams = await db.Equipes.Include(e => e.Joueurs).ToListAsync();
    var players = await db.Joueurs.ToListAsync();
    var sb = new StringBuilder();

    sb.AppendLine("<div class='row'>");
    sb.AppendLine("<div class='col-md-6'>");
    sb.AppendLine("<h3>Teams</h3>");
    sb.AppendLine("<ul class='list-group'>");
    foreach (var t in teams)
    {
        sb.AppendLine($"<li class='list-group-item d-flex justify-content-between align-items-center'>");
        sb.AppendLine($"<div><strong>{t.Nom}</strong> <small class='text-muted'>({t.Joueurs.Count} players)</small></div>");
        sb.AppendLine($"<div><a class='btn btn-sm btn-outline-primary me-1' href='/teams/edit/{t.Id}'>Edit</a><a class='btn btn-sm btn-outline-danger' href='/teams/delete/{t.Id}'>Delete</a></div>");
        sb.AppendLine("</li>");
    }
    sb.AppendLine("</ul>");
    sb.AppendLine("</div>");

    sb.AppendLine("<div class='col-md-6'>");
    sb.AppendLine("<h3>Players</h3>");
    sb.AppendLine("<ul class='list-group'>");
    foreach (var p in players)
    {
        var team = p.EquipeId.HasValue ? db.Equipes.Find(p.EquipeId.Value)?.Nom : "-";
        sb.AppendLine($"<li class='list-group-item'>{p.Nom} <small class='text-muted'>({team})</small> - V:{p.Vitesse} E:{p.Endurance} F:{p.Force} T:{p.Technique} Inj:{p.EstBlesse} <a class='btn btn-sm btn-outline-primary ms-2' href='/players/edit/{p.Id}'>Edit</a> <a class='btn btn-sm btn-outline-danger' href='/players/delete/{p.Id}'>Delete</a></li>");
    }
    sb.AppendLine("</ul>");
    sb.AppendLine("</div>");
    sb.AppendLine("</div>");

    // Match simulator form
    sb.AppendLine("<hr/>\n<h3>Simulate Match</h3>");
    sb.AppendLine("<form method='post' action='/simulate' class='row gy-2'>");
    sb.AppendLine("<div class='col-md-5'><select name='homeId' class='form-select'>");
    sb.AppendLine("<option value=''>-- Home team --</option>");
    foreach (var t in teams) sb.AppendLine($"<option value='{t.Id}'>{t.Nom}</option>");
    sb.AppendLine("</select></div>");
    sb.AppendLine("<div class='col-md-5'><select name='awayId' class='form-select'>");
    sb.AppendLine("<option value=''>-- Away team --</option>");
    foreach (var t in teams) sb.AppendLine($"<option value='{t.Id}'>{t.Nom}</option>");
    sb.AppendLine("</select></div>");
    sb.AppendLine("<div class='col-md-2'><button class='btn btn-success w-100' type='submit'>Simulate</button></div>");
    sb.AppendLine("</form>");

    return Results.Content(Layout("Sport Manager", sb.ToString()), "text/html");
});

// Create player form
app.MapGet("/players/new", async (SportContext db) =>
{
    var teams = await db.Equipes.ToListAsync();
    var postes = await db.Postes.ToListAsync();
    var sb = new StringBuilder();
    sb.AppendLine("<form method='post' action='/players/create' class='row g-2'>");
    sb.AppendLine("<div class='col-md-6'><input name='nom' class='form-control' placeholder='Name' required /></div>");
    sb.AppendLine("<div class='col-md-2'><input name='age' type='number' class='form-control' placeholder='Age' value='18' /></div>");
    sb.AppendLine("<div class='col-md-4'></div>");
    sb.AppendLine("<div class='col-md-3'><input name='vitesse' type='number' class='form-control' placeholder='Vitesse' value='50' /></div>");
    sb.AppendLine("<div class='col-md-3'><input name='endurance' type='number' class='form-control' placeholder='Endurance' value='50' /></div>");
    sb.AppendLine("<div class='col-md-3'><input name='force' type='number' class='form-control' placeholder='Force' value='50' /></div>");
    sb.AppendLine("<div class='col-md-3'><input name='technique' type='number' class='form-control' placeholder='Technique' value='50' /></div>");
    sb.AppendLine("<div class='col-md-6'><select name='equipeId' class='form-select'><option value=''>-- Team --</option>");
    foreach (var t in teams) sb.AppendLine($"<option value='{t.Id}'>{t.Nom}</option>");
    sb.AppendLine("</select></div>");
    sb.AppendLine("<div class='col-md-6'><select name='posteId' class='form-select'><option value=''>-- Poste --</option>");
    foreach (var p in postes) sb.AppendLine($"<option value='{p.Id}'>{p.Nom}</option>");
    sb.AppendLine("</select></div>");
    sb.AppendLine("<div class='col-12'><button class='btn btn-primary' type='submit'>Create</button></div>");
    sb.AppendLine("</form>");
    return Results.Content(Layout("Create Player", sb.ToString()), "text/html");
});

app.MapPost("/players/create", async (HttpRequest req, SportContext db) =>
{
    var form = await req.ReadFormAsync();
    var nom = form["nom"].ToString();
    var age = int.TryParse(form["age"], out var a) ? a : 18;
    var v = int.TryParse(form["vitesse"], out var vv) ? vv : 50;
    var en = int.TryParse(form["endurance"], out var ee) ? ee : 50;
    var fo = int.TryParse(form["force"], out var ff) ? ff : 50;
    var te = int.TryParse(form["technique"], out var tt) ? tt : 50;
    var equipeId = int.TryParse(form["equipeId"], out var eid) ? (int?)eid : null;
    var posteId = int.TryParse(form["posteId"], out var pid) ? (int?)pid : null;

    var j = new Joueur { Nom = nom, Age = age, Vitesse = v, Endurance = en, Force = fo, Technique = te, EquipeId = equipeId, PosteId = posteId };
    db.Joueurs.Add(j);
    await db.SaveChangesAsync();
    return Results.Redirect("/");
});

// Edit player
app.MapGet("/players/edit/{id}", async (int id, SportContext db) =>
{
    var j = await db.Joueurs.FindAsync(id);
    if (j == null) return Results.Content(Layout("Not Found", "<p>Player not found</p>"), "text/html");
    var teams = await db.Equipes.ToListAsync();
    var postes = await db.Postes.ToListAsync();
    var sb = new StringBuilder();
    sb.AppendLine($"<form method='post' action='/players/update/{id}' class='row g-2'>");
    sb.AppendLine($"<div class='col-md-6'><input name='nom' class='form-control' value='{j.Nom}' required /></div>");
    sb.AppendLine($"<div class='col-md-2'><input name='age' type='number' class='form-control' value='{j.Age}' /></div>");
    sb.AppendLine($"<div class='col-md-3'><input name='vitesse' type='number' class='form-control' value='{j.Vitesse}' /></div>");
    sb.AppendLine($"<div class='col-md-3'><input name='endurance' type='number' class='form-control' value='{j.Endurance}' /></div>");
    sb.AppendLine($"<div class='col-md-3'><input name='force' type='number' class='form-control' value='{j.Force}' /></div>");
    sb.AppendLine($"<div class='col-md-3'><input name='technique' type='number' class='form-control' value='{j.Technique}' /></div>");
    sb.AppendLine("<div class='col-md-6'><select name='equipeId' class='form-select'><option value=''>-- Team --</option>");
    foreach (var t in teams) sb.AppendLine($"<option value='{t.Id}' {(j.EquipeId==t.Id?"selected":"")}>{t.Nom}</option>");
    sb.AppendLine("</select></div>");
    sb.AppendLine("<div class='col-md-6'><select name='posteId' class='form-select'><option value=''>-- Poste --</option>");
    foreach (var p in postes) sb.AppendLine($"<option value='{p.Id}' {(j.PosteId==p.Id?"selected":"")}>{p.Nom}</option>");
    sb.AppendLine("</select></div>");
    sb.AppendLine("<div class='col-12'><button class='btn btn-primary' type='submit'>Update</button></div>");
    sb.AppendLine("</form>");
    return Results.Content(Layout("Edit Player", sb.ToString()), "text/html");
});

app.MapPost("/players/update/{id}", async (int id, HttpRequest req, SportContext db) =>
{
    var j = await db.Joueurs.FindAsync(id);
    if (j == null) return Results.Redirect("/");
    var form = await req.ReadFormAsync();
    j.Nom = form["nom"]; j.Age = int.TryParse(form["age"], out var a)?a:j.Age;
    j.Vitesse = int.TryParse(form["vitesse"], out var v)?v:j.Vitesse;
    j.Endurance = int.TryParse(form["endurance"], out var e)?e:j.Endurance;
    j.Force = int.TryParse(form["force"], out var f)?f:j.Force;
    j.Technique = int.TryParse(form["technique"], out var t)?t:j.Technique;
    j.EquipeId = int.TryParse(form["equipeId"], out var eid)?(int?)eid:j.EquipeId;
    j.PosteId = int.TryParse(form["posteId"], out var pid)?(int?)pid:j.PosteId;
    await db.SaveChangesAsync();
    return Results.Redirect("/");
});

app.MapGet("/players/delete/{id}", async (int id, SportContext db) =>
{
    var j = await db.Joueurs.FindAsync(id);
    if (j != null) { db.Joueurs.Remove(j); await db.SaveChangesAsync(); }
    return Results.Redirect("/");
});

// Teams CRUD
app.MapGet("/teams/new", () => Results.Content(Layout("Create Team", "<form method='post' action='/teams/create' class='row g-2'><div class='col-md-6'><input name='nom' class='form-control' placeholder='Team name' required/></div><div class='col-12'><button class='btn btn-primary'>Create</button></div></form>"), "text/html"));
app.MapPost("/teams/create", async (HttpRequest req, SportContext db) =>
{
    var f = await req.ReadFormAsync(); var name = f["nom"].ToString(); db.Equipes.Add(new Equipe{Nom=name}); await db.SaveChangesAsync(); return Results.Redirect("/");
});
app.MapGet("/teams/edit/{id}", async (int id, SportContext db) =>
{
    var t = await db.Equipes.FindAsync(id);
    if (t==null) return Results.Content(Layout("Not Found","<p>Team not found</p>"),"text/html");
    var body = $"<form method='post' action='/teams/update/{id}' class='row g-2'><div class='col-md-6'><input name='nom' class='form-control' value='{t.Nom}' required/></div><div class='col-12'><button class='btn btn-primary'>Update</button></div></form>";
    return Results.Content(Layout("Edit Team", body), "text/html");
});
app.MapPost("/teams/update/{id}", async (int id, HttpRequest req, SportContext db) => { var t = await db.Equipes.FindAsync(id); if (t!=null){ var f = await req.ReadFormAsync(); t.Nom = f["nom"]; await db.SaveChangesAsync(); } return Results.Redirect("/"); });
app.MapGet("/teams/delete/{id}", async (int id, SportContext db) => { var t = await db.Equipes.FindAsync(id); if (t!=null){ db.Equipes.Remove(t); await db.SaveChangesAsync(); } return Results.Redirect("/"); });

// Simulation
app.MapPost("/simulate", async (HttpRequest req, SportContext db, MatchService svc) =>
{
    var f = await req.ReadFormAsync(); if (!int.TryParse(f["homeId"], out var hid) || !int.TryParse(f["awayId"], out var aid)) return Results.Redirect("/");
    var home = await db.Equipes.Include(e => e.Joueurs).FirstOrDefaultAsync(e => e.Id == hid);
    var away = await db.Equipes.Include(e => e.Joueurs).FirstOrDefaultAsync(e => e.Id == aid);
    if (home==null||away==null) return Results.Redirect("/");
    var match = svc.SimulateAndSaveMatch(home, away, applyInjuries: true);
    return Results.Redirect($"/match/{match.Id}");
});

app.MapGet("/match/{id}", async (int id, SportContext db) =>
{
    var m = await db.Matches.FindAsync(id);
    if (m==null) return Results.Content(Layout("Not Found","<p>Match not found</p>"),"text/html");
    var home = db.Equipes.Find(m.Equipe1Id)?.Nom ?? "?";
    var away = db.Equipes.Find(m.Equipe2Id)?.Nom ?? "?";
    var sb = new StringBuilder();
    sb.AppendLine($"<p><strong>{home}</strong> {m.Score1} - {m.Score2} <strong>{away}</strong></p>");
    sb.AppendLine("<h4>Injured players (recent)</h4>");
    var injured = await db.Joueurs.Where(j => j.EstBlesse && (j.EquipeId==m.Equipe1Id || j.EquipeId==m.Equipe2Id)).ToListAsync();
    sb.AppendLine("<ul>");
    foreach (var i in injured) sb.AppendLine($"<li>{i.Nom} ({db.Equipes.Find(i.EquipeId ?? 0)?.Nom})</li>");
    sb.AppendLine("</ul>");
    sb.AppendLine("<a class='btn btn-sm btn-secondary' href='/'>Back</a>");
    return Results.Content(Layout($"Match {id}", sb.ToString()), "text/html");
});

// Ensure DB created and seed minimal data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SportContext>();
    db.Database.EnsureCreated();
    if (!db.Postes.Any())
    {
        db.Postes.AddRange(Poste.Gardien, Poste.Defense, Poste.Milieu, Poste.Attaquant);
        db.SaveChanges();
    }
    if (!db.Equipes.Any())
    {
        var psg = new Equipe { Nom = "PSG" };
        var om = new Equipe { Nom = "OM" };
        db.Equipes.AddRange(psg, om);
        db.SaveChanges();
    }
}

app.Run();
