# Introduction

Crystal.Avalonia is a lightweight infrastructure layer for Avalonia UI applications. It provides:

- **Module System** - Organize your app into independent, self-contained modules
- **Dependency Injection** - Built-in support via Microsoft.Extensions.DependencyInjection
- **View/ViewModel Wiring** - Two binding modes: View-first and ViewModel-first
- **AOT Ready** - Full trimming and AOT compilation support

## What Crystal.Avalonia Is NOT

Crystal.Avalonia is **not** an MVVM framework. It does not provide:
- ViewModel base classes (use CommunityToolkit.Mvvm, Prism, ReactiveUI, etc.)
- Command implementations (use your preferred MVVM library)
- Data binding implementations (Avalonia handles this)

You are free to use **any** MVVM library with Crystal.Avalonia.

## Two Binding Modes

### Mode 1: ViewModelLocator (View-First)

View 在 XAML 中设置 `ViewModelLocator.AutoWireViewModel="True"`，系统自动从 DI 解析 ViewModel 并注入 DataContext：

```xml
<UserControl ViewModelLocator.AutoWireViewModel="True">
    <TextBlock Text="{Binding Greeting}"/>
</UserControl>
```

### Mode 2: ViewLocator (ViewModel-First)

直接在 ContentControl 或 ItemsControl 绑定 ViewModel，ViewLocator 自动从 DI 解析对应 View：

```xml
<ContentControl Content="{Binding MainViewModel}"/>
```

ViewLocator is enabled by default (controlled by `CrystalOptions.EnableViewModelLocator = true`).

## Architecture

| Component | Description |
|-----------|-------------|
| `CrystalApplication` | Base class with module and DI support |
| `IModule` | Interface for creating modules |
| `MvvmManager` | View/ViewModel registration |
| `ViewModelLocator` | Auto-binding attached property (View-first) |
| `ViewLocator` | IDataTemplate implementation (ViewModel-first, built-in) |

## Next Steps

- [Getting Started](getting-started.md) - Create your first app
- [Migrate from Avalonia](tutorials/migrate-from-avalonia.md) - Convert official template
