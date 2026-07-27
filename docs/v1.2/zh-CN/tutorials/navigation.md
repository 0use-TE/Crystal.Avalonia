# 导航（v1.2）

> **v2.0+** 请参阅 [v2.0 导航](~/docs/v2.0/zh-CN/tutorials/navigation.md)。

通过 `AddMvvm*` 将 View 注册到 DI。

## 从 DI 解析 View

```csharp
NavigationHost.Content = serviceProvider.GetRequiredService<MainView>();
```

## ViewModel-First

```csharp
NavigationHost.Content = serviceProvider.GetRequiredService<MainViewModel>();
// ViewLocator 从 DI 创建 View 并设置 DataContext
```

## 导航服务

```csharp
public class NavigationService(ContentControl host, IServiceProvider sp)
{
    public void Navigate<TView>() where TView : Control
        => host.Content = sp.GetRequiredService<TView>();

    public void Navigate<TViewModel>() where TViewModel : class
        => host.Content = sp.GetRequiredService<TViewModel>();
}
```
