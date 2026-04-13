# Tutorial: MVVM Pattern in Crystal.Avalonia

This tutorial explains how the MVVM (Model-View-ViewModel) pattern works in Crystal.Avalonia.

## MVVM Overview

```
┌─────────────┐         ┌──────────────────┐         ┌─────────────┐
│    View     │◄───────│   ViewModel      │─────────│    Model    │
│  (AXAML)    │  Data  │   (C# Class)     │  Data   │  (C# Class) │
└─────────────┘  Binding└──────────────────┘  Access └─────────────┘
```

- **Model** - Your data and business logic
- **ViewModel** - Exposes data and commands for the View
- **View** - Declarative UI (AXAML)

## ViewModel Base Class

Crystal.Avalonia uses `ObservableObject` as the base class for ViewModels. It provides:

- `SetProperty()` - For notifying the UI when properties change
- `OnPropertyChanged()` - For manual property change notifications

### Example: Basic ViewModel

```csharp
using System;

public class MainViewModel : ObservableObject
{
    private string _title = "My App";
    private int _itemCount;
    private bool _isLoading;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public int ItemCount
    {
        get => _itemCount;
        set => SetProperty(ref _itemCount, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
}
```

## Property Binding

### In ViewModel

```csharp
private string _message;
public string Message
{
    get => _message;
    set => SetProperty(ref _message, value);  // Notifies UI of change
}
```

### In View (AXAML)

```xml
<TextBlock Text="{Binding Message}"/>
```

When `Message` is updated in the ViewModel, the TextBlock automatically updates.

## Command Binding

Commands allow the View to invoke methods in the ViewModel.

### ViewModel with Commands

```csharp
using System.Windows.Input;

public class CounterViewModel : ObservableObject
{
    private int _count;

    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }

    // Commands using Action
    public void Increment()
    {
        Count++;
    }

    public void Decrement()
    {
        Count--;
    }

    public void Reset()
    {
        Count = 0;
    }
}
```

### Command Binding in View

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <Button Content="-" Command="{Binding Decrement}"/>
    <Button Content="Reset" Command="{Binding Reset}"/>
    <Button Content="+" Command="{Binding Increment}"/>
</StackPanel>
```

## Collection Binding

### ViewModel with ObservableCollection

```csharp
using System.Collections.ObjectModel;

public class ListViewModel : ObservableObject
{
    public ObservableCollection<ItemViewModel> Items { get; } = new();

    public ListViewModel()
    {
        // Initialize with some items
        Items.Add(new ItemViewModel { Name = "Item 1" });
        Items.Add(new ItemViewModel { Name = "Item 2" });
    }
}

public class ItemViewModel : ObservableObject
{
    private string _name;
    private bool _isSelected;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
```

### List Binding in View

```xml
<ListBox ItemsSource="{Binding Items}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <CheckBox IsChecked="{Binding IsSelected}"/>
                <TextBlock Text="{Binding Name}"/>
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

## Automatic ViewModel Binding

Crystal.Avalonia can automatically inject ViewModels into Views.

### Enable Auto-Wiring

```xml
<UserControl xmlns:vm="using:Crystal.Avalonia"
             vm:ViewModelLocator.AutoWireViewModel="True">
```

### How It Works

1. View loads with `AutoWireViewModel="True"`
2. Crystal looks up the registered ViewModel type for this View
3. Resolves the ViewModel from the DI container
4. Assigns it to `DataContext`

### Manual Resolution

You can also manually resolve ViewModels:

```csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var vm = MvvmManager.ServiceProvider!.GetService<MainViewModel>();
        DataContext = vm;
    }
}
```

## Two-Way Binding

By default, bindings are one-way. Use two-way for input controls:

```xml
<TextBox Text="{Binding UserName, Mode=TwoWay}"/>
<TextBox Text="{Binding SearchQuery, Mode=TwoWay}"/>
```

## Best Practices

| Practice | Description |
|----------|-------------|
| Keep ViewModels thin | Move logic to services/models |
| Use interfaces | Makes testing easier |
| Avoid code-behind | Keep UI logic in ViewModel |
| Use ObservableCollection | For collections that change |
| Immutable models | Use immutable records for data transfer |

## Complete Example

### ViewModel

```csharp
public class OrderViewModel : ObservableObject
{
    private string _orderId = "";
    private OrderStatus _status;
    private bool _isEditing;

    public string OrderId
    {
        get => _orderId;
        set => SetProperty(ref _orderId, value);
    }

    public OrderStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public void Approve() => Status = OrderStatus.Approved;
    public void Reject() => Status = OrderStatus.Rejected;
    public void Edit() => IsEditing = true;
    public void Save() => IsEditing = false;
}

public enum OrderStatus { Pending, Approved, Rejected }
```

### View

```xml
<UserControl xmlns:vm="using:Crystal.Avalonia"
             xmlns:sys="clr-namespace:System;assembly=mscorlib"
             vm:ViewModelLocator.AutoWireViewModel="True">

    <StackPanel Margin="20" Spacing="15">
        <StackPanel Orientation="Horizontal" Spacing="10">
            <TextBlock Text="Order ID:" VerticalAlignment="Center"/>
            <TextBlock Text="{Binding OrderId}"
                       FontWeight="Bold"
                       VerticalAlignment="Center"/>
        </StackPanel>

        <StackPanel Orientation="Horizontal" Spacing="10">
            <TextBlock Text="Status:" VerticalAlignment="Center"/>
            <TextBlock Text="{Binding Status}"
                       VerticalAlignment="Center"/>
        </StackPanel>

        <StackPanel Orientation="Horizontal" Spacing="10">
            <Button Content="Approve"
                    Command="{Binding Approve}"
                    IsVisible="{Binding !IsEditing}"/>
            <Button Content="Reject"
                    Command="{Binding Reject}"
                    IsVisible="{Binding !IsEditing}"/>
            <Button Content="Edit"
                    Command="{Binding Edit}"
                    IsVisible="{Binding !IsEditing}"/>
            <Button Content="Save"
                    Command="{Binding Save}"
                    IsVisible="{Binding IsEditing}"/>
        </StackPanel>
    </StackPanel>
</UserControl>
```

## Next Steps

- [Dependency Injection](dependency-injection.md) - Learn about DI in Crystal.Avalonia
- [Module Development](module-development.md) - Organize code into modules
