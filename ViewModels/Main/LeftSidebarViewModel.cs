using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IRis.ViewModels;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using Tmds.DBus.Protocol;
using Avalonia;
using IRis.ViewModels.Circuit.CircuitObjects.Core;
using IRis.Models.Core;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel : ViewModelBase
{
    [ObservableProperty] 
    private SimulationManager _simulation = SimulationManager.GetInstance();


    [RelayCommand]
    private void AddAnd()
    {
        TerminalViewModel i1 = new(new Terminal(TerminalType.Input));
        TerminalViewModel i2 = new(new Terminal(TerminalType.Input));

        AndGateViewModel gate = new() { Opacity = 0.5 };

        gate.AddInput(i1);
        gate.AddInput(i2);
        Simulation.PreviewObjects.Add(gate);
        Simulation.PreviewMouseOffset = new Point(25, 25);
    }
}
