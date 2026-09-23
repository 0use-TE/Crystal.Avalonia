# 发行说明

## 3.1.0

### CrystalOptions（实例 + DI）

`CrystalOptions` 不再是静态类。通过 `ConfigureOptions` 配置，并可从 DI 注入：

```csharp
public override void ConfigureOptions(CrystalOptions options)
{
    options.EnableViewLocator = true; // 默认
}
```

详见 [从 3.0 升级](upgrade-from-3.0.md)。

### EventToCommand

轻量事件 → `ICommand` 接线，位于默认 Avalonia xmlns（无需额外前缀）：

```xml
<!-- 单事件 -->
<Button EventToCommand.EventName="Click"
        EventToCommand.Command="{Binding SaveCommand}"/>

<!-- 多事件 -->
<Button>
  <EventToCommand.Bindings>
    <EventBinding EventName="Click" Command="{Binding PingCommand}"/>
    <EventBinding EventName="DoubleTapped" Command="{Binding OpenCommand}"/>
  </EventToCommand.Bindings>
</Button>
```

### 文档站

- WASM 在线 demo 随 DocFX 发布到 `/demo/`
- 侧栏将各版本升级说明收纳在 **升级指南** 下

## 3.0.0

- 实例 `MvvmManager`（`CrystalApplication.Mvvm`）
- `EnableViewLocator`（仅控制 ViewModel-first 的 `DataTemplates`）
- `ILifecycleAware.OnLoadedAsync(bool isFirstLoad)`

详见 [从 2.0 升级](upgrade-from-2.0.md)。
