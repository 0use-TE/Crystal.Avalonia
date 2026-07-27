# 升级指南

当前版本：**Crystal.Avalonia 2.0.1**

| 来源 | 指南 |
|------|------|
| **2.0.0** | [从 2.0.0 升级](upgrade-from-2.0.0.md) — 移除 `CreateShellFromDi`，Shell 不再进 DI |
| **1.2.x** | [从 v1.2 升级](upgrade-from-1.2.md) — 仅 ViewModel 进 DI，无 `AddMvvmHybrid` |

## 快速参考（2.0.1）

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
- **View** → 仅映射；由 XAML 或 `ViewLocator` 创建
- **Shell**（`MainWindow` / `MainView`）→ `CreateShell<...>()` 使用 `new`，不进 DI
