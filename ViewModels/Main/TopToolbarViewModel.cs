using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services.Singleton;
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
        if (!Simulation.GetInstance().Running)
        {
            SimulationToggleContent = "Simulation: ON";
            SimulationToggleBackground = new SolidColorBrush(Colors.DarkGreen);
            Simulation.GetInstance().Start();
        }

        else
        {
            SimulationToggleContent = "Simulation: OFF";
            SimulationToggleBackground = new SolidColorBrush(Colors.DarkRed);
            Simulation.GetInstance().Stop();
        }
    }
}
