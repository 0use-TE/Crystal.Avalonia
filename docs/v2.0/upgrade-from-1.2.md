# Upgrade from v1.2

## Breaking Changes in v2.0

| v1.2 | v2.0 |
|------|------|
| `AddMvvm*` registers **View + ViewModel** in DI | Only **ViewModel** in DI; `TView` is mapping only |
| `AddMvvmHybrid` available | **Removed** — use `AddMvvmTransient` or `AddMvvmSingleton` |
| `ViewLocator` resolves View from DI | `ViewLocator` uses `Activator.CreateInstance` |
| `CreateShell`: `new MainWindow()` | `CreateShellFromDi<MainWindow, MainView>(sp)` + register shell in DI |

## Migration Steps

### 1. Replace `AddMvvmHybrid`

```csharp
// v1.2
services.AddMvvmHybrid<SettingsView, SettingsViewModel>();

// v2.0 — pick one lifetime for the ViewModel
services.AddMvvmSingleton<SettingsView, SettingsViewModel>();
// or
services.AddMvvmTransient<SettingsView, SettingsViewModel>();
```

### 2. Shell from DI

```csharp
services.AddTransient<MainWindow>();
services.AddTransient<MainView>();

public override void CreateShell(IServiceProvider sp)
{
    CreateShellFromDi<MainWindow, MainView>(sp);
}
```

### 3. Navigation

```csharp
// v1.2 — View from DI
var view = sp.GetRequiredService<MainView>();

// v2.0 — prefer ViewModel-first
NavigationHost.Content = sp.GetRequiredService<MainViewModel>();
// or View-first: new MainView() with AutoWireViewModel
```

## Unchanged

- `ILifecycleAware`
- `IModule` / module system
- `ViewModelLocator` / View-first binding
- AOT annotations
