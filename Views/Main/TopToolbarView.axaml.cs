using Avalonia.Controls;
using Avalonia.Media;
using IRis.Models.Core;


namespace IRis.Views.Main;


public partial class TopToolbarView : UserControl
{
    public TopToolbarView()
    {
        InitializeComponent();
    }


    private void OnSimulationToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var sim = Simulation.GetInstance();

        if (!sim.Running)
        {
            SimulationToggle.Content = "Simulation: ON";
            SimulationToggle.Background = new SolidColorBrush(Colors.DarkGreen);
            sim.Start();
        }

        else
        {
            SimulationToggle.Content = "Simulation: OFF";
            SimulationToggle.Background = new SolidColorBrush(Colors.DarkRed);
            sim.End();
        }
    }

    private void OnToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnProbeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDLatchClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnWireClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
