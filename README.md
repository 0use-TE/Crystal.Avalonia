# Crystal.Avalonia

A lightweight infrastructure layer for Avalonia UI applications with modular architecture and dependency injection.

Thanks to the community, Crystal.Avalonia is feature-complete and mature enough for commercial use. We appreciate your support!

[![NuGet](https://img.shields.io/nuget/v/Crystal.Avalonia.svg)](https://www.nuget.org/packages/Crystal.Avalonia)
[![AOT Compatible](https://img.shields.io/badge/AOT-Compatible-brightgreen)](https://0use.net/Crystal.Avalonia/docs/v3.0/aot-compatibility.html)

## What It Provides

- **Module System** - Organize code into independent, self-contained modules
- **Dependency Injection** - Built-in support via Microsoft.Extensions.DependencyInjection
- **View/ViewModel Wiring** - AutoWire, ViewLocator, and lightweight `EventToCommand` (default Avalonia xmlns)
- **AOT Friendly** - Trimming and Native AOT support for registered routed event bindings
- **Cross-Platform** - Works with all Avalonia-supported platforms (Windows, macOS, Linux, Android, iOS, WebAssembly)

## What It's NOT

Crystal.Avalonia is **not** an MVVM framework. It does not provide ViewModel base classes or commands. You can use **any** MVVM library:

- CommunityToolkit.Mvvm
- Prism
- ReactiveUI
- Any other

The library does not include navigation, regions, or an event aggregator.

## Versioning (3.0+)

- **3.0.x** — bug fixes only
- **3.1+** — new abstractions (3.1 moves `CrystalOptions` to an instance / DI singleton — see [Upgrade from 3.0](https://0use.net/Crystal.Avalonia/docs/v3.0/upgrade-from-3.0.html))
- **4.0** — breaking changes

## Quick Start

### Install the Template

```bash
dotnet new install CrystalTemplate::3.1.1
```

### Create a New Project

```bash
dotnet new CT -o MyApp
cd MyApp
dotnet run
```

## Documentation

- [Live Demo (WASM)](https://0use.net/Crystal.Avalonia/demo/)
- [3.1.1 (Current)](https://0use.net/Crystal.Avalonia/docs/v3.0/introduction.html)
- [EventToCommand](https://0use.net/Crystal.Avalonia/docs/v3.0/event-to-command.html)
- [Release Notes](https://0use.net/Crystal.Avalonia/docs/v3.0/release-notes.html)
- [2.0.1 (Legacy)](https://0use.net/Crystal.Avalonia/docs/v2.0/introduction.html)
- [v1.2 (Legacy)](https://0use.net/Crystal.Avalonia/docs/v1.2/introduction.html)
- [Upgrade Guide](https://0use.net/Crystal.Avalonia/docs/v3.0/upgrade.html)
- [Upgrade from 3.0](https://0use.net/Crystal.Avalonia/docs/v3.0/upgrade-from-3.0.html)
- [Upgrade from 2.0](https://0use.net/Crystal.Avalonia/docs/v3.0/upgrade-from-2.0.html)
- [Upgrade from v1.2](https://0use.net/Crystal.Avalonia/docs/v3.0/upgrade-from-1.2.html)
- [API Reference](https://0use.net/Crystal.Avalonia/api/) — 3.1.0+

## AOT & Trimming Support

Crystal.Avalonia supports .NET trimming and Native AOT. Registered routed events are supported; the legacy CLR event fallback requires JIT and preserved metadata:

- `IsAotCompatible=true` - The library is annotated for AOT compatibility
- Properly annotated `[DynamicallyAccessedMembers]` for reflection-heavy operations
- No dynamic assembly scanning or runtime type discovery

`EventToCommand.EventName` resolves registered Avalonia routed events (including inherited
events) through `RoutedEventRegistry`, without reflecting over `*Event` fields. Existing
names such as `Loaded`, `Click`, and `TextChanged` work with Native AOT.

For custom or attached routed events, you can also supply the event directly:

```xml
<Button EventToCommand.RoutedEvent="{x:Static Button.ClickEvent}"
        EventToCommand.Command="{Binding SaveCommand}" />
```

`EventBinding.RoutedEvent` supports the same syntax inside `EventToCommand.Bindings`.
An explicit `RoutedEvent` takes precedence over `EventName`. Custom event owners must
register their event before resolving it by name; supplying the event directly ensures
the owner is initialized. The legacy CLR `EventHandler` fallback is available only on
JIT runtimes and requires preserved event metadata in trimmed applications. It is disabled
under Native AOT. Unsupported event names produce an Avalonia warning log.

See [AOT Compatibility](https://0use.net/Crystal.Avalonia/docs/v3.0/aot-compatibility.html) for more details.

## 3.1.1 Update

- Fix startup and other routed event commands failing silently after Native AOT publishing.
- Resolve `EventName` through Avalonia's event registry instead of reflection over event fields.
- Add `EventToCommand.RoutedEvent` and `EventBinding.RoutedEvent` for explicit event references.
- Log unsupported event bindings and detach subscriptions when binding collections are replaced.

Update the library package with:

```bash
dotnet add package Crystal.Avalonia --version 3.1.1
```

Existing routed event XAML remains valid. App-specific `DynamicDependency` workarounds
for these event fields can be removed after upgrading. CrystalTemplate 3.1.1 generates
applications referencing Crystal.Avalonia 3.1.1. Reinstalling the template affects newly
generated applications; update existing applications' library references separately.

## License

MIT License
