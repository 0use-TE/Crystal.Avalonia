# Module Development (v1.2)

> For **v2.0+**, see [v2.0 Module Development](~/docs/v2.0/tutorials/module-development.md).

## Basic Module

```csharp
public class SettingsModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmTransient<SettingsView, SettingsViewModel>();
        services.AddSingleton<ISettingsService, SettingsService>();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        // initialization
    }
}
```

Register in `App.axaml.cs`:

```csharp
public override void RegisterModules(IModuleRegistrar registrar)
{
    registrar.RegisterModule<SettingsModule>();
}
```

In v1.2, `AddMvvmTransient` registers **both** View and ViewModel in DI.

## Separate Module Projects

Modules can live in their own class library projects and be referenced by the main app.
