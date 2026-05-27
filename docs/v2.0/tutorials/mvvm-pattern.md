# MVVM Pattern

## Binding Modes

| Mode | XAML | View | ViewModel |
|------|------|------|-----------|
| **View-first** | `ViewModelLocator.AutoWireViewModel="True"` | XAML / DI shell / `new` | From DI |
| **ViewModel-first** | `ContentControl Content="{Binding Vm}"` | `ViewLocator` creates | From DI |

## Registration

`AddMvvmTransient` / `AddMvvmSingleton` register **ViewModel in DI** and record **View mapping only**:

```csharp
services.AddMvvmTransient<MainView, MainViewModel>();
services.AddMvvmSingleton<AboutView, AboutViewModel>();
```

Shell views (`MainWindow`, etc.) are **not** included — register separately:

```csharp
services.AddTransient<MainWindow>();
CreateShellFromDi<MainWindow, MainView>(serviceProvider);
```

## ILifecycleAware (Optional)

Implement on ViewModel for automatic `OnLoadedAsync` / `OnUnloaded`:

```csharp
public partial class MainViewModel : ObservableObject, ILifecycleAware
{
    public Task OnLoadedAsync() => LoadDataAsync();
    public Task OnUnloaded() => SaveStateAsync();
}
```

> Tab / WebView: `OnUnloaded` fires when leaving visual tree — manage caching in your app if needed.

## API Summary

| API | Description |
|-----|-------------|
| `AddMvvmTransient<TView, TViewModel>()` | `AddTransient<TViewModel>()` + mapping |
| `AddMvvmSingleton<TView, TViewModel>()` | `AddSingleton<TViewModel>()` + mapping |
| `CreateShellFromDi<TWindow, TView>(sp)` | Resolve shell from DI by platform lifetime |
| `ILifecycleAware` | Optional load/unload hooks |

## Further Reading

> **How it works:** [Architecture — MVVM Wiring](../architecture.md#mvvm-wiring) — `ViewModelLocator` vs `ViewLocator`, mapping dictionaries, and lifecycle binding. [Design Decisions](../architecture.md#design-decisions) — why View is not in DI.
