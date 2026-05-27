# Getting Started

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Install & Run

```bash
dotnet new install CrystalTemplate
dotnet new CT -o MyApp
cd MyApp
dotnet run
```

## App.axaml.cs

```csharp
public partial class App : CrystalApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmTransient<MainView, MainViewModel>(); // ViewModel → DI, View → mapping
        services.AddTransient<MainWindow>();                  // Shell → DI (manual)
        services.AddTransient<MainView>();
    }

    public override void CreateShell(IServiceProvider sp)
    {
        CreateShellFromDi<MainWindow, MainView>(sp);
    }
}
```

## View (View-First)

```xml
<UserControl ViewModelLocator.AutoWireViewModel="True">
    <TextBlock Text="{Binding Greeting}"/>
</UserControl>
```

## ViewModel-First

```xml
<ContentControl Content="{Binding MainViewModel}"/>
```

## Next Steps

- [Architecture](architecture.md) — bootstrap pipeline & MVVM wiring
- [MVVM Pattern](tutorials/mvvm-pattern.md)
- [Module Development](tutorials/module-development.md)
