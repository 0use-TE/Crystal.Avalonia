# Introduction (v1.2)

> **Legacy documentation** for Crystal.Avalonia **1.2.x**. [Upgrade to v2.0](../v2.0/upgrade-from-1.2.md).

Crystal.Avalonia is a lightweight infrastructure layer for Avalonia applications:

- **Modules** — `IModule` for feature registration
- **DI** — Microsoft.Extensions.DependencyInjection
- **View/ViewModel wiring** — View-first & ViewModel-first
- **AOT** — Trimming-friendly

## Not an MVVM Framework

Use CommunityToolkit.Mvvm, Prism, ReactiveUI, etc. for MVVM primitives.

## Binding Modes

**View-first** — `ViewModelLocator.AutoWireViewModel="True"`:

```xml
<UserControl ViewModelLocator.AutoWireViewModel="True">
    <TextBlock Text="{Binding Greeting}"/>
</UserControl>
```

**ViewModel-first** — bind ViewModel; ViewLocator resolves **View from DI**:

```xml
<ContentControl Content="{Binding MainViewModel}"/>
```

## Registration (v1.2)

`AddMvvm*` registers **both View and ViewModel** in DI:

```csharp
services.AddMvvmTransient<MainView, MainViewModel>();
services.AddMvvmHybrid<SettingsView, SettingsViewModel>();   // View=Transient, VM=Singleton
services.AddMvvmSingleton<AboutView, AboutViewModel>();
```

| Method | View | ViewModel |
|--------|------|-----------|
| `AddMvvmTransient` | Transient | Transient |
| `AddMvvmHybrid` | Transient | Singleton |
| `AddMvvmSingleton` | Singleton | Singleton |

## Next Steps

- [Getting Started](getting-started.md)
- [MVVM Pattern](tutorials/mvvm-pattern.md)
