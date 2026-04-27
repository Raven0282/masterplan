using CommunityToolkit.Mvvm.ComponentModel;

namespace MasterplanXP.UI.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _greeting = "Welcome to Avalonia!";
    }
}
