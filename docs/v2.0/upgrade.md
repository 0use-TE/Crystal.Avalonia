# Upgrade Guide

Current release: **Crystal.Avalonia 2.0.1**

| From | Guide |
|------|-------|
| **2.0.0** | [Upgrade from 2.0.0](upgrade-from-2.0.0.md) — `CreateShellFromDi` removed, shell no longer in DI |
| **1.2.x** | [Upgrade from v1.2](upgrade-from-1.2.md) — ViewModel-only DI, no `AddMvvmHybrid` |

## Quick Reference (2.0.1)

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
- **View** → mapping only; created by XAML or `ViewLocator`
- **Shell** (`MainWindow` / `MainView`) → `CreateShell<...>()` with `new`, not DI
