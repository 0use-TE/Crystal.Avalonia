# 简介（3.0.0）

Crystal.Avalonia 是面向 Avalonia 应用的轻量基础设施层：

- **模块** — 通过 `IModule` 注册功能
- **DI** — Microsoft.Extensions.DependencyInjection
- **View/ViewModel 绑定** — View-first 与 ViewModel-first
- **AOT** — 对裁剪友好

## 不是 MVVM 框架

不提供 ViewModel 基类、命令或绑定 —— 请使用 CommunityToolkit.Mvvm、Prism、ReactiveUI 等。

## 绑定模式

**View-first** — XAML 设置 `ViewModelLocator.AutoWireViewModel="True"`，ViewModel 从 DI 解析：

```xml
<UserControl ViewModelLocator.AutoWireViewModel="True">
    <TextBlock Text="{Binding Greeting}"/>
</UserControl>
```

**ViewModel-first** — 将 ViewModel 绑定到 `ContentControl`，由 ViewLocator 创建 View：

```xml
<ContentControl Content="{Binding MainViewModel}"/>
```

## 注册

```csharp
services.AddMvvmTransient<MainView, MainViewModel>(); // ViewModel → DI，View → 仅映射
// Shell: CreateShell<MainWindow, MainView>() — 不进 DI
```

| 组件 | 作用 |
|------|------|
| `CrystalApplication` | 带模块/DI 启动的应用基类 |
| `CrystalApplication.Mvvm` | 每应用一份 View↔VM 映射和 `ServiceProvider` |
| `CreateShell<TWindow, TView>()` | 用 `new` 创建 Shell；ViewModel 由 ViewModelLocator 注入 |
| `AddMvvmTransient` / `AddMvvmSingleton` | ViewModel 生命周期 + View 映射 |
| `ViewModelLocator` | View-first 的 DataContext 注入 |
| `ViewLocator` | ViewModel-first 的 View 创建（`EnableViewLocator`） |

## 下一步

- [升级指南](upgrade.md) — 从 2.0.x 或 v1.2 迁移
- [架构原理](architecture.md) — 内部如何工作
- [快速开始](getting-started.md)
- [MVVM 模式](tutorials/mvvm-pattern.md)
