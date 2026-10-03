# EventToCommand (3.1.1)

Bind View events to ViewModel `ICommand` properties without code-behind event handlers.
`EventToCommand` and `EventBinding` use the default Avalonia XML namespace; no additional
namespace declaration is required. Commands can come from CommunityToolkit.Mvvm or another MVVM library.

## Bind by event name

```xml
<Button Content="Save"
        EventToCommand.EventName="Click"
        EventToCommand.Command="{Binding SaveCommand}"
        EventToCommand.CommandParameter="{Binding SelectedItem}" />
```

In 3.1.1, names resolve through Avalonia's routed event registry, including base control
events. `Click` and `ClickEvent` are equivalent. Names are case-sensitive.
Examples include `Loaded`, `Unloaded`, `SizeChanged`, `TextChanged`, `SelectionChanged`,
`PointerPressed`, and `DoubleTapped`, where the host supports the corresponding event.

For a startup command, bind `Loaded` on the root view:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             EventToCommand.EventName="Loaded"
             EventToCommand.Command="{Binding InitializeCommand}">
    <!-- View content -->
</UserControl>
```

`Loaded` can fire again when a view is reattached. Guard one-time initialization in your
ViewModel when needed. `ILifecycleAware.OnLoadedAsync(bool isFirstLoad)` is also available
for ViewModels wired through Crystal's ViewModelLocator or ViewLocator.

## Supply the routed event directly

```xml
<Button EventToCommand.RoutedEvent="{x:Static Button.ClickEvent}"
        EventToCommand.Command="{Binding SaveCommand}" />
```

Use an appropriate XML namespace prefix for a custom event owner. An explicit
`RoutedEvent` takes precedence over `EventName`, requires no reflective event-field lookup,
and initializes the declaring type when its field is accessed. This is useful for custom
or attached routed events whose owner has not yet registered its events.
Name-based lookup can only find events already registered by the owner or a base type of the host.

## Multiple events and event arguments

```xml
<Button>
    <EventToCommand.Bindings>
        <EventBinding RoutedEvent="{x:Static Button.ClickEvent}"
                      Command="{Binding SaveCommand}"
                      CommandParameter="{Binding SelectedItem}" />
        <EventBinding EventName="PointerPressed"
                      Command="{Binding PressCommand}"
                      PassEventArgs="True" />
    </EventToCommand.Bindings>
</Button>
```

When `PassEventArgs="True"`, the event arguments become the command parameter and
`CommandParameter` is ignored. Otherwise, the configured parameter is used. Commands run
only when `CanExecute(parameter)` returns `true`; a null command does nothing.
Both the shorthand and collection syntax can be used on the same control. If both bind
the same event, both commands execute.

| Property | Purpose |
| --- | --- |
| `EventName` | Name of a registered routed event; ordinary CLR `EventHandler` fallback on JIT only |
| `RoutedEvent` | Explicit routed event; takes precedence over `EventName` |
| `Command` | ViewModel `ICommand` |
| `CommandParameter` | Parameter used when `PassEventArgs` is false |
| `PassEventArgs` | Pass the event arguments instead of the configured parameter |
| `EventToCommand.Bindings` | Collection of `EventBinding` mappings |

Changing an event name or explicit event rebuilds the subscription and removes the previous
handler. Collection additions, removals, and replacement update the handlers; replacing a
collection also releases its change subscription. Commands and parameters are resolved
when the event fires, so updated bindings are used.

## Native AOT and upgrading

```bash
dotnet add package Crystal.Avalonia --version 3.1.1
```

Existing routed event name bindings remain valid. Remove event-field `DynamicDependency`
workarounds previously added for `LoadedEvent`, `TextChangedEvent`, or similar routed events.
No application-side preservation attribute is needed for the registry lookup.

The legacy CLR fallback supports only ordinary `EventHandler` events on JIT runtimes.
It does not support CLR `EventHandler<TEventArgs>` events. Trimmed JIT applications must
preserve their CLR event metadata; Native AOT disables this fallback. An explicit
`RoutedEvent` represents a routed event, not an arbitrary CLR event.

## Diagnose a missing binding

Unresolved events produce an Avalonia warning in the `Crystal.Avalonia` log area.
Configure your application's Avalonia logging sink to include warnings. Check the spelling
and capitalization, that the host supports the event, and that custom owners registered
their events. Prefer an explicit `RoutedEvent` for custom or attached routed events.

See [AOT Compatibility](aot-compatibility.md) and [Release Notes](release-notes.md).
