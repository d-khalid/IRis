using Avalonia.Controls;
using Avalonia.Input;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.CircuitObjects;


namespace IRis.Views.Main.Canvas.CircuitObjects;


public partial class WireView : UserControl
{
    public WireView()
    {
        InitializeComponent();
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(sender as Control).Properties.IsLeftButtonPressed) return;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (!Simulation.GetInstance().Running)
            (DataContext as WireViewModel)!.PointerPressed();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;

        if (!Simulation.GetInstance().Running)
            (DataContext as WireViewModel)!.PointerEntered();
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;

        if (!Simulation.GetInstance().Running)
            (DataContext as WireViewModel)?.PointerExited();
    }
}
