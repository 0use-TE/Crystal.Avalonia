using CommunityToolkit.Mvvm.ComponentModel;
using ModuleAExample;
using ModuleBExample;
using ShareContract;

namespace Crystal.AvaloniaExample.ViewModels
{
    internal partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _greeting = "I am from MainViewModel";
        [ObservableProperty]
        private ViewModelBase _ouseViewModel;
        [ObservableProperty]
        private ViewModelBase _moduleBViewModel;

        public MainViewModel(OuseViewModel ouseViewModel, ModuleBViewModel moduleBViewModel)
        {
            _ouseViewModel = ouseViewModel;
            _moduleBViewModel = moduleBViewModel;
        }
    }
}
