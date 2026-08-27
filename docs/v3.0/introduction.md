# Introduction (3.0.0)

Crystal.Avalonia is a lightweight infrastructure layer for Avalonia applications:

- **Modules** — `IModule` for feature registration
- **DI** — Microsoft.Extensions.DependencyInjection
- **View/ViewModel wiring** — View-first & ViewModel-first
- **AOT** — Trimming-friendly

## Not an MVVM Framework

No ViewModel base classes, commands, or bindings — use CommunityToolkit.Mvvm, Prism, ReactiveUI, etc.

## Binding Modes

**View-first** — XAML sets `ViewModelLocator.AutoWireViewModel="True"`, ViewModel resolved from DI:

```xml
<UserControl ViewModelLocator.AutoWireViewModel="True">
    <TextBlock Text="{Binding Greeting}"/>
</UserControl>
```

**ViewModel-first** — bind ViewModel to `ContentControl`, ViewLocator creates View:

```xml
<ContentControl Content="{Binding MainViewModel}"/>
```

## Registration

```csharp
services.AddMvvmTransient<MainView, MainViewModel>(); // ViewModel → DI, View → mapping only
// Shell: CreateShell<MainWindow, MainView>() — not from DI
```

| Component | Role |
|-----------|------|
| `CrystalApplication` | App base with module/DI bootstrap |
| `CrystalApplication.Mvvm` | Per-app View↔VM mappings and `ServiceProvider` |
| `CreateShell<TWindow, TView>()` | Shell via `new`; ViewModel wired by ViewModelLocator |
| `AddMvvmTransient` / `AddMvvmSingleton` | ViewModel lifetime + View mapping |
| `ViewModelLocator` | View-first DataContext injection |
| `ViewLocator` | ViewModel-first View creation (`EnableViewLocator`) |

## Next Steps

- [Upgrade Guide](upgrade.md) — migrate from 2.0.x or v1.2
- [Architecture](architecture.md) — how it works internally
- [Getting Started](getting-started.md)
- [MVVM Pattern](tutorials/mvvm-pattern.md)
