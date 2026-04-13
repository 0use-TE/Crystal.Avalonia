# Introduction

Crystal.Avalonia is a lightweight MVVM framework designed specifically for [Avalonia UI](https://avaloniaui.net/) applications. It provides a clean architecture pattern that makes building complex, cross-platform applications simpler and more maintainable.

## Why Crystal.Avalonia?

When building large-scale Avalonia applications, you often face challenges like:

- **Code organization** - How to keep your code maintainable as the app grows
- **View/ViewModel binding** - Managing the relationship between views and their view models
- **Modularity** - How to split your app into independent, testable units
- **Dependency Injection** - Properly wiring up services across your application

Crystal.Avalonia addresses all of these concerns out of the box.

## Core Concepts

### 1. CrystalApplication

The base application class that extends Avalonia's `Application` with modular development support:

```csharp
public class MyApp : CrystalApplication
{
    public override void RegisterModules(IModuleRegistrar moduleRegistrar) { }
    public override void RegisterServices(IServiceCollection services) { }
    public override void CreateShell(IServiceProvider serviceProvider) { }
}
```

### 2. Module System

Modules are self-contained units of functionality that manage their own services and views:

```csharp
public class MyModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        // Register module services here
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        // Initialize module here
    }
}
```

### 3. MVVM Binding

Crystal.Avalonia provides simple yet powerful View/ViewModel binding:

```csharp
// Register the mapping
services.AddMvvmBindingTransient<MainView, MainViewModel>();

// In XAML, enable auto-binding
<Window xmlns:vm="using:Crystal.Avalonia"
        vm:ViewModelLocator.AutoWireViewModel="True">
```

## Architecture Overview

```
┌─────────────────────────────────────────────────────┐
│                  CrystalApplication                  │
├─────────────────────────────────────────────────────┤
│  RegisterModules()   - Register IModule impls       │
│  RegisterServices()  - Register DI services         │
│  CreateShell()       - Create main window/view      │
└─────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│   Module A    │    │   Module B    │    │  Main App     │
├───────────────┤    ├───────────────┤    ├───────────────┤
│ • Services    │    │ • Services    │    │ • ViewModels  │
│ • Views       │    │ • Views       │    │ • Views       │
│ • ViewModels  │    │ • ViewModels  │    │ • Services    │
└───────────────┘    └───────────────┘    └───────────────┘
```

## Key Features

| Feature | Description |
|---------|-------------|
| **Modular Architecture** | Organize code into independent modules with their own services and views |
| **Automatic DI Wiring** | ViewModels and Views are automatically registered and resolved |
| **Flexible Binding** | Two binding modes: `AddMvvmBindingTransient` and `AddMvvmBindingSingleton` |
| **AOT Ready** | Full support for trimming and AOT compilation |
| **Cross-Platform** | Supports all Avalonia platforms including WASM |

## Next Steps

- [Getting Started](getting-started.md) - Create your first Crystal.Avalonia application
- [API Reference](api/index.md) - Explore the full API documentation
