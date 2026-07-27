# MVVM 模式

## 绑定模式

| 模式 | XAML | View | ViewModel |
|------|------|------|-----------|
| **View-first** | `ViewModelLocator.AutoWireViewModel="True"` | XAML / DI shell / `new` | 来自 DI |
| **ViewModel-first** | `ContentControl Content="{Binding Vm}"` | 由 `ViewLocator` 创建 | 来自 DI |

## 注册

`AddMvvmTransient` / `AddMvvmSingleton` 将 **ViewModel 注册到 DI**，并仅记录 **View 映射**：

```csharp
services.AddMvvmTransient<MainView, MainViewModel>();
services.AddMvvmSingleton<AboutView, AboutViewModel>();
```

Shell 视图（如 `MainWindow` 等）**不在 DI 中** — 使用 `CreateShell`：

```csharp
CreateShell<MainWindow, MainView>();
```

## ILifecycleAware（可选）

在 ViewModel 上实现，可自动触发 `OnLoadedAsync` / `OnUnloaded`：

```csharp
public partial class MainViewModel : ObservableObject, ILifecycleAware
{
    public Task OnLoadedAsync() => LoadDataAsync();
    public Task OnUnloaded() => SaveStateAsync();
}
```

> Tab / WebView：`OnUnloaded` 在离开可视树时触发 — 若需要缓存，请在应用中自行管理。

## API 摘要

| API | 说明 |
|-----|------|
| `AddMvvmTransient<TView, TViewModel>()` | `AddTransient<TViewModel>()` + 映射 |
| `AddMvvmSingleton<TView, TViewModel>()` | `AddSingleton<TViewModel>()` + 映射 |
| `CreateShell<TWindow, TView>()` | 按平台生命周期用 `new` 创建 Shell |
| `ILifecycleAware` | 可选的加载/卸载钩子 |

## 延伸阅读

> **工作原理：** [架构原理 — MVVM 绑定](../architecture.md#mvvm-wiring) — `ViewModelLocator` 与 `ViewLocator`、映射字典及生命周期绑定。[设计决策](../architecture.md#design-decisions) — 为何 View 不在 DI 中。
