# AOT Compatibility

Crystal.Avalonia is fully compatible with .NET AOT (Ahead-of-Time) compilation and trimming.

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
public static void AddMvvmBindingTransient<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>
    (this IServiceCollection services)
    where TView : Control where TViewModel : class
{
    // ...
}
```

This tells the trimmer exactly what members are needed at runtime, preventing accidental removal.

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

- [.NET AOT Documentation](https://docs.microsoft.com/dotnet/core/deploying/trimming)
- [Avalonia AOT Tips](https://docs.avaloniaui.net/)
