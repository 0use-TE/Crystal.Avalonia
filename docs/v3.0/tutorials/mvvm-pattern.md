# MVVM Pattern

## Binding Modes

| Mode | XAML | View | ViewModel |
|------|------|------|-----------|
| **View-first** | `ViewModelLocator.AutoWireViewModel="True"` | XAML / `new` | From DI |
| **ViewModel-first** | `ContentControl Content="{Binding Vm}"` | `ViewLocator` creates | From DI |

`CrystalOptions.EnableViewLocator` only toggles ViewModel-first (`ViewLocator` on `DataTemplates`). AutoWire always works when a mapping exists.

## Registration

`AddMvvmTransient` / `AddMvvmSingleton` register **ViewModel in DI** and record **View mapping** on `CrystalApplication.Mvvm`:

```csharp
services.AddMvvmTransient<MainView, MainViewModel>();
services.AddMvvmSingleton<AboutView, AboutViewModel>();
```

Shell views (`MainWindow`, etc.) are **not** in DI — use `CreateShell`:

```csharp
CreateShell<MainWindow, MainView>();
```

## ILifecycleAware (Optional)

`OnLoadedAsync` runs on every Loaded. `isFirstLoad` is per ViewModel **instance**.

Refresh on every visit:

```csharp
public partial class MainViewModel : ObservableObject, ILifecycleAware
{
    public Task OnLoadedAsync(bool isFirstLoad) => LoadDataAsync();
    public Task OnUnloaded() => SaveStateAsync();
}
```

Run setup only once (typical for a singleton ViewModel):

```csharp
public Task OnLoadedAsync(bool isFirstLoad)
{
    if (!isFirstLoad) return Task.CompletedTask;
    return LoadDataAsync();
}
```

> Tab / WebView: `OnUnloaded` fires whenever the view leaves the visual tree.

## API Summary

| API | Description |
|-----|-------------|
| `AddMvvmTransient<TView, TViewModel>()` | `AddTransient<TViewModel>()` + mapping |
| `AddMvvmSingleton<TView, TViewModel>()` | `AddSingleton<TViewModel>()` + mapping |
| `CreateShell<TWindow, TView>()` | Create shell with `new` by platform lifetime |
| `CrystalApplication.Mvvm` | Per-app mappings and `ServiceProvider` |
| `CrystalOptions.EnableViewLocator` | Whether to register ViewModel-first `ViewLocator` |
| `ILifecycleAware` | Optional load/unload hooks |

## Further Reading

> **How it works:** [Architecture — MVVM Wiring](../architecture.md#mvvm-wiring) — `ViewModelLocator` vs `ViewLocator`, mapping, and lifecycle binding. [Design Decisions](../architecture.md#design-decisions) — why View is not in DI.
