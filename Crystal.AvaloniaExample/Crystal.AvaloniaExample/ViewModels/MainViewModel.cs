using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Crystal.AvaloniaExample.ViewModels;

internal partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Crystal.Avalonia Example";

    [ObservableProperty]
    private string _clickStatus = "Click the button (EventToCommand)";
    [ObservableProperty]
    private string _tappedStatus = "Tap the button (EventToCommand)";

    [ObservableProperty]
    private OuseViewModel _ouseViewModel;

    [ObservableProperty]
    private ModuleBViewModel _moduleBViewModel;

    public MainViewModel(OuseViewModel ouseViewModel, ModuleBViewModel moduleBViewModel)
    {
        _ouseViewModel = ouseViewModel;
        _moduleBViewModel = moduleBViewModel;
    }

    [RelayCommand]
    private void Ping()
    {
        ClickStatus = $"Click at {DateTime.Now:HH:mm:ss}";
    }

    [RelayCommand]
    private void DoublePing()
    {
        TappedStatus = $"DoubleTapped at {DateTime.Now:HH:mm:ss}";
    }
}
