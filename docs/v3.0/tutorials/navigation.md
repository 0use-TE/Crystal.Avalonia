# Navigation

> Views are not in DI by default. Prefer ViewModel-first navigation.

## ViewModel-First

```csharp
NavigationHost.Content = serviceProvider.GetRequiredService<MainViewModel>();
```

## View-First

```csharp
NavigationHost.Content = new MainView(); // AutoWireViewModel="True" in XAML
```

## Navigation Service

```csharp
public class NavigationService(ContentControl host, IServiceProvider sp)
{
    public void Navigate<TViewModel>() where TViewModel : class
        => host.Content = sp.GetRequiredService<TViewModel>();
}
```

## Further Reading

> **How it works:** [Architecture — MVVM Wiring](../architecture.md#mvvm-wiring) — ViewModel-first navigation uses `ViewLocator` as `IDataTemplate`; View-first uses `ViewModelLocator` on existing Views.
