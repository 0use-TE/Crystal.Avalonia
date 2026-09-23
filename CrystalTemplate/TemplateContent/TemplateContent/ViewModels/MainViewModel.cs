using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TemplateContent.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _greeting = "Welcome to Crystal.Avalonia!";

        [ObservableProperty]
        private string _status = "Try EventToCommand below";

        [RelayCommand]
        private void Ping() => Status = "Ping via EventToCommand";
    }
}
