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
        services.AddMvvmTransient<UserListView, UserListViewModel>();
        services.AddMvvmTransient<UserDetailView, UserDetailViewModel>();

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

## Conditional registration

The library always constructs every registered module at startup. To skip a feature, do not call `RegisterModule` — that decision belongs in your app:

```csharp
public override void RegisterModules(IModuleRegistrar moduleRegistrar)
{
    if (User.IsAdmin)
        moduleRegistrar.RegisterModule<AdminModule>();

    moduleRegistrar.RegisterModule<CommonModule>();
}
```

There is no built-in lazy loading, module `DependsOn`, navigation service, or event aggregator. Share data across modules with ordinary DI services you register yourself.

## Best Practices

| Practice | Why |
|----------|-----|
| Keep modules focused | Each module should represent a single feature area |
| Define module contracts | Use interfaces for cross-module communication |
| Minimize module dependencies | Modules should be as independent as possible |
| Register explicitly | No assembly scanning — every module is listed in `RegisterModules` |

## Next Steps

- [Dependency Injection](dependency-injection.md) - Advanced DI patterns and service registration
- [Navigation](navigation.md) - Implementing navigation between views

## Further Reading

> **How it works:** [Architecture — Module System](../architecture.md#module-system) — `RegisterModule` → `InitService` → `InitModules` pipeline and why there is no assembly scanning.
