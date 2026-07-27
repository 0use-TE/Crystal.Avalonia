# 从 2.0.0 升级

Crystal.Avalonia **2.0.1** 简化了 Shell 创建：Shell 视图不再从 DI 解析。

## 变更说明

| 2.0.0 | 2.0.1 |
|-------|-------|
| `CreateShellFromDi<TWindow, TView>(sp)` | **已移除** — 使用 `CreateShell<TWindow, TView>()` |
| `services.AddTransient<MainWindow>()` | **不需要** — Shell 用 `new` 创建 |
| `services.AddTransient<MainView>()` | **不需要** |

Shell 视图（`MainWindow`、`MainView`）通常无构造依赖。其中的 ViewModel 仍通过 `ViewModelLocator.AutoWireViewModel="True"` 从 DI 获取。

## 迁移步骤

### 之前（2.0.0）

```csharp
public override void RegisterServices(IServiceCollection services)
{
    services.AddMvvmTransient<MainView, MainViewModel>();
    services.AddTransient<MainWindow>();
    services.AddTransient<MainView>();
}

public override void CreateShell(IServiceProvider sp)
{
    CreateShellFromDi<MainWindow, MainView>(sp);
}
```

### 之后（2.0.1）

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

### 包 / 模板

```bash
dotnet add package Crystal.Avalonia --version 2.0.1
dotnet new install CrystalTemplate::2.0.1
```

## 若 Shell 需要构造注入

较少见。手动重写 `CreateShell`：

```csharp
public override void CreateShell(IServiceProvider sp)
{
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        desktop.MainWindow = sp.GetRequiredService<MainWindow>();
}
```

仅在此情况下才把 `MainWindow` 注册进 DI。

## 2.0.1 未改动部分

- 仅 ViewModel 进 DI（`AddMvvmTransient` / `AddMvvmSingleton`）
- `ViewModelLocator` / `ViewLocator`
- `ILifecycleAware`、模块系统、AOT 支持
