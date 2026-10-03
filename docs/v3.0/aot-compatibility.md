# AOT Compatibility

Crystal.Avalonia supports .NET AOT (Ahead-of-Time) compilation and trimming. Registered routed events work with Native AOT; the legacy CLR event fallback requires JIT and retained metadata.

## What is AOT?

AOT compilation converts your .NET code to native code at build time, rather than at runtime via JIT compilation. This results in:

- Faster startup time
- Reduced memory footprint
- Smaller deployment size (with trimming)
- No JIT overhead

## Crystal.Avalonia AOT Features

### IsAotCompatible

The library is marked as AOT-compatible:

```xml
<PropertyGroup>
    <IsAotCompatible>true</IsAotCompatible>
</PropertyGroup>
```

### DynamicallyAccessedMembers Annotations

All generic methods that use reflection (like `Activator.CreateInstance`) are properly annotated:

```csharp
public static void AddMvvmTransient<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>
    (this IServiceCollection services)
    where TView : Control where TViewModel : class
{
    // ...
}
```

Similarly, `AddMvvmSingleton` also uses these annotations to ensure AOT compatibility.

This tells the trimmer exactly what members are needed at runtime, preventing accidental removal.

### EventToCommand

Starting with **3.1.1**, `EventToCommand.EventName` resolves registered routed events through
`RoutedEventRegistry`, including events inherited from base controls. No event field
reflection or event-field `DynamicDependency` is required for names such as `Loaded`,
`Click`, and `TextChanged`.

For custom or attached events, reference the routed event explicitly:

```xml
<Button EventToCommand.RoutedEvent="{x:Static Button.ClickEvent}"
        EventToCommand.Command="{Binding SaveCommand}" />
```

`EventBinding.RoutedEvent` also works inside `EventToCommand.Bindings`. An explicit event
takes precedence over `EventName` and ensures its declaring type is initialized.
Name-based lookup requires the custom event owner to have registered its event already.

The legacy CLR fallback supports ordinary `EventHandler` events on JIT runtimes only.
Trimmed JIT applications must retain their CLR event metadata. Generic CLR
`EventHandler<TEventArgs>` bindings are not supported by this fallback, and the fallback
is disabled under Native AOT. Use registered routed events for AOT applications.
Failed resolution produces an Avalonia warning in the `Crystal.Avalonia` log area.

See the [EventToCommand guide](event-to-command.md) for parameters, multiple events, and upgrading.

## Publishing with AOT

### Desktop Application

```bash
dotnet publish -c Release -p:PublishAot=true
```

### With Trimming

```bash
dotnet publish -c Release -p:PublishTrimmed=true -p:PublishAot=true
```

### Show Trim Warnings

```bash
dotnet publish -c Release -p:ShowTrimmedWarnings=true
```

## Project Configuration

### Example .csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>WinExe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
        <Nullable>enable</Nullable>
        <IsAotCompatible>true</IsAotCompatible>
    </PropertyGroup>

</Project>
```

## Avalonia Integration

Crystal.Avalonia template already enables compiled bindings by default:

```xml
<AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
```

This means XAML bindings are pre-compiled and do not use reflection at runtime. Combined with `x:DataType`, binding is fully AOT-friendly.

## Troubleshooting

### Trimmed Types Missing

If you see warnings about trimmed types:

1. Add `[DynamicallyAccessedMembers]` to generic method parameters
2. Use `Preserve<>` attribute on needed types
3. Add entries to `rd.xml` file for complex scenarios

### Runtime Missing Types

If types are missing at runtime:

1. Check that all View/ViewModel types are registered
2. Verify module assemblies are not trimmed away
3. Use `PreserveAll` or `PreserveMember` where needed

## Further Reading

- [Architecture — AOT & Trimming](architecture.md#aot--trimming) — compile-time type discovery, no assembly scanning
- [.NET AOT Documentation](https://docs.microsoft.com/dotnet/core/deploying/trimming)
- [Avalonia AOT Tips](https://docs.avaloniaui.net/)
