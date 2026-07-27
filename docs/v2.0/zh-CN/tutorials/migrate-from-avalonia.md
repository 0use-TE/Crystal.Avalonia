# 从官方 Avalonia 模板迁移到 Crystal.Avalonia

本教程说明如何将现有官方 Avalonia 项目迁移为使用 Crystal.Avalonia。

## 为何迁移？

| 特性 | 官方模板 | Crystal.Avalonia |
|------|----------|------------------|
| 模块系统 | 无 | 内置模块化架构 |
| DI 集成 | 手动 | 通过 Microsoft.Extensions.DependencyInjection 自动完成 |
| MVVM 绑定 | 手动 | 自动 View/ViewModel 绑定 |
| AOT 支持 | 基础 | 完整支持并带注解 |

## 步骤 1：安装 Crystal.Avalonia

添加 NuGet 包：

```bash
dotnet add package Crystal.Avalonia
```

或在 `.csproj` 中：

```xml
<PackageReference Include="Crystal.Avalonia" />
```

## 步骤 2：继承 CrystalApplication

替换 `App.axaml.cs`：

### 之前（官方模板）

```csharp
public override void OnFrameworkInitializationCompleted()
{
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
        desktop.MainWindow = new MainWindow();
    }
    // ...
}
```

### 之后（Crystal.Avalonia）

```csharp
public class App : CrystalApplication  // Change base class
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void RegisterModules(IModuleRegistrar moduleRegistrar)
    {
        // Register modules here
    }

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmTransient<MainView, MainViewModel>();
    }

    public override void CreateShell(IServiceProvider serviceProvider)
    {
        CreateShell<MainWindow, MainView>();
    }
}
```

## 步骤 3：注册 View/ViewModel 对

### 之前

```csharp
// Manual wiring in code-behind
public MainWindow()
{
    InitializeComponent();
    DataContext = new MainViewModel();
}
```

### 之后

仍需要 `InitializeComponent()`（这是 Avalonia 的工作方式），但 DataContext 会自动注入：

```csharp
// In RegisterServices
public override void RegisterServices(IServiceCollection services)
{
    services.AddMvvmTransient<MainView, MainViewModel>();
}
```

### 在 XAML 中

```xml
<Window xmlns:vm="using:Crystal.Avalonia"
        ViewModelLocator.AutoWireViewModel="True"
        x:Class="MyApp.Views.MainWindow">
```

## 步骤 4：创建模块（可选）

### 之前

所有代码在单一项目中，手动组织。

### 之后

创建模块类：

```csharp
public class SettingsModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmTransient<SettingsView, SettingsViewModel>();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        // Module initialization
    }
}
```

在 `App.axaml.cs` 中注册：

```csharp
public override void RegisterModules(IModuleRegistrar moduleRegistrar)
{
    moduleRegistrar.RegisterModule<SettingsModule>();
}
```

## 步骤 5：更新 XAML 视图

在每个视图上启用 `AutoWireViewModel`：

```xml
<UserControl xmlns:vm="using:Crystal.Avalonia"
             ViewModelLocator.AutoWireViewModel="True"
             x:Class="MyApp.Views.MainView">
    <!-- Your content -->
</UserControl>
```

## 完整对比

### 项目结构

**之前：**
```
MyApp/
├── App.axaml
├── App.axaml.cs
├── ViewModels/
│   └── MainViewModel.cs
├── Views/
│   ├── MainWindow.axaml
│   └── MainView.axaml
└── Program.cs
```

**之后（含模块）：**
```
MyApp/
├── App.axaml
├── App.axaml.cs
├── ViewModels/
├── Views/
├── Modules/           # Optional: organized by feature
│   ├── Settings/
│   │   ├── SettingsModule.cs
│   │   ├── SettingsView.axaml
│   │   └── SettingsViewModel.cs
│   └── Dashboard/
└── Program.cs
```

### 关键变更摘要

| 变更项 | 做法 |
|--------|------|
| 基类 | `Application` → `CrystalApplication` |
| 应用启动 | 重写 `CreateShell()`，而非 `OnFrameworkInitializationCompleted()` |
| View/VM 绑定 | `AddMvvmTransient` + `AutoWireViewModel="True"` |
| Shell | `CreateShell<MainWindow, MainView>()` |
| 模块 | 实现 `IModule` 的类，通过 `RegisterModules()` 注册 |

## 下一步

- [模块开发](module-development.md) — 深入了解模块系统
- [MVVM 模式](mvvm-pattern.md) — 掌握 Crystal.Avalonia 的 MVVM 模式

## 延伸阅读

> **工作原理：** [架构原理 — 启动流程](../architecture.md#bootstrap-pipeline) — `CrystalApplication` 如何替代官方 Avalonia 模板中的启动逻辑。
