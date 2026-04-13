# Introduction

Crystal.Avalonia is a lightweight MVVM framework for [Avalonia UI](https://avaloniaui.net/).

## Core Features

- **Modular Architecture** - Organize code into self-contained modules
- **Automatic DI** - Built-in dependency injection via Microsoft.Extensions.DependencyInjection
- **MVVM Binding** - Automatic View/ViewModel wiring with `ViewModelLocator.AutoWireViewModel="True"`
- **AOT Ready** - Full trimming and AOT compilation support

## Quick Example

```csharp
// Register View/ViewModel mapping
services.AddMvvmBindingTransient<MainView, MainViewModel>();
```

```xml
<!-- Enable auto-binding in XAML -->
<UserControl ViewModelLocator.AutoWireViewModel="True">
    <TextBlock Text="{Binding Greeting}"/>
</UserControl>
```

## Architecture

| Component | Description |
|-----------|-------------|
| `CrystalApplication` | Base class with modular support |
| `IModule` | Interface for creating modules |
| `MvvmManager` | View/ViewModel registration |
| `ViewModelLocator` | Auto-binding attached property |

## Next Steps

- [Getting Started](getting-started.md) - Create your first app
- [Migrate from Avalonia](tutorials/migrate-from-avalonia.md) - Convert official template
