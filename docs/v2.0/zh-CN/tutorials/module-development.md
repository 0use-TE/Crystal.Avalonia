# 模块开发

模块是 Crystal.Avalonia 应用的核心构建块，可将代码组织为独立、可复用的单元。

## 什么是模块？

模块是实现 `IModule` 的类，封装：

- **Services** — 模块提供的服务依赖
- **Views** — 模块专属的 UI 组件
- **ViewModels** — 模块视图的业务逻辑
- **Initialization Logic** — 应用启动时执行的初始化

## 创建模块

### 步骤 1：定义模块类

```csharp
using Crystal.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MyApp.Modules.UserManagement;

public class UserManagementModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        // Register View/ViewModel pairs
        services.AddMvvmTransient<UserListView, UserListViewModel>();
        services.AddMvvmTransient<UserDetailView, UserDetailViewModel>();

        // Register other services
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddTransient<UserService>();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        // Perform initialization tasks
        var userService = serviceProvider.GetRequiredService<UserService>();
        userService.LoadUsers();
    }
}
```

### 步骤 2：注册模块

在 `App.axaml.cs` 中：

```csharp
public override void RegisterModules(IModuleRegistrar moduleRegistrar)
{
    moduleRegistrar.RegisterModule<UserManagementModule>();
}
```

## 模块项目结构

对于较大应用，可将每个模块放在独立项目中：

```
MySolution/
├── MyApp/                    # Main application
│   └── App.axaml.cs
├── MyApp.Modules.UserManagement/  # Module in separate project
│   ├── UserManagementModule.cs
│   ├── Views/
│   ├── ViewModels/
│   └── Services/
└── MyApp.Modules.Settings/   # Another module
    └── ...
```

## 在应用间共享模块

模块可轻松在不同应用间共享：

1. 为模块创建独立项目
2. 在主应用中引用该模块项目
3. 在应用中注册模块

### 模块项目文件（.csproj）

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Crystal.Avalonia" />
    </ItemGroup>

</Project>
```

### 在主应用中引用

```csharp
// In App.axaml.cs of the main application
public override void RegisterModules(IModuleRegistrar moduleRegistrar)
{
    // Register modules from referenced projects
    moduleRegistrar.RegisterModule<MyApp.Modules.UserManagement.UserManagementModule>();
    moduleRegistrar.RegisterModule<MyApp.Modules.Settings.SettingsModule>();
}
```

## 高级模块模式

### 带依赖的模块

```csharp
public class ReportingModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmTransient<ReportView, ReportViewModel>();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        // Get services from the main app
        var logger = serviceProvider.GetRequiredService<ILogger>();
        logger.LogInformation("Reporting module initialized");
    }
}
```

### 条件加载模块

```csharp
public override void RegisterModules(IModuleRegistrar moduleRegistrar)
{
    // Only load admin module for admin users
    if (User.IsAdmin)
    {
        moduleRegistrar.RegisterModule<AdminModule>();
    }

    moduleRegistrar.RegisterModule<CommonModule>();
}
```

### 带共享基类的模块

```csharp
// Base class for all modules in your application
public abstract class AppModule : IModule
{
    protected IServiceCollection? Services { get; private set; }
    protected IServiceProvider? ServiceProvider { get; private set; }

    public void RegisterServices(IServiceCollection services)
    {
        Services = services;
        OnRegisterServices();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        OnInitialize();
    }

    protected abstract void OnRegisterServices();
    protected abstract void OnInitialize();
}

// Usage
public class MyModule : AppModule
{
    protected override void OnRegisterServices()
    {
        Services!.AddMvvmTransient<MyView, MyViewModel>();
    }

    protected override void OnInitialize()
    {
        var config = ServiceProvider!.GetRequiredService<AppConfig>();
        // Use config...
    }
}
```

## 最佳实践

| 实践 | 原因 |
|------|------|
| 保持模块聚焦 | 每个模块应代表单一功能域 |
| 定义模块契约 | 跨模块通信用接口 |
| 最小化模块依赖 | 模块应尽可能独立 |
| 使用共享基类 | 减少大量模块的样板代码 |
| 考虑延迟加载 | 适用于并非始终需要的大型模块 |

## 跨模块通信

### 方案 1：共享服务

```csharp
// In a shared project
public interface INavigationService
{
    void NavigateTo<TView>() where TView : Control;
}

// In ModuleA
public class NavigationService : INavigationService { ... }

// In ModuleA's RegisterServices
services.AddSingleton<INavigationService, NavigationService>();

// In ModuleB, use the shared service
public class ModuleB : IModule
{
    public void InitializeModule(IServiceProvider sp)
    {
        var nav = sp.GetRequiredService<INavigationService>();
        nav.NavigateTo<ModuleAView>();
    }
}
```

### 方案 2：事件聚合器

```csharp
// Simple event aggregator
public interface IEventAggregator
{
    void Subscribe<TEvent>(Action<TEvent> handler);
    void Publish<TEvent>(TEvent event);
}

// Usage
public class ModuleA : IModule
{
    public void InitializeModule(IServiceProvider sp)
    {
        var ea = sp.GetRequiredService<IEventAggregator>();
        ea.Subscribe<UserCreatedEvent>(e => /* react */);
    }
}

public class ModuleB : IModule
{
    public void InitializeModule(IServiceProvider sp)
    {
        var ea = sp.GetRequiredService<IEventAggregator>();
        ea.Publish(new UserCreatedEvent { UserId = 123 });
    }
}
```

## 下一步

- [依赖注入](dependency-injection.md) — 高级 DI 模式与服务注册
- [导航](navigation.md) — 在视图间实现导航

## 延伸阅读

> **工作原理：** [架构原理 — 模块系统](../architecture.md#module-system) — `RegisterModule` → `InitService` → `InitModules` 流水线，以及为何无程序集扫描。
