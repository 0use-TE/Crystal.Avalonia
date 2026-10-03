# Release Notes

## 3.1.1

### EventToCommand: trimming and Native AOT fix

`EventName` now resolves registered routed events through Avalonia's `RoutedEventRegistry`,
including events inherited from base controls. It no longer reflects over public `*Event`
fields. This fixes commands such as startup `Loaded` and receive editor `TextChanged`
failing silently when their field metadata is trimmed.

- Existing names such as `Loaded`, `Click`, and `ClickEvent` remain valid.
- New `EventToCommand.RoutedEvent` and `EventBinding.RoutedEvent` properties accept explicit
  routed events and take precedence over `EventName`.
- Unsupported bindings produce an Avalonia warning in the `Crystal.Avalonia` log area.
- Replacing binding collections now detaches the previous collection change handler.
- The legacy CLR `EventHandler` fallback runs only on JIT runtimes. It requires retained
  event metadata when trimming and is disabled under Native AOT.

Upgrade with `dotnet add package Crystal.Avalonia --version 3.1.1`. Existing routed event
XAML does not need to change, and event-field `DynamicDependency` workarounds can be removed.
For custom or attached events, prefer explicit `RoutedEvent` references. Custom owners
must register their events before using name-based resolution.

See [EventToCommand](event-to-command.md) and [AOT Compatibility](aot-compatibility.md).
The documentation retains the existing `docs/v3.0/` URLs for the 3.x series.
CrystalTemplate has a separate version and remains at 3.1.0 in this release.

## 3.1.0

### CrystalOptions (instance + DI)

`CrystalOptions` is no longer a static class. Configure via `ConfigureOptions` and inject `CrystalOptions` from DI:

```csharp
public override void ConfigureOptions(CrystalOptions options)
{
    options.EnableViewLocator = true; // default
}
```

See [Upgrade from 3.0](upgrade-from-3.0.md).

### EventToCommand

Lightweight event → `ICommand` wiring in the default Avalonia xmlns (no extra prefix):

```xml
<!-- Single -->
<Button EventToCommand.EventName="Click"
        EventToCommand.Command="{Binding SaveCommand}"/>

<!-- Multiple -->
<Button>
  <EventToCommand.Bindings>
    <EventBinding EventName="Click" Command="{Binding PingCommand}"/>
    <EventBinding EventName="DoubleTapped" Command="{Binding OpenCommand}"/>
  </EventToCommand.Bindings>
</Button>
```

### Docs site

- Live WASM demo published to `/demo/` alongside DocFX
- Upgrade guides nested under **Upgrade Guide** in the sidebar
- **CrystalTemplate 3.1.0** — `ConfigureOptions`, `EventToCommand` sample, package reference `Crystal.Avalonia` 3.1.0

## 3.0.0

- Instance `MvvmManager` on `CrystalApplication.Mvvm`
- `EnableViewLocator` (ViewModel-first `DataTemplates` only)
- `ILifecycleAware.OnLoadedAsync(bool isFirstLoad)`

See [Upgrade from 2.0](upgrade-from-2.0.md).
