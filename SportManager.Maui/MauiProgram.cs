using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SportManager.Models;
using SportManager.Services;
using System.IO;
using Microsoft.Maui.Storage;
using Microsoft.EntityFrameworkCore;

namespace SportManager.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => { });

            // Configure SQLite DB path in app data directory
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "sport.db");

            // Register services
            builder.Services.AddDbContext<SportContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));
            builder.Services.AddScoped<MatchService>();

            // Register pages for DI
            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}
