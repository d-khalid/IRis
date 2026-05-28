using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;
using IRis.Views.Main.Canvas.CircuitObjects.Components;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main;


public partial class TopToolbarViewModel : ViewModelBase
{
    [ObservableProperty] private string _simulationToggleContent = "Simulation: OFF";
    [ObservableProperty] private Brush _simulationToggleBackground = new SolidColorBrush(Colors.DarkRed);


    [RelayCommand]
    private void SimulationToggle()
    {
        var sim = Simulation.GetInstance();

        if (!sim.Running)
        {
            SimulationToggleContent = "Simulation: ON";
            SimulationToggleBackground = new SolidColorBrush(Colors.DarkGreen);
            sim.Start();
        }

        else
        {
            SimulationToggleContent = "Simulation: OFF";
            SimulationToggleBackground = new SolidColorBrush(Colors.DarkRed);
            sim.End();
        }
    }
}
