# Getting Started

This tutorial will guide you through creating your first Crystal.Avalonia application.

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- An IDE with Avalonia support (Visual Studio 2022+, Rider, or VS Code with Avalonia extension)

## Step 1: Install the Template

First, install the Crystal.Avalonia project template:

```bash
dotnet new install Crystal.Avalonia.Template
```

This adds the `crystal.avalonia` template to `dotnet new`.

## Step 2: Create a New Project

Create a new project using the template:

```bash
dotnet new crystal.avalonia -o MyFirstApp
cd MyFirstApp
```

The template creates a complete project structure:

```
MyFirstApp/
├── ModuleA/              # Example module
│   ├── ModuleA.cs
│   ├── ModuleAView.axaml
│   ├── ModuleAView.axaml.cs
│   ├── ModuleAViewModel.cs
│   └── ModuleA.csproj
├── ModuleB/              # Another example module
├── ShareBaseClass/       # Shared contracts
├── MyFirstApp/           # Main application
│   ├── App.axaml
│   ├── App.axaml.cs
│   ├── ViewModels/
│   │   └── MainViewModel.cs
│   └── Views/
│       ├── MainWindow.axaml
│       └── MainWindow.axaml.cs
├── MyFirstApp.Desktop/   # Desktop-specific code
└── MyFirstApp.csproj     # Main project file
```

## Step 3: Explore the Project Structure

### App.axaml.cs - Application Entry Point

```csharp
public partial class App : CrystalApplication
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Register modules (optional)
    public override void RegisterModules(IModuleRegistrar moduleRegistrar)
    {
        moduleRegistrar.RegisterModule<ModuleA>();
    }

    // Register View/ViewModel pairs
    public override void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmBindingTransient<MainView, MainViewModel>();
    }

    // Create the main window
    public override void CreateShell(IServiceProvider serviceProvider)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();
    }
}
```

### MainViewModel.cs - ViewModel Example

```csharp
public class MainViewModel : ObservableObject
{
    private string _greeting = "Welcome to Crystal.Avalonia!";

    public string Greeting
    {
        get => _greeting;
        set => SetProperty(ref _greeting, value);
    }
}
```

### MainView.axaml - View with Data Binding

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Crystal.Avalonia"
             vm:ViewModelLocator.AutoWireViewModel="True">

    <StackPanel Margin="20">
        <TextBlock Text="{Binding Greeting}"
                   FontSize="24"
                   HorizontalAlignment="Center"/>
    </StackPanel>
</UserControl>
```

## Step 4: Create a Module

Modules are independent units that can be shared across projects.

### 1. Define the Module

```csharp
using Crystal.Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Modules.Settings;

public class SettingsModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        // Register View/ViewModel pair for this module
        services.AddMvvmBindingTransient<SettingsView, SettingsViewModel>();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        // Perform initialization (e.g., load settings, subscribe to events)
    }
}
```

### 2. Register the Module

In `App.axaml.cs`:

```csharp
public override void RegisterModules(IModuleRegistrar moduleRegistrar)
{
    moduleRegistrar.RegisterModule<SettingsModule>();
}
```

## Step 5: Choose Binding Mode

Crystal.Avalonia provides two binding modes:

### Transient (Default)

Both View and ViewModel are created each time:

```csharp
services.AddMvvmBindingTransient<MainView, MainViewModel>();
// Use this when: Each view instance needs its own ViewModel instance
```

### Singleton

View is transient, but ViewModel is singleton:

```csharp
services.AddMvvmBindingSingleton<SettingsView, SettingsViewModel>();
// Use this when: The same ViewModel instance should be shared
```

## Step 6: Run the Application

```bash
dotnet run
```

For desktop applications:

```bash
dotnet run --project MyFirstApp.Desktop
```

## Understanding the MVVM Flow

1. **Startup** - `CrystalApplication.OnFrameworkInitializationCompleted()` is called
2. **Module Registration** - All modules are registered via `RegisterModules()`
3. **Service Registration** - Services are registered via `RegisterServices()` and each module's `RegisterServices()`
4. **Shell Creation** - `CreateShell()` is called with the fully configured `ServiceProvider`
5. **View Resolution** - When a View with `AutoWireViewModel="True"` loads, its corresponding ViewModel is resolved from DI and assigned to `DataContext`

## Next Steps

- [Create a More Complex App](tutorials/create-first-app.md) - Build a complete application step by step
- [Module Development](tutorials/module-development.md) - Deep dive into the module system
- [API Reference](../api/index.md) - Explore all available APIs
