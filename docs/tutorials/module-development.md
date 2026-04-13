# Tutorial: Module Development

Modules are the core building blocks of Crystal.Avalonia applications. They provide a way to organize your code into independent, reusable units.

## What is a Module?

A module is a class that implements `IModule` and encapsulates:

- **Services** - Dependencies that the module provides
- **Views** - UI components specific to the module
- **ViewModels** - Business logic for the module's views
- **Initialization Logic** - Setup that runs when the app starts

## Creating a Module

### Step 1: Define the Module Class

```csharp
using Crystal.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MyApp.Modules.UserManagement;

public class UserManagementModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        // Register View/ViewModel pairs
        services.AddMvvmBindingTransient<UserListView, UserListViewModel>();
        services.AddMvvmBindingTransient<UserDetailView, UserDetailViewModel>();

        // Register other services
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddTransient<UserService>();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        // Perform initialization tasks
        var userService = serviceProvider.GetRequiredService<UserService>();
        userService.LoadUsers();
    }
}
```

### Step 2: Register the Module

In your `App.axaml.cs`:

```csharp
public override void RegisterModules(IModuleRegistrar moduleRegistrar)
{
    moduleRegistrar.RegisterModule<UserManagementModule>();
}
```

## Module Project Structure

For larger applications, consider placing each module in its own project:

```
MySolution/
├── MyApp/                    # Main application
│   └── App.axaml.cs
├── MyApp.Modules.UserManagement/  # Module in separate project
│   ├── UserManagementModule.cs
│   ├── Views/
│   ├── ViewModels/
│   └── Services/
└── MyApp.Modules.Settings/   # Another module
    └── ...
```

## Sharing Modules Between Apps

Modules can be easily shared across different applications:

1. Create a separate project for the module
2. Add the module project as a reference to your main app
3. Register the module in your app

### Module Project File (.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Crystal.Avalonia" />
    </ItemGroup>

</Project>
```

### Consuming in Main App

```csharp
// In App.axaml.cs of the main application
public override void RegisterModules(IModuleRegistrar moduleRegistrar)
{
    // Register modules from referenced projects
    moduleRegistrar.RegisterModule<MyApp.Modules.UserManagement.UserManagementModule>();
    moduleRegistrar.RegisterModule<MyApp.Modules.Settings.SettingsModule>();
}
```

## Advanced Module Patterns

### Module with Dependencies

```csharp
public class ReportingModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmBindingTransient<ReportView, ReportViewModel>();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        // Get services from the main app
        var logger = serviceProvider.GetRequiredService<ILogger>();
        logger.LogInformation("Reporting module initialized");
    }
}
```

### Conditional Module Loading

```csharp
public override void RegisterModules(IModuleRegistrar moduleRegistrar)
{
    // Only load admin module for admin users
    if (User.IsAdmin)
    {
        moduleRegistrar.RegisterModule<AdminModule>();
    }

    moduleRegistrar.RegisterModule<CommonModule>();
}
```

### Module with Shared Base Class

```csharp
// Base class for all modules in your application
public abstract class AppModule : IModule
{
    protected IServiceCollection? Services { get; private set; }
    protected IServiceProvider? ServiceProvider { get; private set; }

    public void RegisterServices(IServiceCollection services)
    {
        Services = services;
        OnRegisterServices();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        OnInitialize();
    }

    protected abstract void OnRegisterServices();
    protected abstract void OnInitialize();
}

// Usage
public class MyModule : AppModule
{
    protected override void OnRegisterServices()
    {
        Services!.AddMvvmBindingTransient<MyView, MyViewModel>();
    }

    protected override void OnInitialize()
    {
        var config = ServiceProvider!.GetRequiredService<AppConfig>();
        // Use config...
    }
}
```

## Best Practices

| Practice | Why |
|----------|-----|
| Keep modules focused | Each module should represent a single feature area |
| Define module contracts | Use interfaces for cross-module communication |
| Minimize module dependencies | Modules should be as independent as possible |
| Use shared base classes | Reduces boilerplate for large module collections |
| Consider lazy loading | For large modules that aren't always needed |

## Cross-Module Communication

### Option 1: Shared Services

```csharp
// In a shared project
public interface INavigationService
{
    void NavigateTo<TView>() where TView : Control;
}

// In ModuleA
public class NavigationService : INavigationService { ... }

// In ModuleA's RegisterServices
services.AddSingleton<INavigationService, NavigationService>();

// In ModuleB, use the shared service
public class ModuleB : IModule
{
    public void InitializeModule(IServiceProvider sp)
    {
        var nav = sp.GetRequiredService<INavigationService>();
        nav.NavigateTo<ModuleAView>();
    }
}
```

### Option 2: Event Aggregator

```csharp
// Simple event aggregator
public interface IEventAggregator
{
    void Subscribe<TEvent>(Action<TEvent> handler);
    void Publish<TEvent>(TEvent event);
}

// Usage
public class ModuleA : IModule
{
    public void InitializeModule(IServiceProvider sp)
    {
        var ea = sp.GetRequiredService<IEventAggregator>();
        ea.Subscribe<UserCreatedEvent>(e => /* react */);
    }
}

public class ModuleB : IModule
{
    public void InitializeModule(IServiceProvider sp)
    {
        var ea = sp.GetRequiredService<IEventAggregator>();
        ea.Publish(new UserCreatedEvent { UserId = 123 });
    }
}
```

## Next Steps

- [Dependency Injection](dependency-injection.md) - Advanced DI patterns and service registration
- [Navigation](navigation.md) - Implementing navigation between views
