# MVVM Pattern

## Binding Modes

| Mode | XAML | View | ViewModel |
|------|------|------|-----------|
| **View-first** | `ViewModelLocator.AutoWireViewModel="True"` | XAML / `new` | From DI |
| **ViewModel-first** | `ContentControl Content="{Binding Vm}"` | `ViewLocator` creates | From DI |

`Options.EnableViewLocator` only toggles ViewModel-first (`ViewLocator` on `DataTemplates`). AutoWire always works when a mapping exists. Configure via `ConfigureOptions`:

```csharp
public override void ConfigureOptions(CrystalOptions options)
{
    options.EnableViewLocator = true; // default
}
```

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

## EventToCommand (Optional)

Bridge View events to `ICommand` without Avalonia.Xaml.Behaviors. Types live in the **default Avalonia xmlns** (same as `ViewModelLocator`) — no extra xmlns.

Single event (shorthand):

```xml
<Button Content="Save"
        EventToCommand.EventName="Click"
        EventToCommand.Command="{Binding SaveCommand}"
        EventToCommand.CommandParameter="{Binding SelectedItem}"/>
```

Multiple events:

```xml
<Border>
  <EventToCommand.Bindings>
    <EventBinding EventName="PointerPressed" Command="{Binding PressCommand}" PassEventArgs="True"/>
    <EventBinding EventName="DoubleTapped" Command="{Binding OpenCommand}"/>
  </EventToCommand.Bindings>
</Border>
```

| API | Description |
|-----|-------------|
| `EventToCommand.Bindings` / `EventBinding` | Multiple event → command mappings |
| `EventBinding.Command` | `ICommand` (usual `{Binding}`) |
| `EventName` / `Command` / … on control | Shorthand for one event |

Shorthand and `Bindings` can be used together. Prefer Avalonia routed events (`Click` → `ClickEvent`).

## API Summary

| API | Description |
|-----|-------------|
| `AddMvvmTransient<TView, TViewModel>()` | `AddTransient<TViewModel>()` + mapping |
| `AddMvvmSingleton<TView, TViewModel>()` | `AddSingleton<TViewModel>()` + mapping |
| `CreateShell<TWindow, TView>()` | Create shell with `new` by platform lifetime |
| `CrystalApplication.Mvvm` | Per-app mappings and `ServiceProvider` |
| `CrystalApplication.Options` / `ConfigureOptions` | Per-app `CrystalOptions` (also DI singleton) |
| `Options.EnableViewLocator` | Whether to register ViewModel-first `ViewLocator` |
| `EventToCommand` | Event → `ICommand` (shorthand + `Bindings` / `EventBinding`) |
| `ILifecycleAware` | Optional load/unload hooks |

## Further Reading

> **How it works:** [Architecture — MVVM Wiring](../architecture.md#mvvm-wiring) — `ViewModelLocator` vs `ViewLocator`, mapping, and lifecycle binding. [Design Decisions](../architecture.md#design-decisions) — why View is not in DI.
