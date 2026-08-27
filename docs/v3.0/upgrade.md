# Upgrade Guide

Current release: **Crystal.Avalonia 3.0.0**

**Versioning from 3.0.0:** `3.0.x` is bug fixes only. New abstractions go to `3.1+`. Breaking changes go to `4.0`. (2.0.1 removed `CreateShellFromDi` in a patch — that will not be repeated.)

| From | Guide |
|------|-------|
| **2.0.x** | [Upgrade from 2.0](upgrade-from-2.0.md) — instance `MvvmManager`, `EnableViewLocator`, `OnLoadedAsync(bool)` |
| **2.0.0** | First apply [Upgrade from 2.0.0](upgrade-from-2.0.0.md), then 3.0 |
| **1.2.x** | [Upgrade from v1.2](upgrade-from-1.2.md) then [Upgrade from 2.0](upgrade-from-2.0.md) |

## Quick Reference (3.0.0)

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

- **ViewModel** → DI (`AddMvvm*`)
- **View** → mapping on `CrystalApplication.Mvvm`; created by XAML or `ViewLocator`
- **Shell** → `CreateShell<...>()` with `new`, not DI
- **`CrystalOptions.EnableViewLocator`** → ViewModel-first `DataTemplates` only
