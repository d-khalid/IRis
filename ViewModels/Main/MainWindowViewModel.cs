using IRis.ViewModels;


namespace IRis.ViewModels.Main;


public partial class MainWindowViewModel : ViewModelBase
{
    public LeftSidebarViewModel LeftSidebarVm { get; }
    public MainCanvasViewModel MainCanvasVm { get; }


    public MainWindowViewModel()
    {
        SimulationViewModel simulation = new();
        LeftSidebarVm = new(simulation);
        MainCanvasVm = new(simulation);
    }
}
