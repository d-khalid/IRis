using IRis.ViewModels;


namespace IRis.ViewModels.Main;


public partial class MainWindowViewModel : ViewModelBase
{
    public SimulationViewModel Simulation { get; } = SimulationViewModel.GetInstance();
}
