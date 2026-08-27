# MVVM 模式

## 绑定模式

| 模式 | XAML | View | ViewModel |
|------|------|------|-----------|
| **View-first** | `ViewModelLocator.AutoWireViewModel="True"` | XAML / `new` | 来自 DI |
| **ViewModel-first** | `ContentControl Content="{Binding Vm}"` | 由 `ViewLocator` 创建 | 来自 DI |

`CrystalOptions.EnableViewLocator` 只控制 ViewModel-first（是否把 `ViewLocator` 加入 `DataTemplates`）。只要有映射，AutoWire 始终生效。

## 注册

`AddMvvmTransient` / `AddMvvmSingleton` 将 **ViewModel 注册到 DI**，并在 `CrystalApplication.Mvvm` 上记录 **View 映射**：

```csharp
services.AddMvvmTransient<MainView, MainViewModel>();
services.AddMvvmSingleton<AboutView, AboutViewModel>();
```

Shell 视图（如 `MainWindow` 等）**不在 DI 中** — 使用 `CreateShell`：

```csharp
CreateShell<MainWindow, MainView>();
```

## ILifecycleAware（可选）

每次 Loaded 都会调用 `OnLoadedAsync`。`isFirstLoad` 按 ViewModel **实例**计算。

每次进入都刷新：

```csharp
public partial class MainViewModel : ObservableObject, ILifecycleAware
{
    public Task OnLoadedAsync(bool isFirstLoad) => LoadDataAsync();
    public Task OnUnloaded() => SaveStateAsync();
}
```

只初始化一次（单例 ViewModel 常见）：

```csharp
public Task OnLoadedAsync(bool isFirstLoad)
{
    if (!isFirstLoad) return Task.CompletedTask;
    return LoadDataAsync();
}
```

> Tab / WebView：`OnUnloaded` 在离开可视树时都会触发。

## API 摘要

| API | 说明 |
|-----|------|
| `AddMvvmTransient<TView, TViewModel>()` | `AddTransient<TViewModel>()` + 映射 |
| `AddMvvmSingleton<TView, TViewModel>()` | `AddSingleton<TViewModel>()` + 映射 |
| `CreateShell<TWindow, TView>()` | 按平台生命周期用 `new` 创建 Shell |
| `CrystalApplication.Mvvm` | 每应用一份映射和 `ServiceProvider` |
| `CrystalOptions.EnableViewLocator` | 是否注册 ViewModel-first 的 `ViewLocator` |
| `ILifecycleAware` | 可选的加载/卸载钩子 |

## 延伸阅读

> **工作原理：** [架构原理 — MVVM 绑定](../architecture.md#mvvm-wiring) — `ViewModelLocator` 与 `ViewLocator`、映射及生命周期绑定。[设计决策](../architecture.md#design-decisions) — 为何 View 不在 DI 中。
