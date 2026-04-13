# Crystal.Avalonia

A lightweight, AOT-friendly MVVM framework for Avalonia UI applications.

[![NuGet](https://img.shields.io/nuget/v/Crystal.Avalonia.svg)](https://www.nuget.org/packages/Crystal.Avalonia)
[![AOT Compatible](https://img.shields.io/badge/AOT-Compatible-brightgreen)](README.md#aot--trimming-support)

## Features

- **Modular Architecture** - Divide your application into independent, self-contained modules
- **Built-in MVVM Support** - View/ViewModel mapping with automatic DataContext injection
- **Dependency Injection** - First-class DI support via Microsoft.Extensions.DependencyInjection
- **AOT Friendly** - Full trimming and AOT compilation support
- **Cross-Platform** - Works with all Avalonia-supported platforms (Windows, macOS, Linux, Android, iOS, WebAssembly)

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

- [Introduction](docs/introduction.md) - Overview of Crystal.Avalonia
- [Getting Started](docs/getting-started.md) - Step-by-step tutorial
- [API Documentation](api/index.md) - Generated API reference

## AOT & Trimming Support

Crystal.Avalonia is fully compatible with .NET trimming and AOT compilation:

- `IsAotCompatible=true` - The library is annotated for AOT compatibility
- Properly annotated `[DynamicallyAccessedMembers]` for reflection-heavy operations
- No dynamic assembly scanning or runtime type discovery

See [AOT Compatibility](docs/aot-compatibility.md) for more details.

## License

MIT License
