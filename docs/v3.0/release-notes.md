# Release Notes

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

## 3.0.0

- Instance `MvvmManager` on `CrystalApplication.Mvvm`
- `EnableViewLocator` (ViewModel-first `DataTemplates` only)
- `ILifecycleAware.OnLoadedAsync(bool isFirstLoad)`

See [Upgrade from 2.0](upgrade-from-2.0.md).
