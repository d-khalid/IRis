using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services.Singleton;


namespace IRis.ViewModels;


public partial class AppViewModel : ViewModelBase
{
    [ObservableProperty] private AppState _appState = AppState.Get();
}
