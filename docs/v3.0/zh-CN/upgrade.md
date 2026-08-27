# 升级指南

当前版本：**Crystal.Avalonia 3.0.0**

**从 3.0.0 起的版本约定：** `3.0.x` 只修缺陷；新抽象走 `3.1+`；破坏性变更走 `4.0`。（2.0.1 曾在补丁号删除 `CreateShellFromDi` —— 不会再这样做。）

| 来源 | 指南 |
|------|------|
| **2.0.x** | [从 2.0 升级](upgrade-from-2.0.md) — 实例 `MvvmManager`、`EnableViewLocator`、`OnLoadedAsync(bool)` |
| **2.0.0** | 先按 [从 2.0.0 升级](upgrade-from-2.0.0.md)，再升 3.0 |
| **1.2.x** | [从 v1.2 升级](upgrade-from-1.2.md)，再 [从 2.0 升级](upgrade-from-2.0.md) |

## 快速参考（3.0.0）

```csharp
public override void RegisterServices(IServiceCollection services)
{
    services.AddMvvmTransient<MainView, MainViewModel>();
}

public override void CreateShell(IServiceProvider sp)
{
    CreateShell<MainWindow, MainView>();
}
```

- **ViewModel** → DI（`AddMvvm*`）
- **View** → 映射在 `CrystalApplication.Mvvm`；由 XAML 或 `ViewLocator` 创建
- **Shell** → `CreateShell<...>()` 使用 `new`，不进 DI
- **`CrystalOptions.EnableViewLocator`** → 只控制 ViewModel-first 的 `DataTemplates`
