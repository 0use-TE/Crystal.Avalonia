# 导航

> 默认情况下 View 不在 DI 中。建议优先使用 ViewModel-first 导航。

## ViewModel-First

```csharp
NavigationHost.Content = serviceProvider.GetRequiredService<MainViewModel>();
```

## View-First

```csharp
NavigationHost.Content = new MainView(); // XAML 中设置 AutoWireViewModel="True"
```

## 导航服务

```csharp
public class NavigationService(ContentControl host, IServiceProvider sp)
{
    public void Navigate<TViewModel>() where TViewModel : class
        => host.Content = sp.GetRequiredService<TViewModel>();
}
```

## 延伸阅读

> **工作原理：** [架构原理 — MVVM 绑定](../architecture.md#mvvm-wiring) — ViewModel-first 导航通过 `ViewLocator` 作为 `IDataTemplate`；View-first 在现有 View 上使用 `ViewModelLocator`。
