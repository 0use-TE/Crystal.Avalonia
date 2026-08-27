# Upgrade from 2.0.0

Crystal.Avalonia **2.0.1** simplifies shell creation: shell views are no longer resolved from DI.

## What Changed

| 2.0.0 | 2.0.1 |
|-------|-------|
| `CreateShellFromDi<TWindow, TView>(sp)` | **Removed** — use `CreateShell<TWindow, TView>()` |
| `services.AddTransient<MainWindow>()` | **Not needed** — shell created with `new` |
| `services.AddTransient<MainView>()` | **Not needed** |

Shell views (`MainWindow`, `MainView`) typically have no constructor dependencies. ViewModels inside them still come from DI via `ViewModelLocator.AutoWireViewModel="True"`.

## Migration Steps

### Before (2.0.0)

```csharp
public override void RegisterServices(IServiceCollection services)
{
    services.AddMvvmTransient<MainView, MainViewModel>();
    services.AddTransient<MainWindow>();
    services.AddTransient<MainView>();
}

public override void CreateShell(IServiceProvider sp)
{
    CreateShellFromDi<MainWindow, MainView>(sp);
}
```

### After (2.0.1)

```csharp
public override void RegisterServices(IServiceCollection services)
{
    services.AddMvvmTransient<MainView, MainViewModel>();
}

public override void CreateShell(IServiceProvider sp)
{
    CreateShell<MainWindow, MainView>();
}
```

### Package / Template

```bash
dotnet add package Crystal.Avalonia --version 2.0.1
dotnet new install CrystalTemplate::2.0.1
```

## If Shell Needs Constructor Injection

This is uncommon. Override `CreateShell` manually:

```csharp
public override void CreateShell(IServiceProvider sp)
{
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        desktop.MainWindow = sp.GetRequiredService<MainWindow>();
}
```

Register `MainWindow` in DI only in this case.

## Unchanged in 2.0.1

- ViewModel-only DI via `AddMvvmTransient` / `AddMvvmSingleton`
- `ViewModelLocator` / `ViewLocator`
- `ILifecycleAware`, modules, AOT support
