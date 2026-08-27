# 从 v1.2 升级

## v2.0 破坏性变更

| v1.2 | v2.0 |
|------|------|
| `AddMvvm*` 将 **View + ViewModel** 都注册进 DI | 仅 **ViewModel** 进 DI；`TView` 只做映射 |
| 提供 `AddMvvmHybrid` | **已移除** — 使用 `AddMvvmTransient` 或 `AddMvvmSingleton` |
| `ViewLocator` 从 DI 解析 View | `ViewLocator` 使用 `Activator.CreateInstance` |
| `CreateShell`: `new MainWindow()` | `CreateShell<MainWindow, MainView>()`（2.0.1+，仍是 `new`，不进 DI） |

## 迁移步骤

### 1. 替换 `AddMvvmHybrid`

```csharp
// v1.2
services.AddMvvmHybrid<SettingsView, SettingsViewModel>();

// v2.0 — 为 ViewModel 选择一种生命周期
services.AddMvvmSingleton<SettingsView, SettingsViewModel>();
// 或
services.AddMvvmTransient<SettingsView, SettingsViewModel>();
```

### 2. Shell 创建

```csharp
public override void CreateShell(IServiceProvider sp)
{
    CreateShell<MainWindow, MainView>();
}
```

不要再 `AddTransient<MainWindow>()` — Shell 视图不进 DI（2.0.1+）。

> 从 **2.0.0** 升级？见 [从 2.0.0 升级](upgrade-from-2.0.0.md)。升到 **3.0** 还须完成 [从 2.0 升级](upgrade-from-2.0.md)（`ILifecycleAware` 签名、`EnableViewLocator`、实例 `MvvmManager`）。

### 3. 导航

```csharp
// v1.2 — 从 DI 取 View
var view = sp.GetRequiredService<MainView>();

// v2.0 — 推荐 ViewModel-first
NavigationHost.Content = sp.GetRequiredService<MainViewModel>();
// 或 View-first: new MainView() 并启用 AutoWireViewModel
```

## 未改动（相对 v1.2 → 2.0）

- `IModule` / 模块系统
- `ViewModelLocator` / View-first 绑定
- AOT 注解

`ILifecycleAware` 在 **3.0** 有签名变化，见 [从 2.0 升级](upgrade-from-2.0.md)。
