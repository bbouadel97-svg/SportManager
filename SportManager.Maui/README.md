Requirements

- A machine with .NET MAUI support (Windows with Visual Studio 2022/2023 + MAUI workload, macOS with Visual Studio for Mac, or Android setup). MAUI is not supported on Linux for building/running.
- .NET SDK that matches the TargetFrameworks (net8.0). Install from https://dotnet.microsoft.com/.
- Install the MAUI workload:
  dotnet workload install maui

Build & run (Windows recommended for quick test):

1. From solution root, restore:
   dotnet restore

2. Build the MAUI project (this requires the MAUI workload):
   dotnet build ./SportManager.Maui/SportManager.Maui.csproj -f net8.0-windows10.0.22621.0

3. Run from Visual Studio (recommended) or via CLI (Windows):
   dotnet run -p ./SportManager.Maui/SportManager.Maui.csproj -f net8.0-windows10.0.22621.0

Notes

- The CI/build environment used here does not have the MAUI workload or platform SDKs installed, therefore building the full solution in this environment fails with missing Microsoft.Maui types. Locally, after installing the workload and platform prerequisites, the MAUI project will build and run.

- I implemented a minimal MAUI UI (MainPage) connected to the existing EF Core models and MatchService. The app stores the SQLite DB in the platform app data directory.

Next steps I can take for you

- Add more pages (player detail, poste management) and polish UI.
- Implement injury recovery over time and nicer match logs.
- Prepare a Windows .msix or Android .apk packaging configuration.

Tell me which next step you want and whether you will build/run locally (I can provide exact commands for your OS).
