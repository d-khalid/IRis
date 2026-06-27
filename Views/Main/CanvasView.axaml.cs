using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using IRis.Services;
using IRis.Services.Singleton;
using IRis.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;


namespace IRis.Views.Main;


public partial class CanvasView : UserControl
{
    public CanvasView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<CanvasViewModel>();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (AppState.Get().EditingAllowed)
            (DataContext as CanvasViewModel)?.PointerEntered();
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (AppState.Get().EditingAllowed)
            (DataContext as CanvasViewModel)?.PointerExited();
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var ctrl = (sender as Control)!;

        if (!e.GetCurrentPoint(ctrl).Properties.IsLeftButtonPressed) return;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (AppState.Get().EditingAllowed)
            (DataContext as CanvasViewModel)?.PointerPressed(ctrl, e);
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (AppState.Get().EditingAllowed)
            (DataContext as CanvasViewModel)?.PointerMoved(sender, e);
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (AppState.Get().EditingAllowed)
            (DataContext as CanvasViewModel)?.PointerReleased(e);
    }


    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        double step = AppState.Get().PanSensistivity * 10;
        Point mousePosition = AppState.Get().MousePosition;

        var m = ZoomBorder.Matrix;
        double delta = step / m.M11;    // normalize step to zoom scale (for MousePosition)

        switch (e.Key)
        {
            case Key.Left:
                ZoomBorder.SetMatrix(new Matrix(m.M11, m.M12, m.M21, m.M22, m.M31 + step, m.M32));
                mousePosition = new(mousePosition.X - delta, mousePosition.Y);
                break;

            case Key.Right:
                ZoomBorder.SetMatrix(new Matrix(m.M11, m.M12, m.M21, m.M22, m.M31 - step, m.M32));
                mousePosition = new(mousePosition.X + delta, mousePosition.Y);
                break;

            case Key.Up:
                ZoomBorder.SetMatrix(new Matrix(m.M11, m.M12, m.M21, m.M22, m.M31, m.M32 + step));
                mousePosition = new(mousePosition.X, mousePosition.Y - delta);
                break;

            case Key.Down:
                ZoomBorder.SetMatrix(new Matrix(m.M11, m.M12, m.M21, m.M22, m.M31, m.M32 - step));
                mousePosition = new(mousePosition.X, mousePosition.Y + delta);
                break;

            default:
                return;
        }

        AppState.Get().MousePosition = SimulationService.SnapPointToGrid(mousePosition);
        e.Handled = true;
    }
}
