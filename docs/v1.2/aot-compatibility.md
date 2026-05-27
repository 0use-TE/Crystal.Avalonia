# AOT Compatibility (v1.2)

Same as current — Crystal.Avalonia is AOT-compatible with `[DynamicallyAccessedMembers]` on `AddMvvmTransient`, `AddMvvmHybrid`, and `AddMvvmSingleton`.

See [v2.0 AOT Compatibility](../v2.0/aot-compatibility.md) for full details.

```bash
dotnet publish -c Release -p:PublishAot=true
```
