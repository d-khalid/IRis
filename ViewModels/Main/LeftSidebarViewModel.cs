using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IRis.ViewModels;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using Tmds.DBus.Protocol;
using Avalonia;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel : ViewModelBase
{
    [ObservableProperty] 
    private SimulationViewModel _simulation = SimulationViewModel.GetInstance();


    [RelayCommand]
    private void AddAnd()
    {
        Simulation.PreviewObjects.Add(new AndGateViewModel() { 
            Width = 50,
            Height = 50,
            Opacity = 0.5
        });

        Simulation.PreviewMouseOffset = new Point(25, 25);
    }
}
