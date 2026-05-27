# Create Your First App

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

## Register

```csharp
services.AddMvvmTransient<CounterView, CounterViewModel>();
services.AddTransient<MainWindow>();
```

```bash
dotnet run
```

## Further Reading

> **How it works:** [Architecture — Bootstrap Pipeline & MVVM Wiring](../architecture.md#bootstrap-pipeline) — startup sequence, `ViewModelLocator` auto-binding, and why only ViewModel enters DI.
