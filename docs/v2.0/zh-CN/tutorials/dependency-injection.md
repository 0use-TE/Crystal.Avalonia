# 依赖注入

Crystal.Avalonia 使用 Microsoft.Extensions.DependencyInjection 进行依赖注入。本教程介绍 DI 模式与最佳实践。

## 基本概念

### 构造函数注入（推荐）

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

### 属性注入

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

public partial class SettingsViewModel : ObservableObject
{
    public IConfigService? ConfigService { get; set; }
}
```

## 注册服务

### 在 Module.RegisterServices 中

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

### 在 App.RegisterServices 中

```csharp
public override void RegisterServices(IServiceCollection services)
{
    // Register application-wide services
    services.AddSingleton<IAppConfiguration, AppConfiguration>();
    services.AddSingleton<INavigationService, NavigationService>();

    services.AddMvvmTransient<MainView, MainViewModel>();
}
```

`AddMvvm*` 将 ViewModel 注册到 DI 并建立 View 映射。Shell：`CreateShell<MainWindow, MainView>()`。

### View/ViewModel 注册

| 方法 | 作用 |
|------|------|
| `AddMvvmTransient<TView, TViewModel>()` | `AddTransient<TViewModel>()` + View ↔ ViewModel 映射 |
| `AddMvvmSingleton<TView, TViewModel>()` | `AddSingleton<TViewModel>()` + View ↔ ViewModel 映射 |

`TView` 不会注册到 DI。View 由 XAML（View-first）或 `Activator.CreateInstance`（ViewLocator）创建。

## 服务生命周期

| 生命周期 | 适用场景 | 行为 |
|----------|----------|------|
| `Transient` | 轻量、无状态服务 | 每次新建实例 |
| `Singleton` | 共享状态、配置 | 始终同一实例 |
| `Scoped` | 按操作的数据（如 DB 上下文） | 每个作用域一个实例 |

### 示例：选择合适生命周期

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

## 高级模式

### 工厂模式

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

### 延迟解析

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

### Options 模式

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

## 跨模块共享服务

### 在主应用中定义服务

```csharp
// Main App
public override void RegisterServices(IServiceCollection services)
{
    services.AddSingleton<INavigationService, NavigationService>();
}
```

### 在模块中使用

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

## 使用 DI 进行测试

### 注册测试替身

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

### 测试 ViewModel

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

## 常见陷阱

### 1. 被捕获依赖（Captive Dependency）

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

### 2. 循环依赖

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

## 下一步

- [导航](navigation.md) — 在视图间实现导航
- [模块开发](module-development.md) — 创建可复用模块

## 延伸阅读

> **工作原理：** [架构原理 — 启动流程](../architecture.md#bootstrap-pipeline) — App 与模块服务的注册时机。[设计决策](../architecture.md#design-decisions) — ViewModel 在 DI 中，View 仅通过映射。
