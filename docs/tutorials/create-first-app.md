# Tutorial: Create Your First Application

In this tutorial, we'll build a counter application with Crystal.Avalonia.

## What We're Building

A simple counter with increment/decrement buttons and reset.

## Step 1: Set Up the Project

```bash
dotnet new CT -o CounterApp
cd CounterApp
```

## Step 2: Define the ViewModel

Using CommunityToolkit.Mvvm:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace CounterApp.ViewModels;

public partial class CounterViewModel : ObservableObject
{
    [ObservableProperty]
    private int _count;

    public void Increment() => Count++;
    public void Decrement() => Count--;
    public void Reset() => Count = 0;
}
```

## Step 3: Create the View

`CounterView.axaml`:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Crystal.Avalonia"
             ViewModelLocator.AutoWireViewModel="True">
    <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Spacing="10">
        <TextBlock Text="{Binding Count}" FontSize="48" HorizontalAlignment="Center"/>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <Button Content="-" Command="{Binding Decrement}" Width="50"/>
            <Button Content="Reset" Command="{Binding Reset}"/>
            <Button Content="+" Command="{Binding Increment}" Width="50"/>
        </StackPanel>
    </StackPanel>
</UserControl>
```

`CounterView.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace CounterApp.Views;

public partial class CounterView : UserControl
{
    public CounterView() => InitializeComponent();
}
```

## Step 4: Register

In `App.axaml.cs`:

```csharp
public override void RegisterServices(IServiceCollection services)
{
    services.AddMvvmTransient<CounterView, CounterViewModel>();
}
```

## Step 5: Run

```bash
dotnet run
```

## Key Concepts

| Concept | Description |
|---------|-------------|
| `AddMvvmTransient` | Registers View/ViewModel pair in DI |
| `ViewModelLocator.AutoWireViewModel` | Auto-injects ViewModel into View's DataContext |
| `MvvmManager.ServiceProvider` | Access DI container directly |

## Next Steps

- [Module Development](module-development.md) - Create reusable modules
- [Dependency Injection](dependency-injection.md) - Advanced DI patterns
