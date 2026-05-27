# Getting Started (v1.2)

> For **v2.0+**, see [v2.0 Getting Started](../v2.0/getting-started.md).

## Install & Run

```bash
dotnet new install CrystalTemplate
dotnet new CT -o MyApp
cd MyApp && dotnet run
```

## App.axaml.cs

```csharp
public partial class App : CrystalApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmTransient<MainView, MainViewModel>();
    }

    public override void CreateShell(IServiceProvider sp)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();
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

ViewLocator resolves the View from DI.
