# MVVM Pattern (v1.2)

> For **v2.0+**, see [v2.0 MVVM Pattern](~/docs/v2.0/tutorials/mvvm-pattern.md).

## Registration Modes

| Method | View in DI | ViewModel in DI |
|--------|------------|-----------------|
| `AddMvvmTransient` | `AddTransient<TView>()` | `AddTransient<TViewModel>()` |
| `AddMvvmHybrid` | `AddTransient<TView>()` | `AddSingleton<TViewModel>()` |
| `AddMvvmSingleton` | `AddSingleton<TView>()` | `AddSingleton<TViewModel>()` |

```csharp
services.AddMvvmTransient<MainView, MainViewModel>();
services.AddMvvmHybrid<SettingsView, SettingsViewModel>();
services.AddMvvmSingleton<AboutView, AboutViewModel>();
```

## ViewModel-First

ViewLocator resolves View from DI:

```csharp
// ContentControl.Content = viewModelInstance
// → ViewLocator → sp.GetService<TView>()
```

## ILifecycleAware

```csharp
public partial class MainViewModel : ObservableObject, ILifecycleAware
{
    public Task OnLoadedAsync() => LoadDataAsync();
    public Task OnUnloaded() => SaveStateAsync();
}
```

Automatically hooked when View loads/unloads.
