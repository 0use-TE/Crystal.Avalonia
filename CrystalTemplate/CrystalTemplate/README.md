# CrystalTemplate 3.1.1

Create a cross-platform Avalonia 12 application using Crystal.Avalonia 3.1.1,
.NET 10, dependency injection, and CommunityToolkit.Mvvm. The template includes
Desktop, Browser, Android, and iOS projects. Build the platform projects you need;
mobile and browser workloads must be installed separately.

## Install and create an application

```bash
dotnet new install CrystalTemplate::3.1.1
dotnet new CT -n MyApp -o MyApp
dotnet run --project MyApp/MyApp.Desktop/MyApp.Desktop.csproj
```

The generated `Directory.Packages.props` references Crystal.Avalonia **3.1.1**.
The sample demonstrates ViewModel wiring and `EventToCommand`, whose registered
routed event bindings support Native AOT in this library version. The Desktop
project enables Native AOT publishing for Release.

Updating the template affects newly created applications. Update existing applications'
Crystal.Avalonia package references separately. Publish Crystal.Avalonia 3.1.1 before
CrystalTemplate 3.1.1 so generated projects can restore the library from NuGet.

- [Documentation](https://0use.net/Crystal.Avalonia/docs/v3.0/introduction.html)
- [EventToCommand guide](https://0use.net/Crystal.Avalonia/docs/v3.0/event-to-command.html)
- [Source](https://github.com/0use-TE/Crystal.Avalonia)
