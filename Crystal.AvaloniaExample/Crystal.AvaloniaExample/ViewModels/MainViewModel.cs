using CommunityToolkit.Mvvm.ComponentModel;

namespace Crystal.AvaloniaExample.ViewModels;

internal partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Crystal.Avalonia Example";

    [ObservableProperty]
    private OuseViewModel _ouseViewModel;

    [ObservableProperty]
    private ModuleBViewModel _moduleBViewModel;

    public MainViewModel(OuseViewModel ouseViewModel, ModuleBViewModel moduleBViewModel)
    {
        _ouseViewModel = ouseViewModel;
        _moduleBViewModel = moduleBViewModel;
    }
}
