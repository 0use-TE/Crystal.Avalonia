# 快速开始（v1.2）

> **v2.0+** 请参阅 [v2.0 快速开始](../../v2.0/zh-CN/getting-started.md)。

## 安装并运行

```bash
dotnet new install CrystalTemplate
dotnet new CT -o MyApp
cd MyApp && dotnet run
```

## App.axaml.cs

```csharp
public partial class App : CrystalApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmTransient<MainView, MainViewModel>();
    }

    public override void CreateShell(IServiceProvider sp)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();
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

ViewLocator 从 DI 解析 View。
