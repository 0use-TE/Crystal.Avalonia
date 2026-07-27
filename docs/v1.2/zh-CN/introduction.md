# 简介（v1.2）

> **旧版文档**，适用于 Crystal.Avalonia **1.2.x**。[升级到 v2.0](../../v2.0/zh-CN/upgrade-from-1.2.md)。

Crystal.Avalonia 是面向 Avalonia 应用的轻量基础设施层：

- **模块** — 通过 `IModule` 注册功能
- **DI** — Microsoft.Extensions.DependencyInjection
- **View/ViewModel 绑定** — View-first 与 ViewModel-first
- **AOT** — 对裁剪友好

## 不是 MVVM 框架

请使用 CommunityToolkit.Mvvm、Prism、ReactiveUI 等作为 MVVM 原语。

## 绑定模式

**View-first** — `ViewModelLocator.AutoWireViewModel="True"`：

```xml
<UserControl ViewModelLocator.AutoWireViewModel="True">
    <TextBlock Text="{Binding Greeting}"/>
</UserControl>
```

**ViewModel-first** — 绑定 ViewModel；ViewLocator 从 **DI 解析 View**：

```xml
<ContentControl Content="{Binding MainViewModel}"/>
```

## 注册（v1.2）

`AddMvvm*` 将 **View 与 ViewModel 均注册到 DI**：

```csharp
services.AddMvvmTransient<MainView, MainViewModel>();
services.AddMvvmHybrid<SettingsView, SettingsViewModel>();   // View=Transient, VM=Singleton
services.AddMvvmSingleton<AboutView, AboutViewModel>();
```

| 方法 | View | ViewModel |
|--------|------|-----------|
| `AddMvvmTransient` | Transient | Transient |
| `AddMvvmHybrid` | Transient | Singleton |
| `AddMvvmSingleton` | Singleton | Singleton |

## 下一步

- [快速开始](getting-started.md)
- [MVVM 模式](tutorials/mvvm-pattern.md)
