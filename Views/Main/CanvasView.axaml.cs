using Avalonia.Controls;
using Avalonia.Input;
using IRis.Services.Singleton;
using IRis.ViewModels.Main;


namespace IRis.Views.Main;


public partial class CanvasView : UserControl
{
    public CanvasView()
    {
        InitializeComponent();
        DataContext = new CanvasViewModel();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (!Simulation.GetInstance().Running)
            (DataContext as CanvasViewModel)?.PointerEntered();
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (!Simulation.GetInstance().Running)
            (DataContext as CanvasViewModel)?.PointerExited();
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var ctrl = (sender as Control)!;

        if (!e.GetCurrentPoint(ctrl).Properties.IsLeftButtonPressed) return;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (!Simulation.GetInstance().Running)
            CanvasViewModel.PointerPressed(ctrl, e);
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (!Simulation.GetInstance().Running)
            (DataContext as CanvasViewModel)?.PointerMoved(sender, e);
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (!Simulation.GetInstance().Running)
            CanvasViewModel.PointerReleased(e);
    }
}
