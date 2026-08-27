# Upgrade from 2.0

Crystal.Avalonia **3.0.0** keeps ViewModel-only DI and `CreateShell<TWindow, TView>()`. These APIs changed.

## What Changed

| 2.0.1 | 3.0.0 |
|-------|-------|
| Static `MvvmManager` dictionaries and `MvvmManager.ServiceProvider` | Instance on `CrystalApplication.Mvvm`; `AddMvvm*` writes to that instance |
| `CrystalOptions.EnableViewModelLocator` | **`EnableViewLocator`** — only whether `ViewLocator` is added to `DataTemplates`. Does not control `AutoWireViewModel` |
| `OnLoadedAsync()` / one-shot Loaded+Unloaded | `OnLoadedAsync(bool isFirstLoad)` on every Loaded; `OnUnloaded` on every Unload. `isFirstLoad` is per ViewModel instance |
| `ServiceProvider` assigned after `InitModules` | Assigned **before** `InitModules` so AutoWire works during module init |

`AddMvvmTransient` / `AddMvvmSingleton` call sites are unchanged.

## Migration Steps

### 1. Rename the option

```csharp
// 2.0
CrystalOptions.EnableViewModelLocator = false;

// 3.0
CrystalOptions.EnableViewLocator = false;
```

### 2. Update `ILifecycleAware`

```csharp
public Task OnLoadedAsync(bool isFirstLoad)
{
    if (!isFirstLoad) return Task.CompletedTask;
    return LoadDataAsync();
}

public Task OnUnloaded() => Task.CompletedTask;
```

### 3. Stop using static `MvvmManager.ServiceProvider`

Use the `IServiceProvider` passed to `CreateShell` / `InitializeModule`, or `CrystalApplication` current app's `Mvvm.ServiceProvider`.

```csharp
var mvvm = ((CrystalApplication)Application.Current!).Mvvm;
```

### Package / Template

```bash
dotnet add package Crystal.Avalonia --version 3.0.0
dotnet new install CrystalTemplate::3.0.0
```

## Unchanged in 3.0.0

- ViewModel-only DI via `AddMvvmTransient` / `AddMvvmSingleton`
- Shell created with `CreateShell<MainWindow, MainView>()` (`new`, not DI)
- `ViewModelLocator.AutoWireViewModel`
- `IModule` (still no scanning, no built-in navigation or event aggregator)
- AOT annotations
