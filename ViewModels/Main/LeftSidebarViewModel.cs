using IRis.ViewModels;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel(SimulationViewModel simulation) : ViewModelBase
{
    private readonly SimulationViewModel _simulation = simulation;
}
