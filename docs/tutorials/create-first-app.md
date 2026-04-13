# Tutorial: Create Your First Application

In this tutorial, we'll build a complete counter application with Crystal.Avalonia to understand the framework better.

## What We're Building

A simple counter application with:
- Increment/Decrement buttons
- Current count display
- Reset functionality

## Step 1: Set Up the Project

```bash
dotnet new crystal.avalonia -o CounterApp
cd CounterApp
```

## Step 2: Define the ViewModel

Create `CounterViewModel.cs`:

```csharp
using System;

public class CounterViewModel : ObservableObject
{
    private int _count;

    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }

    public void Increment() => Count++;
    public void Decrement() => Count--;
    public void Reset() => Count = 0;
}
```

## Step 3: Create the View

Create `CounterView.axaml`:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Crystal.Avalonia"
             ViewModelLocator.AutoWireViewModel="True">

    <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Spacing="10">
        <TextBlock Text="{Binding Count}"
                   FontSize="48"
                   HorizontalAlignment="Center"/>

        <StackPanel Orientation="Horizontal" Spacing="10">
            <Button Content="-" Command="{Binding Decrement}" Width="50"/>
            <Button Content="Reset" Command="{Binding Reset}"/>
            <Button Content="+" Command="{Binding Increment}" Width="50"/>
        </StackPanel>
    </StackPanel>
</UserControl>
```

Create `CounterView.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace CounterApp.Views;

public partial class CounterView : UserControl
{
    public CounterView()
    {
        InitializeComponent();
    }
}
```

## Step 4: Register in App.axaml.cs

```csharp
public override void RegisterServices(IServiceCollection services)
{
    services.AddMvvmBindingTransient<CounterView, CounterViewModel>();
}
```

## Step 5: Navigate to the Counter

In your `MainWindow.axaml`, add a button to navigate:

```xml
<StackPanel>
    <Button Content="Open Counter"
            Click="OpenCounter"/>

    <ContentControl Name="ContentArea"/>
</StackPanel>
```

In `MainWindow.axaml.cs`:

```csharp
public void OpenCounter()
{
    var counterView = MvvmManager.ServiceProvider!.GetService<CounterView>();
    ContentArea.Content = counterView;
}
```

## Step 6: Run

```bash
dotnet run
```

## Key Concepts Demonstrated

| Concept | How It's Used |
|---------|--------------|
| `ObservableObject` | ViewModel inherits from it to support property change notifications |
| `SetProperty()` | Used in setters to automatically notify UI of changes |
| `Command` binding | Buttons bound directly to ViewModel methods |
| `AutoWireViewModel` | XAML attribute that triggers automatic DataContext injection |
| `MvvmManager.ServiceProvider` | Access to DI container for manual view resolution |

## Complete Code

### ViewModel

```csharp
using System;

namespace CounterApp.ViewModels;

public class CounterViewModel : ObservableObject
{
    private int _count;

    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }

    public void Increment() => Count++;
    public void Decrement() => Count--;
    public void Reset() => Count = 0;
}
```

### View (AXAML)

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Crystal.Avalonia"
             ViewModelLocator.AutoWireViewModel="True">

    <StackPanel HorizontalAlignment="Center"
                VerticalAlignment="Center"
                Spacing="10">

        <TextBlock Text="{Binding Count}"
                   FontSize="48"
                   HorizontalAlignment="Center"/>

        <StackPanel Orientation="Horizontal" Spacing="10">
            <Button Content="-" Command="{Binding Decrement}" Width="50"/>
            <Button Content="Reset" Command="{Binding Reset}"/>
            <Button Content="+" Command="{Binding Increment}" Width="50"/>
        </StackPanel>
    </StackPanel>
</UserControl>
```

## Next Steps

- [Module Development](module-development.md) - Learn how to create reusable modules
- [Dependency Injection](dependency-injection.md) - Advanced DI patterns
