using Avalonia.Controls;
using Avalonia.Markup.Xaml;


namespace IRis.Views.Main;


public partial class TopToolbarView : UserControl
{
    public TopToolbarView()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnProbeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDLatchClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnWireClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnSimulationToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
