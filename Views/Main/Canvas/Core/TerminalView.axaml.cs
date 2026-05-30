using Avalonia.Controls;
using Avalonia.Input;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.Views.Main.Canvas.Core;


public partial class TerminalView : UserControl
{
    public TerminalView()
    {
        InitializeComponent();
    }


    public void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(sender as Control).Properties.IsLeftButtonPressed) return;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (!Simulation.GetInstance().Running)
            (DataContext as TerminalViewModel)!.PointerPressed();
    }


    public void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (!Simulation.GetInstance().Running)
            (DataContext as TerminalViewModel)!.PointerEntered();
    }


    public void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (!Simulation.GetInstance().Running)
            (DataContext as TerminalViewModel)!.PointerExited();
    }
}
