# 模块开发（v1.2）

> **v2.0+** 请参阅 [v2.0 模块开发](~/docs/v2.0/zh-CN/tutorials/module-development.md)。

## 基础模块

```csharp
public class SettingsModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmTransient<SettingsView, SettingsViewModel>();
        services.AddSingleton<ISettingsService, SettingsService>();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
        // initialization
    }
}
```

在 `App.axaml.cs` 中注册：

```csharp
public override void RegisterModules(IModuleRegistrar registrar)
{
    registrar.RegisterModule<SettingsModule>();
}
```

在 v1.2 中，`AddMvvmTransient` 将 **View 与 ViewModel 均注册到 DI**。

## 独立模块项目

模块可放在独立的类库项目中，由主应用引用。
