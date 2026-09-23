# 从 3.0 升级

Crystal.Avalonia **3.1.0** 将 `CrystalOptions` 从静态类改为每应用一份实例，并注册到 DI。

## 变更

| 3.0.0 | 3.1.0 |
|-------|-------|
| `static class CrystalOptions` / `CrystalOptions.EnableViewLocator = …` | `CrystalApplication.Options`；通过 `ConfigureOptions` 配置 |
| Options 不在 DI 中 | `RegisterServices` 之前已注册为单例 |

## 迁移

```csharp
// 3.0
CrystalOptions.EnableViewLocator = false;

// 3.1
public override void ConfigureOptions(CrystalOptions options)
{
    options.EnableViewLocator = false;
}
```

需要时注入：

```csharp
public class MyService(CrystalOptions options)
{
    // options.EnableViewLocator
}
```

也可读 `CrystalApplication.Options`，或从 `IServiceProvider` 解析。

## 未变

- `AddMvvmTransient` / `AddMvvmSingleton`
- `CreateShell<TWindow, TView>()`
- `ViewModelLocator.AutoWireViewModel`
- 模块流水线与 AOT 注解

## 3.1 同版本还包含

- **`EventToCommand`** — 轻量事件 → `ICommand`（默认 Avalonia xmlns）；多事件用 `Bindings` / `EventBinding`
- 文档站 **WASM 在线 demo**：`/demo/`
