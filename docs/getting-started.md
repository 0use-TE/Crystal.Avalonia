# Getting Started

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Step 1: Install the Template

```bash
dotnet new install Crystal.Avalonia.Template
```

## Step 2: Create a New Project

```bash
dotnet new crystal.avalonia -o MyApp
cd MyApp
dotnet run
```

## Project Structure

```
MyApp/
├── ViewModels/
│   └── MainViewModel.cs
├── Views/
│   ├── MainView.axaml
│   └── MainWindow.axaml
├── App.axaml.cs               # Application entry point
└── Program.cs
```

## Quick Example

### App.axaml.cs

```csharp
public partial class App : CrystalApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void RegisterModules(IModuleRegistrar moduleRegistrar)
    {
        // Register modules here (optional)
        // moduleRegistrar.RegisterModule<MyModule>();
    }

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmBindingTransient<MainView, MainViewModel>();
    }

    public override void CreateShell(IServiceProvider serviceProvider)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();
    }
}
```

### ViewModel

```csharp
public class MainViewModel : ObservableObject
{
    public string Greeting => "Welcome to Crystal.Avalonia!";
}
```

### View (AXAML)

```xml
<UserControl xmlns:vm="using:Crystal.Avalonia"
             ViewModelLocator.AutoWireViewModel="True">
    <TextBlock Text="{Binding Greeting}" FontSize="24"/>
</UserControl>
```

## Next Steps

- [MVVM Pattern](tutorials/mvvm-pattern.md) - Learn the MVVM pattern
- [Module Development](tutorials/module-development.md) - Create reusable modules
- [Migrate from Avalonia](tutorials/migrate-from-avalonia.md) - Convert official template to Crystal
