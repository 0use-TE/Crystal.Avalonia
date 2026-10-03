# 发行说明

## 3.1.1

### EventToCommand：修复裁剪与 Native AOT 事件绑定

`EventName` 改为通过 Avalonia 的 `RoutedEventRegistry` 查找已注册的路由事件，
支持继承自基类的事件，不再反射查找公开的 `*Event` 字段。
修复了字段反射信息被裁剪后，启动 `Loaded`、接收编辑器 `TextChanged` 等事件命令静默失效的问题。

- 保持现有名称写法，例如 `Loaded`、`Click`、`ClickEvent`。
- 新增 `EventToCommand.RoutedEvent` 和 `EventBinding.RoutedEvent`，直接传入路由事件；优先级高于 `EventName`。
- 不支持的事件绑定会在 Avalonia 的 `Crystal.Avalonia` 日志区域产生警告。
- 替换绑定集合时正确解绑旧集合的变更订阅。
- 旧的 CLR `EventHandler` 回退仅在 JIT 运行时启用；裁剪应用需保留事件元数据，Native AOT 下禁用该回退。

执行 `dotnet add package Crystal.Avalonia --version 3.1.1` 升级。
现有路由事件 XAML 无需修改，可移除应用中用于保留这些事件字段的 `DynamicDependency` 临时措施。
自定义事件或附加事件推荐直接使用 `RoutedEvent`；按名称解析自定义事件前，需要先完成事件注册。

详见 [EventToCommand](event-to-command.md) 和 [AOT 兼容性](aot-compatibility.md)。
3.x 文档沿用现有 `docs/v3.0/` 地址。CrystalTemplate 独立发布，本次仍为 3.1.0。

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
- **CrystalTemplate 3.1.0** — `ConfigureOptions`、`EventToCommand` 示例、包引用 `Crystal.Avalonia` 3.1.0

## 3.0.0

- 实例 `MvvmManager`（`CrystalApplication.Mvvm`）
- `EnableViewLocator`（仅控制 ViewModel-first 的 `DataTemplates`）
- `ILifecycleAware.OnLoadedAsync(bool isFirstLoad)`

详见 [从 2.0 升级](upgrade-from-2.0.md)。
