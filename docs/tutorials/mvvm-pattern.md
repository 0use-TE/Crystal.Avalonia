# Tutorial: MVVM with Crystal.Avalonia

This tutorial explains the two MVVM binding modes in Crystal.Avalonia.

## Two Binding Modes

Crystal.Avalonia provides two ways to bind Views and ViewModels:

| Mode | Description | Use Case |
|------|-------------|----------|
| **ViewModelLocator** | View sets `AutoWireViewModel="True"`, system injects DataContext | View-first development |
| **ViewLocator** | ViewModel is assigned to ContentControl, system resolves View from DI | ViewModel-first development |

## Mode 1: ViewModelLocator (View-First)

### Register

```csharp
public override void RegisterServices(IServiceCollection services)
{
    services.AddMvvmBindingTransient<MainView, MainViewModel>();
}
```

### XAML

```xml
<UserControl xmlns:vm="using:Crystal.Avalonia"
             ViewModelLocator.AutoWireViewModel="True">
    <TextBlock Text="{Binding Title}"/>
</UserControl>
```

### How It Works

1. View loads with `ViewModelLocator.AutoWireViewModel="True"`
2. System looks up the registered ViewModel type for this View
3. Resolves ViewModel from DI container
4. Assigns to `DataContext`

## Mode 2: ViewLocator (ViewModel-First)

ViewLocator is a built-in `IDataTemplate` that automatically resolves View from ViewModel type.

### Register

```csharp
public override void RegisterServices(IServiceCollection services)
{
    services.AddMvvmBindingTransient<MainView, MainViewModel>();
}
```

### XAML

```xml
<!-- No extra attributes needed -->
<ContentControl Content="{Binding MainViewModel}"/>
```

### How It Works

1. ViewModel is assigned to ContentControl
2. ViewLocator matches the ViewModel type against registered mappings
3. Resolves View from DI container
4. Creates View with ViewModel as DataContext

ViewLocator is enabled by default (controlled by `CrystalOptions.EnableViewModelLocator`).

## Example with CommunityToolkit.Mvvm

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "My App";
}
```

### ViewModelLocator (View-First)

```xml
<UserControl ViewModelLocator.AutoWireViewModel="True">
    <TextBlock Text="{Binding Title}"/>
</UserControl>
```

### ViewLocator (ViewModel-First)

```xml
<ContentControl Content="{Binding MyViewModel}"/>
```

Same ViewModel works with both modes.

## Key Concepts

| Concept | Description |
|---------|-------------|
| `AddMvvmBindingTransient` | Registers View and ViewModel types, builds bidirectional mapping |
| `ViewModelLocator.AutoWireViewModel` | Attached property for View-first auto-binding |
| `ViewLocator` | Built-in IDataTemplate for ViewModel-first resolution |
| `MvvmManager.GetVmType(viewType)` | Looks up ViewModel type for a View type |
| `MvvmManager.GetViewType(vmType)` | Looks up View type for a ViewModel type |

## Next Steps

- [Dependency Injection](dependency-injection.md) - Learn about DI in Crystal.Avalonia
- [Module Development](module-development.md) - Organize code into modules
