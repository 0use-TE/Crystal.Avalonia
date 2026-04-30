# Tutorial: Dependency Injection

Crystal.Avalonia uses Microsoft.Extensions.DependencyInjection for dependency injection. This tutorial covers DI patterns and best practices.

## Basic Concepts

### Constructor Injection (Recommended)

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

public partial class UserListViewModel : ObservableObject
{
    private readonly IUserService _userService;
    private readonly ILogger _logger;

    // Dependencies are injected through the constructor
    public UserListViewModel(IUserService userService, ILogger logger)
    {
        _userService = userService;
        _logger = logger;
    }
}
```

### Property Injection

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

public partial class SettingsViewModel : ObservableObject
{
    public IConfigService? ConfigService { get; set; }
}
```

## Registering Services

### In Module.RegisterServices

```csharp
public class MyModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        // Transient - New instance each time
        services.AddTransient<IUserService, UserService>();

        // Singleton - Same instance shared
        services.AddSingleton<ISettingsService, SettingsService>();

        // Scoped - One instance per scope
        services.AddScoped<IDbContext, AppDbContext>();

        // Instance - Pre-created instance
        services.AddInstance(new AppMetrics());
    }
}
```

### In App.RegisterServices

```csharp
public override void RegisterServices(IServiceCollection services)
{
    // Register application-wide services
    services.AddSingleton<IAppConfiguration, AppConfiguration>();
    services.AddSingleton<INavigationService, NavigationService>();

    // Register View/ViewModel pairs
    services.AddMvvmTransient<MainView, MainViewModel>();
}
```

## Service Lifetimes

| Lifetime | When to Use | Behavior |
|----------|-------------|----------|
| `Transient` | Lightweight, stateless services | New instance each time |
| `Singleton` | Shared state, configuration | Same instance always |
| `Scoped` | Per-operation data (e.g., DB context) | One instance per scope |

### Example: Choosing the Right Lifetime

```csharp
public void RegisterServices(IServiceCollection services)
{
    // Transient: New instance for each request
    // Good for: ViewModels, lightweight services
    services.AddTransient<MainViewModel>();
    services.AddTransient<IIdGenerator, GuidIdGenerator>();

    // Singleton: Shared across the app
    // Good for: Configuration, logging, caching
    services.AddSingleton<AppSettings>();
    services.AddSingleton<ILogger, FileLogger>();

    // Scoped: Per operation/scope
    // Good for: Database contexts
    services.AddScoped<IDbContext>(sp => new MyDbContext());
}
```

## Advanced Patterns

### Factory Pattern

```csharp
public interface IViewModelFactory
{
    TViewModel Create<TViewModel>() where TViewModel : class;
}

public class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TViewModel Create<TViewModel>() where TViewModel : class
    {
        return _serviceProvider.GetRequiredService<TViewModel>();
    }
}

// Registration
services.AddSingleton<IViewModelFactory, ViewModelFactory>();

// Usage
public partial class DetailViewModel : ObservableObject
{
    private readonly IViewModelFactory _factory;

    public DetailViewModel(IViewModelFactory factory)
    {
        _factory = factory;
    }

    public void OpenItem(ItemViewModel item)
    {
        var vm = _factory.Create<ItemDetailViewModel>();
        vm.Load(item);
    }
}
```

### Lazy Resolution

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

public partial class MyViewModel : ObservableObject
{
    private readonly Lazy<IHeavyService> _heavyService;

    public MyViewModel(Lazy<IHeavyService> heavyService)
    {
        _heavyService = heavyService;
    }

    public void DoWork()
    {
        // Service is only created when accessed
        var service = _heavyService.Value;
        service.Execute();
    }
}
```

### Options Pattern

```csharp
// Configuration
public class AppSettings
{
    public string ApiBaseUrl { get; set; } = "";
    public int TimeoutSeconds { get; set; } = 30;
}

// Registration
var settings = new AppSettings
{
    ApiBaseUrl = "https://api.example.com",
    TimeoutSeconds = 60
};
services.AddSingleton(settings);

// Usage
public class ApiService : IApiService
{
    public ApiService(AppSettings settings)
    {
        var baseUrl = settings.ApiBaseUrl;
        var timeout = settings.TimeoutSeconds;
    }
}
```

## Cross-Module Service Sharing

### Define Service in Main App

```csharp
// Main App
public override void RegisterServices(IServiceCollection services)
{
    services.AddSingleton<INavigationService, NavigationService>();
}
```

### Use in Modules

```csharp
public class MyModule : IModule
{
    public void InitializeModule(IServiceProvider serviceProvider)
    {
        // NavigationService was registered in the main app
        var nav = serviceProvider.GetRequiredService<INavigationService>();
        nav.NavigateToHome();
    }
}
```

## Testing with DI

### Register Test Doubles

```csharp
public void Setup()
{
    var services = new ServiceCollection();

    // Register test doubles
    services.AddSingleton<IUserService, MockUserService>();
    services.AddSingleton<ILogger, MockLogger>();

    // Register the ViewModel
    services.AddTransient<UserListViewModel>();

    var provider = services.BuildServiceProvider();
    var vm = provider.GetRequiredService<UserListViewModel>();
}
```

### Testing ViewModels

```csharp
public class UserListViewModelTests
{
    [Fact]
    public void LoadUsers_UpdatesItemsList()
    {
        // Arrange
        var mockService = new MockUserService();
        var vm = new UserListViewModel(mockService);

        // Act
        vm.RefreshCommand.Execute();

        // Assert
        Assert.NotEmpty(vm.Users);
    }
}
```

## Common Pitfalls

### 1. Captive Dependency

```csharp
// WRONG: DbContext captured by singleton
services.AddSingleton<IService>(sp =>
{
    var dbContext = sp.GetRequiredService<DbContext>(); // ❌
    return new Service(dbContext);
});

// CORRECT: Scoped DbContext
services.AddScoped<MyService>(sp =>
{
    var dbContext = sp.GetRequiredService<DbContext>(); // ✓
    return new Service(dbContext);
});
```

### 2. Circular Dependencies

```csharp
// WRONG: Circular dependency
public class A
{
    public A(B b) { }
}
public class B
{
    public B(A a) { }  // ❌ Circular!

// CORRECT: Break the cycle with an interface
public interface IA { }
public interface IB { }
public class A : IA
{
    public A(IB b) { }
}
```

## Next Steps

- [Navigation](navigation.md) - Implement navigation between views
- [Module Development](module-development.md) - Create reusable modules
