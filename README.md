# Crystal.Avalonia

A lightweight infrastructure layer for Avalonia UI applications with modular architecture and dependency injection.

[![NuGet](https://img.shields.io/nuget/v/Crystal.Avalonia.svg)](https://www.nuget.org/packages/Crystal.Avalonia)
[![AOT Compatible](https://img.shields.io/badge/AOT-Compatible-brightgreen)](https://0use.net/Crystal.Avalonia/docs/aot-compatibility.html)

## What It Provides

- **Module System** - Organize code into independent, self-contained modules
- **Dependency Injection** - Built-in support via Microsoft.Extensions.DependencyInjection
- **View/ViewModel Wiring** - Simple registration with automatic DataContext injection
- **AOT Friendly** - Full trimming and AOT compilation support
- **Cross-Platform** - Works with all Avalonia-supported platforms (Windows, macOS, Linux, Android, iOS, WebAssembly)

## What It's NOT

Crystal.Avalonia is **not** an MVVM framework. It does not provide ViewModel base classes or commands. You can use **any** MVVM library:

- CommunityToolkit.Mvvm
- Prism
- ReactiveUI
- Any other

## Quick Start

### Install the Template

```bash
dotnet new install Crystal.Avalonia.Template
```

### Create a New Project

```bash
dotnet new crystal.avalonia -o MyApp
cd MyApp
dotnet run
```

## Documentation

- [Introduction](https://0use.net/Crystal.Avalonia/docs/introduction.html) - Overview of Crystal.Avalonia
- [Getting Started](https://0use.net/Crystal.Avalonia/docs/getting-started.html) - Step-by-step tutorial
- [API Documentation](https://0use.net/Crystal.Avalonia/api/) - Generated API reference

## AOT & Trimming Support

Crystal.Avalonia is fully compatible with .NET trimming and AOT compilation:

- `IsAotCompatible=true` - The library is annotated for AOT compatibility
- Properly annotated `[DynamicallyAccessedMembers]` for reflection-heavy operations
- No dynamic assembly scanning or runtime type discovery

See [AOT Compatibility](https://0use.net/Crystal.Avalonia/docs/aot-compatibility.html) for more details.

## License

MIT License
