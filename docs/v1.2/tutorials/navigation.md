# Navigation (v1.2)

> For **v2.0+**, see [v2.0 Navigation](~/docs/v2.0/tutorials/navigation.md).

Views are registered in DI via `AddMvvm*`.

## Resolve View from DI

```csharp
NavigationHost.Content = serviceProvider.GetRequiredService<MainView>();
```

## ViewModel-First

```csharp
NavigationHost.Content = serviceProvider.GetRequiredService<MainViewModel>();
// ViewLocator creates View from DI and sets DataContext
```

## Navigation Service

```csharp
public class NavigationService(ContentControl host, IServiceProvider sp)
{
    public void Navigate<TView>() where TView : Control
        => host.Content = sp.GetRequiredService<TView>();

    public void Navigate<TViewModel>() where TViewModel : class
        => host.Content = sp.GetRequiredService<TViewModel>();
}
```
