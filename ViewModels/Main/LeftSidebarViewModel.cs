using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IRis.ViewModels;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using Tmds.DBus.Protocol;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel(SimulationViewModel simulation) : ViewModelBase
{
    [ObservableProperty] private SimulationViewModel _simulation = simulation;


    [RelayCommand]
    private void AddAnd()
    {
        Simulation.Preview = new AndGateViewModel()
        {
            Width = 50,
            Height = 50,
            Opacity = 0.5
        };
    }
}
