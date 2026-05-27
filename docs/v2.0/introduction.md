# Introduction (v2.0)

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
services.AddTransient<MainWindow>();                  // Shell views → register manually in DI
```

| Component | Role |
|-----------|------|
| `CrystalApplication` | App base with module/DI bootstrap |
| `AddMvvmTransient` / `AddMvvmSingleton` | ViewModel lifetime + View mapping |
| `ViewModelLocator` | View-first DataContext injection |
| `ViewLocator` | ViewModel-first View creation |

## Next Steps

- [Architecture](architecture.md) — how it works internally
- [Upgrade from v1.2](upgrade-from-1.2.md)
- [Getting Started](getting-started.md)
- [MVVM Pattern](tutorials/mvvm-pattern.md)
