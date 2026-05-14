using IRis.ViewModels;


namespace IRis.ViewModels.Main;


public partial class MainWindowViewModel : ViewModelBase
{
    public SimulationViewModel Simulation { get; }
    public LeftSidebarViewModel LeftSidebarVm { get; }
    public MainCanvasViewModel MainCanvasVm { get; }


    public MainWindowViewModel()
    {
        Simulation = new();
        MainCanvasVm = new(Simulation);
        LeftSidebarVm = new(Simulation);
    }
}
