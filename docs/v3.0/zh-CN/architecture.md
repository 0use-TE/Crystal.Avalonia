# 架构原理

Crystal.Avalonia 的内部工作方式。用法请见 [快速开始](getting-started.md) 与 [教程](tutorials/mvvm-pattern.md)。

<a id="bootstrap-pipeline"></a>
## 启动流程

`CrystalApplication.OnFrameworkInitializationCompleted()` 按以下顺序执行：

```
创建 CrystalApplication.Mvvm
    ↓
services.AddSingleton(Mvvm)
RegisterServices(services)          ← 应用级 DI（AddMvvm 写入该实例的映射）
    ↓
创建并注册 ModuleManager
RegisterModules(moduleRegistrar)    ← 应用注册 IModule
    ↓
moduleManager.InitService(services) ← 各 module.RegisterServices()
    ↓
services.BuildServiceProvider()
    ↓
Mvvm.ServiceProvider = sp           ← 在 InitModules 之前（ViewModelLocator 可解析）
    ↓
将 ViewLocator 加入 DataTemplates  ← 若 EnableViewLocator
    ↓
moduleManager.InitModules(sp)       ← 各 module.InitializeModule()
    ↓
CreateShell(sp)                     ← 创建 MainWindow / MainView
```

要点：

- **先 App 后模块** — `App.RegisterServices` 在 `InitService` 之前执行
- **单一 `ServiceProvider`** — 只构建一次；模块在容器就绪后初始化
- **`CrystalApplication.Mvvm.ServiceProvider`** — 在 `InitModules` 与 `CreateShell` 前赋值，供 `ViewModelLocator` 使用
- **每个应用一个 `MvvmManager`** — 映射在应用实例上，不是进程级静态字典

<a id="module-system"></a>
## 模块系统

`ModuleManager` 实现 `IModuleRegistrar`：

| 阶段 | 方法 | 时机 |
|------|------|------|
| 注册 | `RegisterModule<T>()` | 构建容器前；`Activator.CreateInstance<T>()` |
| 服务 | `IModule.RegisterServices()` | `InitService` 期间，`BuildServiceProvider` 之前 |
| 初始化 | `IModule.InitializeModule()` | 容器构建后，`InitModules` 期间 |

模块是普通类 —— 无程序集扫描。每个模块在 `RegisterModules` 中显式注册。库不提供模块依赖图、延迟加载、导航或事件聚合器。

<a id="mvvm-wiring"></a>
## MVVM 绑定

### 类型映射（`MvvmManager`）

`AddMvvmTransient` / `AddMvvmSingleton` 做两件事：

1. 仅将 **ViewModel** 注册进 DI（`AddTransient` / `AddSingleton`）
2. 在 `IServiceCollection` 中的 `MvvmManager` 实例上记录 View ↔ ViewModel 映射（与 `CrystalApplication.Mvvm` 为同一实例）

`TView` **不会**加入 `IServiceCollection`。

### View-First（`ViewModelLocator`）

附加属性 `AutoWireViewModel="True"` 在变更时触发。**不**依赖 `CrystalOptions.EnableViewLocator`。

```
XAML 加载 View
    ↓
ViewModelLocator 读取 view.GetType()
    ↓
CrystalApplication.Mvvm.GetVmType(viewType)
    ↓
ServiceProvider.GetRequiredService(vmType)
    ↓
view.DataContext = viewModel
    ↓
ViewLifecycleBinder.AttachIfNeeded()
```

设计时模式（`Design.IsDesignMode`）会跳过。

### ViewModel-First（`ViewLocator`）

当 `CrystalOptions.EnableViewLocator` 为 `true`（默认）时，注册为 `Application.DataTemplates` 上的 `IDataTemplate`：

```
ContentControl.Content = viewModel
    ↓
ViewLocator.Match(vm) — 是否有映射？
    ↓
Mvvm.GetViewType(vmType)
    ↓
Activator.CreateInstance(viewType)   ← 不从 DI
    ↓
view.DataContext = viewModel
    ↓
ViewLifecycleBinder.AttachIfNeeded()
```

