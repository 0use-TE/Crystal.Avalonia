# 创建第一个应用

```bash
dotnet new CT -o CounterApp
cd CounterApp
```

## ViewModel

```csharp
public partial class CounterViewModel : ObservableObject
{
    [ObservableProperty] private int _count;
    public void Increment() => Count++;
    public void Decrement() => Count--;
    public void Reset() => Count = 0;
}
```

## View

```xml
<UserControl ViewModelLocator.AutoWireViewModel="True">
    <StackPanel HorizontalAlignment="Center" Spacing="10">
        <TextBlock Text="{Binding Count}" FontSize="48"/>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <Button Content="-" Command="{Binding Decrement}"/>
            <Button Content="Reset" Command="{Binding Reset}"/>
            <Button Content="+" Command="{Binding Increment}"/>
        </StackPanel>
    </StackPanel>
</UserControl>
```

## 注册

```csharp
services.AddMvvmTransient<CounterView, CounterViewModel>();
// MainWindow: 在 CreateShell 中调用 CreateShell<MainWindow, MainView>()
```

```bash
dotnet run
```

## 延伸阅读

> **工作原理：** [架构原理 — 启动流程与 MVVM 绑定](../architecture.md#bootstrap-pipeline) — 启动顺序、`ViewModelLocator` 自动绑定，以及为何只有 ViewModel 进入 DI。
