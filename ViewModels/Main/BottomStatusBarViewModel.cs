using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels.Main;


public partial class BottomStatusBarViewModel : ObservableObject
{
    [ObservableProperty] 
    private SimulationViewModel _simulation = SimulationViewModel.GetInstance();
}
