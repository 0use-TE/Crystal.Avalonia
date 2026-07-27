# 快速开始

## 前置条件

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) 或更高版本

## 安装并运行

```bash
dotnet new install CrystalTemplate
dotnet new CT -o MyApp
cd MyApp
dotnet run
```

## App.axaml.cs

```csharp
public partial class App : CrystalApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmTransient<MainView, MainViewModel>(); // ViewModel → DI，View → 映射
    }

    public override void CreateShell(IServiceProvider sp)
    {
        CreateShell<MainWindow, MainView>(); // Shell → new（不进 DI）
    }
}
```

## View（View-First）

```xml
<UserControl ViewModelLocator.AutoWireViewModel="True">
    <TextBlock Text="{Binding Greeting}"/>
</UserControl>
```

## ViewModel-First

```xml
<ContentControl Content="{Binding MainViewModel}"/>
```

## 下一步

- [架构原理](architecture.md) — 启动流程与 MVVM 绑定
- [MVVM 模式](tutorials/mvvm-pattern.md)
- [模块开发](tutorials/module-development.md)
