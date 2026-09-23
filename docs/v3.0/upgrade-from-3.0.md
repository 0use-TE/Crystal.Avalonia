# Upgrade from 3.0

Crystal.Avalonia **3.1.0** moves `CrystalOptions` from a static class to a per-app instance registered in DI.

## What Changed

| 3.0.0 | 3.1.0 |
|-------|-------|
| `static class CrystalOptions` / `CrystalOptions.EnableViewLocator = …` | Instance on `CrystalApplication.Options`; configure via `ConfigureOptions` |
| Options not in DI | `CrystalOptions` registered as singleton before `RegisterServices` |

## Migration

```csharp
// 3.0
CrystalOptions.EnableViewLocator = false;

// 3.1
public override void ConfigureOptions(CrystalOptions options)
{
    options.EnableViewLocator = false;
}
```

Inject where needed:

```csharp
public class MyService(CrystalOptions options)
{
    // options.EnableViewLocator
}
```

Or read `CrystalApplication.Options` / resolve from `IServiceProvider`.

## Unchanged

- `AddMvvmTransient` / `AddMvvmSingleton`
- `CreateShell<TWindow, TView>()`
- `ViewModelLocator.AutoWireViewModel`
- Module pipeline and AOT annotations

## Also in 3.1

- **`EventToCommand`** — lightweight event → `ICommand` (default Avalonia xmlns); use `Bindings` / `EventBinding` for multiple events
- **WASM live demo** on the docs site at `/demo/`
