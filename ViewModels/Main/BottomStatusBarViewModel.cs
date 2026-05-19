using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;


namespace IRis.ViewModels.Main;


public partial class BottomStatusBarViewModel : ViewModelBase
{
    [ObservableProperty] 
    private SimulationManager _simulation = SimulationManager.GetInstance();
}
