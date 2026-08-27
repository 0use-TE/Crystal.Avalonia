# Upgrade from v1.2

Complete the 2.0 ViewModel-only DI and shell changes below, then [Upgrade from 2.0](upgrade-from-2.0.md) for 3.0.0 (`EnableViewLocator`, `ILifecycleAware`, instance `MvvmManager`).

## Breaking Changes in v2.0

| v1.2 | v2.0+ |
|------|-------|
| `AddMvvm*` registers **View + ViewModel** in DI | Only **ViewModel** in DI; `TView` is mapping only |
| `AddMvvmHybrid` available | **Removed** — use `AddMvvmTransient` or `AddMvvmSingleton` |
| `ViewLocator` resolves View from DI | `ViewLocator` uses `Activator.CreateInstance` |
| `CreateShell`: `new MainWindow()` | `CreateShell<MainWindow, MainView>()` (2.0.1+, still `new`, not DI) |

## Migration Steps

### 1. Replace `AddMvvmHybrid`

```csharp
services.AddMvvmSingleton<SettingsView, SettingsViewModel>();
// or
services.AddMvvmTransient<SettingsView, SettingsViewModel>();
```

### 2. Shell creation

```csharp
public override void CreateShell(IServiceProvider sp)
{
    CreateShell<MainWindow, MainView>();
}
```

Do not `AddTransient<MainWindow>()` — shell views are not in DI (2.0.1+).

> From **2.0.0**? See [Upgrade from 2.0.0](upgrade-from-2.0.0.md), then [Upgrade from 2.0](upgrade-from-2.0.md).

### 3. Navigation

```csharp
NavigationHost.Content = sp.GetRequiredService<MainViewModel>();
```
