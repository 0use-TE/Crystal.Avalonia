# MVVM 模式（v1.2）

> **v2.0+** 请参阅 [v2.0 MVVM 模式](~/docs/v2.0/zh-CN/tutorials/mvvm-pattern.md)。

## 注册模式

| 方法 | View 在 DI 中 | ViewModel 在 DI 中 |
|--------|------------|-----------------|
| `AddMvvmTransient` | `AddTransient<TView>()` | `AddTransient<TViewModel>()` |
| `AddMvvmHybrid` | `AddTransient<TView>()` | `AddSingleton<TViewModel>()` |
| `AddMvvmSingleton` | `AddSingleton<TView>()` | `AddSingleton<TViewModel>()` |

```csharp
services.AddMvvmTransient<MainView, MainViewModel>();
services.AddMvvmHybrid<SettingsView, SettingsViewModel>();
services.AddMvvmSingleton<AboutView, AboutViewModel>();
```

## ViewModel-First

ViewLocator 从 DI 解析 View：

```csharp
// ContentControl.Content = viewModelInstance
// → ViewLocator → sp.GetService<TView>()
```

## ILifecycleAware

```csharp
public partial class MainViewModel : ObservableObject, ILifecycleAware
{
    public Task OnLoadedAsync() => LoadDataAsync();
    public Task OnUnloaded() => SaveStateAsync();
}
```

View 加载/卸载时自动挂钩。