仅匹配已注册映射的类型。

### 生命周期（`ViewLifecycleBinder`）

当 DataContext 实现 `ILifecycleAware`：

- 订阅 View `Loaded` / `Unloaded` 并保持订阅
- 每次 `Loaded` 调用 `OnLoadedAsync(isFirstLoad)` — `isFirstLoad` 按 **ViewModel 实例**（该对象第一次加载为 `true`）
- 每次 `Unloaded` 调用 `OnUnloaded()`
- 异常捕获并写入 `Trace`

`ViewModelLocator` 与 `ViewLocator` 都会使用。

## Shell 创建

Shell 视图（`MainWindow`、`MainView`）**不进 DI** — 用 `new` 创建：

```csharp
public override void CreateShell(IServiceProvider sp)
{
    CreateShell<MainWindow, MainView>();
}
```

`CreateShell<TWindow, TView>()` 按 `ApplicationLifetime` 赋值：

| Lifetime | 行为 |
|----------|------|
| `IClassicDesktopStyleApplicationLifetime` | `desktop.MainWindow = new TWindow()` |
| `ISingleViewApplicationLifetime` | `single.MainView = new TView()` |
| `IActivityApplicationLifetime` | `factory.MainViewFactory = () => new TView()` |

子内容 ViewModel 在 View 加载时注入（`AutoWireViewModel="True"`）。

若 Shell **确实**需要构造注入，可重写 `CreateShell` 手动解析 —— 这是例外，不是默认。

<a id="design-decisions"></a>
## 设计决策

### 为什么 View 不进 DI

| | View | ViewModel |
|---|------|-----------|
| 创建方式 | XAML 解析 / `Activator.CreateInstance` | DI 容器 |
| 生命周期 | 绑定视觉树 | Transient 或 Singleton |
| 构造依赖 | 通常无（code-behind 精简） | 服务、仓储等 |

View 是 UI 产物；ViewModel 承载业务逻辑与依赖。View 不进 DI 可避免容器管理 UI 生命周期，并简化 AOT 裁剪。

### Transient vs Singleton

仅两种模式 —— 无 Hybrid。按 ViewModel 选择：

- **Transient** — 每次导航新实例（页面常见）；每次都是 `OnLoadedAsync(true)`
- **Singleton** — 共享状态（设置、会话）；再次进入时 `isFirstLoad: false`

### 版本脉络

| 从 | 到 | 说明 |
|------|----|--------|
| v1.2 | 2.0 | 仅 ViewModel 进 DI；无 `AddMvvmHybrid` |
| 2.0.0 | 2.0.1 | Shell 用 `CreateShell` / `new`；移除 `CreateShellFromDi` |
| 2.0.1 | 3.0.0 | 实例 `MvvmManager`；`EnableViewLocator`；`OnLoadedAsync(bool)` |

见 [升级指南](upgrade.md)。

<a id="aot--trimming"></a>
## AOT 与裁剪

Crystal.Avalonia 避免运行时程序集扫描。类型发现均为编译期泛型：

- `AddMvvmTransient<TView, TViewModel>` — 通过 `[DynamicallyAccessedMembers(PublicConstructors)]` 保留构造函数
- `ModuleManager.RegisterModule<TModule>()` — 同上
- `ViewLocator.CreateView` — 映射字典中的 View 类型已注解

库设置 `IsAotCompatible=true`。发布命令见 [AOT 兼容性](aot-compatibility.md)。

## 组件关系

```
CrystalApplication
├── Mvvm (MvvmManager) ───── 每应用一份映射 + ServiceProvider
├── ModuleManager ────────── IModule.RegisterServices / InitializeModule
├── ViewModelLocator ─────── 附加属性 → DI 解析 ViewModel
├── ViewLocator ──────────── IDataTemplate → Activator.CreateInstance View
├── ViewLifecycleBinder ──── ILifecycleAware 钩子
└── CrystalOptions ───────── EnableViewLocator（默认 true）
```
