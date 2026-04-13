# Tutorial: Migrate from Official Avalonia Template to Crystal.Avalonia

This tutorial shows how to convert an existing official Avalonia project to use Crystal.Avalonia.

## Why Migrate?

| Feature | Official Template | Crystal.Avalonia |
|---------|------------------|------------------|
| Module System | None | Built-in modular architecture |
| DI Integration | Manual | Automatic via Microsoft.Extensions.DependencyInjection |
| MVVM Binding | Manual | Automatic View/ViewModel wiring |
| AOT Support | Basic | Full support with annotations |

## Step 1: Install Crystal.Avalonia

Add the NuGet package:

```bash
dotnet add package Crystal.Avalonia
```

Or in your `.csproj`:

```xml
<PackageReference Include="Crystal.Avalonia" />
```

## Step 2: Inherit from CrystalApplication

Replace your `App.axaml.cs`:

### Before (Official Template)

```csharp
public override void OnFrameworkInitializationCompleted()
{
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
        desktop.MainWindow = new MainWindow();
    }
    // ...
}
```

### After (Crystal.Avalonia)

```csharp
public class App : CrystalApplication  // Change base class
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void RegisterModules(IModuleRegistrar moduleRegistrar)
    {
        // Register modules here
    }

    public override void RegisterServices(IServiceCollection services)
    {
        // Register services here
    }

    public override void CreateShell(IServiceProvider serviceProvider)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();
    }
}
```

## Step 3: Register View/ViewModel Pairs

### Before

```csharp
// Manual wiring in code-behind
public MainWindow()
{
    InitializeComponent();
    DataContext = new MainViewModel();
}
```

### After

```csharp
// In RegisterServices
public override void RegisterServices(IServiceCollection services)
{
    services.AddMvvmBindingTransient<MainView, MainViewModel>();
}
```

### In XAML

```xml
<Window xmlns:vm="using:Crystal.Avalonia"
        ViewModelLocator.AutoWireViewModel="True"
        x:Class="MyApp.Views.MainWindow">
```

## Step 4: Create a Module (Optional)

### Before

All code in a single project with manual organization.

### After

Create a module class:

```csharp
public class SettingsModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmBindingTransient<SettingsView, SettingsViewModel>();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        // Module initialization
    }
}
```

Register in `App.axaml.cs`:

```csharp
public override void RegisterModules(IModuleRegistrar moduleRegistrar)
{
    moduleRegistrar.RegisterModule<SettingsModule>();
}
```

## Step 5: Update XAML Views

Enable `AutoWireViewModel` on each view:

```xml
<UserControl xmlns:vm="using:Crystal.Avalonia"
             ViewModelLocator.AutoWireViewModel="True"
             x:Class="MyApp.Views.MainView">
    <!-- Your content -->
</UserControl>
```

## Complete Comparison

### Project Structure

**Before:**
```
MyApp/
├── App.axaml
├── App.axaml.cs
├── ViewModels/
│   └── MainViewModel.cs
├── Views/
│   ├── MainWindow.axaml
│   └── MainView.axaml
└── Program.cs
```

**After (with Modules):**
```
MyApp/
├── App.axaml
├── App.axaml.cs
├── ViewModels/
├── Views/
├── Modules/           # Optional: organized by feature
│   ├── Settings/
│   │   ├── SettingsModule.cs
│   │   ├── SettingsView.axaml
│   │   └── SettingsViewModel.cs
│   └── Dashboard/
└── Program.cs
```

### Key Changes Summary

| What to Change | How |
|----------------|-----|
| Base class | `Application` → `CrystalApplication` |
| App startup | Override `CreateShell()` instead of `OnFrameworkInitializationCompleted()` |
| View/VM wiring | Use `AddMvvmBindingTransient` + `ViewModelLocator.AutoWireViewModel="True"` |
| Modules | Create classes implementing `IModule` and register via `RegisterModules()` |

## Next Steps

- [Module Development](module-development.md) - Learn more about the module system
- [MVVM Pattern](mvvm-pattern.md) - Master the MVVM pattern with Crystal.Avalonia
