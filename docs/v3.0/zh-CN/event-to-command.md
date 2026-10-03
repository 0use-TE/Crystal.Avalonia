# EventToCommand（3.1.1）

把 View 事件绑定到 ViewModel 的 `ICommand`，无需编写 code-behind 事件处理器。
`EventToCommand` 和 `EventBinding` 位于默认 Avalonia XML 命名空间，无需额外声明。
命令可使用 CommunityToolkit.Mvvm 或其他 MVVM 库提供。

## 按事件名称绑定

```xml
<Button Content="保存"
        EventToCommand.EventName="Click"
        EventToCommand.Command="{Binding SaveCommand}"
        EventToCommand.CommandParameter="{Binding SelectedItem}" />
```

3.1.1 通过 Avalonia 路由事件注册表解析名称，也支持继承自基类的事件。
`Click` 与 `ClickEvent` 等价，名称区分大小写。
常见事件包括 `Loaded`、`Unloaded`、`SizeChanged`、`TextChanged`、`SelectionChanged`、
`PointerPressed` 和 `DoubleTapped`，具体取决于绑定控件支持哪些事件。

启动初始化可绑定根视图的 `Loaded`：

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             EventToCommand.EventName="Loaded"
             EventToCommand.Command="{Binding InitializeCommand}">
    <!-- 视图内容 -->
</UserControl>
```

视图重新挂载时可能再次触发 `Loaded`。需要仅初始化一次时，请在 ViewModel 中处理。
通过 Crystal 的 ViewModelLocator 或 ViewLocator 绑定的 ViewModel，也可使用
`ILifecycleAware.OnLoadedAsync(bool isFirstLoad)`。

## 直接指定路由事件

```xml
<Button EventToCommand.RoutedEvent="{x:Static Button.ClickEvent}"
        EventToCommand.Command="{Binding SaveCommand}" />
```

自定义事件所属类型使用相应的 XML 命名空间前缀。
显式 `RoutedEvent` 优先于 `EventName`，不需要反射查找事件字段；访问字段时也会初始化声明类型。
因此，自定义事件或附加路由事件推荐使用这种方式。
按名称查找只能找到控件及其基类所属类型中已经注册的事件。

## 多事件与事件参数

```xml
<Button>
    <EventToCommand.Bindings>
        <EventBinding RoutedEvent="{x:Static Button.ClickEvent}"
                      Command="{Binding SaveCommand}"
                      CommandParameter="{Binding SelectedItem}" />
        <EventBinding EventName="PointerPressed"
                      Command="{Binding PressCommand}"
                      PassEventArgs="True" />
    </EventToCommand.Bindings>
</Button>
```

`PassEventArgs="True"` 时，命令参数为事件参数对象，忽略 `CommandParameter`。
否则使用配置的命令参数。仅在 `CanExecute(parameter)` 返回 `true` 时执行命令，
命令为空时不执行。简写与集合语法可以同时使用；若两者绑定同一事件，两条命令都会执行。

| 属性 | 用途 |
| --- | --- |
| `EventName` | 已注册路由事件的名称；普通 CLR `EventHandler` 回退仅限 JIT |
| `RoutedEvent` | 显式路由事件，优先于 `EventName` |
| `Command` | ViewModel 的 `ICommand` |
| `CommandParameter` | `PassEventArgs` 为 false 时使用的参数 |
| `PassEventArgs` | 使用事件参数代替配置的参数 |
| `EventToCommand.Bindings` | `EventBinding` 映射集合 |

修改事件名称或显式事件时，会重建订阅并移除原处理器。
集合新增、删除、替换都会更新处理器；替换集合时也会释放旧集合的变更订阅。
命令与参数在事件触发时读取，因此可使用更新后的绑定值。

## Native AOT 与升级

```bash
dotnet add package Crystal.Avalonia --version 3.1.1
```

现有路由事件名称绑定无需修改。可移除之前为 `LoadedEvent`、`TextChangedEvent`
等路由事件字段添加的 `DynamicDependency` 临时措施，注册表查找不需要应用侧保留属性。

旧的 CLR 回退仅支持 JIT 运行时的普通 `EventHandler`，不支持 CLR 泛型 `EventHandler<TEventArgs>`。
裁剪的 JIT 应用需要保留 CLR 事件元数据；Native AOT 下禁用该回退。
显式 `RoutedEvent` 只能表示路由事件，不能直接表示任意 CLR 事件。

## 绑定失败诊断

无法解析事件时，会在 Avalonia 的 `Crystal.Avalonia` 日志区域输出警告。
请将应用的 Avalonia 日志接收器配置为包含警告等级，检查名称拼写、大小写、
控件是否支持该事件，以及自定义事件所属类型是否已注册事件。
自定义事件或附加路由事件优先使用显式 `RoutedEvent`。

另见 [AOT 兼容性](aot-compatibility.md) 和 [发行说明](release-notes.md)。
