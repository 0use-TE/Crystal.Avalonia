# Tutorial: Navigation

This tutorial covers basic navigation patterns in Crystal.Avalonia.

## Simple Navigation

Navigate by resolving views from the service provider:

```csharp
public void NavigateToMain()
{
    var mainView = MvvmManager.ServiceProvider!.GetService<MainView>();
    ContentArea.Content = mainView;
}
```

## Frame/ContentControl Navigation

Use a `ContentControl` as the navigation host:

```xml
<Window>
    <ContentControl Name="NavigationHost"/>
</Window>
```

In code-behind:

```csharp
public void Navigate(Control view)
{
    NavigationHost.Content = view;
}
```

## Navigation Service

Create a reusable navigation service:

```csharp
public interface INavigationService
{
    void Navigate(Control view);
    void Navigate<TView>() where TView : Control;
}

public class NavigationService : INavigationService
{
    private readonly ContentControl _host;

    public NavigationService(ContentControl host)
    {
        _host = host;
    }

    public void Navigate(Control view) => _host.Content = view;

    public void Navigate<TView>() where TView : Control
    {
        var view = MvvmManager.ServiceProvider!.GetRequiredService<TView>();
        _host.Content = view;
    }
}
```

Register in services:

```csharp
services.AddSingleton<INavigationService>(sp =>
    new NavigationService(hostControl));
```

## Next Steps

- [Module Development](module-development.md) - Create reusable modules
- [Dependency Injection](dependency-injection.md) - Advanced DI patterns
